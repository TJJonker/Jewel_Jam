﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography.X509Certificates;

namespace Jewel_Jam
{
    internal class JewelGrid : GameObject
    {
        private Jewel[,] grid;
        private int cellSize;

        public int Height { get; private set; }
        public int Width { get; private set; }

        public JewelGrid(int width, int height, int cellSize)
        {
            Width = width;
            Height = height;
            this.cellSize = cellSize;

            Reset();
        }

        public override void Reset()
        {
            grid = new Jewel[Width, Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    AddJewel(x, y, y);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Jewel jewel in grid)
                jewel.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Jewel jewel in grid)
                jewel.Update(gameTime);
        }

        /// <summary>
        /// Returns position based on cellsize
        /// </summary>
        /// <param name="x">X position on the grid</param>
        /// <param name="y">Y position on the grid</param>
        /// <returns></returns>
        public Vector2 GetCellPosition(int x, int y)
        {
            return new Vector2(x * cellSize, y * cellSize);
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (!inputHelper.KeyPressed(Keys.Space))
                return;

            int mid = Width / 2;
            int extraScore = 10;
            int combo = 0;

            for (int y = 0; y < Height - 2; y++)
            {
                if(IsValidCombination(grid[mid, y], grid[mid, y + 1], grid[mid, y + 2]))
                {
                    RemoveJewel(mid, y, -1);
                    RemoveJewel(mid, y + 1, -2);
                    RemoveJewel(mid, y + 2, -3);
                    y += 2;
                    combo++;
                    JewelJam.GameWorld.AddScore(extraScore);
                    extraScore *= 2;
                }
            }
            if (combo == 2)
            {
                JewelJam.GameWorld.DoubleComboScored();
                ExtendedGame.AssetManager.PlaySoundEffect("snd_double");
            }
            else if (combo == 3)
            {
                JewelJam.GameWorld.TripleComboScored();
                ExtendedGame.AssetManager.PlaySoundEffect("snd_triple");
            }
            else if(combo == 1)
            {
                ExtendedGame.AssetManager.PlaySoundEffect("snd_single");
            }
            else
            {
                ExtendedGame.AssetManager.PlaySoundEffect("snd_error");
            }
        }
        
        private void RemoveJewel(int x, int y, int yStartForNewJewel)
        {
            for (int row = y; row > 0; row--)
            {
                grid[x, row] = grid[x, row - 1];
                grid[x, row].TargetPosition = GetCellPosition(x, row);
            }

            AddJewel(x, 0, yStartForNewJewel);
        }

        private void AddJewel(int x, int yTarget, int yStart)
        {
            grid[x, yTarget] = new Jewel()
            {
                Position = GetCellPosition(x, yStart),
                Parent = this,
                TargetPosition = GetCellPosition(x, yTarget)
            };


        }

        public void ShiftRowLeft(int selectedRow)
        {
            // Store the left most Jewel for backup
            Jewel first = grid[0, selectedRow];

            // Replace all the Jewels with their right neighbour
            for (int x = 0; x < Width - 1; x++)
            {
                grid[x, selectedRow] = grid[x + 1, selectedRow];
                grid[x, selectedRow].TargetPosition = GetCellPosition(x, selectedRow);
            }

            // Re-insert the backup Jewel in the Right most spot
            grid[Width - 1, selectedRow] = first;
            grid[Width - 1, selectedRow].Position = GetCellPosition(Width, selectedRow);
            grid[Width - 1, selectedRow].TargetPosition = GetCellPosition(Width - 1, selectedRow);
        }

        public void ShiftRowRight(int selectedRow)
        {
            // Store the right most Jewel for backup
            Jewel first = grid[Width - 1, selectedRow];

            // replace all the Jewel with their left neighbour
            for(int x = Width - 1; x > 0; x--)
            {
                grid[x, selectedRow] = grid[x - 1, selectedRow];
                grid[x, selectedRow].TargetPosition = GetCellPosition(x, selectedRow);
            }

            // Replace the left most jewel with the backup
            grid[0, selectedRow] = first;
            grid[0, selectedRow].Position = GetCellPosition(-1, selectedRow);
            grid[0, selectedRow].TargetPosition = GetCellPosition(0, selectedRow);
        }

        private bool IsValidCombination(Jewel a, Jewel b, Jewel c)
        {
            return IsConditionValid(a.ShapeType, b.ShapeType, c.ShapeType)
                && IsConditionValid(a.ColorType, b.ColorType, c.ColorType)
                && IsConditionValid(a.NumberType, b.NumberType, c.NumberType);
        }

        private bool AllEqual(int a, int b, int c)
        {
            return a == b && b == c;
        }

        private bool AllDifferent(int a, int b, int c)
        {
            return a != b && b != c && c != a;
        }

        private bool IsConditionValid(int a, int b, int c)
        {
            return AllEqual(a, b, c) || AllDifferent(a, b, c);
        }
    }
}