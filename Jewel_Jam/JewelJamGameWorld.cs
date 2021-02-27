﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Jewel_Jam
{
    internal class JewelJamGameWorld : GameObjectList
    {
        private enum GameState
        { TitleScreen, Playing, HelpScreen, GameOver }

        private GameState currentState;

        private JewelCart jewelCart;
        private SpriteGameObject titleScreen, gameOverScreen, helpScreen;

        private const int GridWidth = 5;
        private const int GridHeight = 10;
        private const int CellSize = 85;

        public Point Size { get; private set; }
        public int Score { get; private set; }

        public JewelJamGameWorld()
        {
            SpriteGameObject background = new SpriteGameObject("spr_background");
            Size = new Point(background.Width, background.Height);
            AddChild(background);

            // Create PlayingField and add it do gameWorld
            GameObjectList playingField = new GameObjectList();
            playingField.Position = new Vector2(85, 150);
            AddChild(playingField);

            // Create new grid and add it to playingField
            JewelGrid grid = new JewelGrid(GridWidth, GridHeight, CellSize);
            playingField.AddChild(grid);

            // Create new RowSelector and add it to playingField
            playingField.AddChild(new RowSelector(grid));

            // Add background sprite for the score object
            SpriteGameObject scoreFrame = new SpriteGameObject("spr_scoreframe");
            scoreFrame.Position = new Vector2(20, 20);
            AddChild(scoreFrame);

            // Add the object that displays the score
            ScoreGameObject scoreObject = new ScoreGameObject();
            scoreObject.Position = new Vector2(270, 30);
            AddChild(scoreObject);

            jewelCart = new JewelCart(new Vector2(410, 230));
            AddChild(jewelCart);

            titleScreen = AddOverlay("spr_title");
            gameOverScreen = AddOverlay("spr_gameover");
            helpScreen = AddOverlay("spr_frame_help");

            GoToState(GameState.TitleScreen);
        }

        public void AddScore(int points)
        {
            Score += points;
            jewelCart.PushBack();
        }

        public override void Reset()
        {
            base.Reset();
            Score = 0;
        }

        public SpriteGameObject AddOverlay(string sprite)
        {
            SpriteGameObject result = new SpriteGameObject(sprite);
            result.SetOriginToCenter();
            result.Position = new Vector2(Size.X / 2, Size.Y / 2);
            AddChild(result);

            return result;
        }

        private void GoToState(GameState newState)
        {
            currentState = newState;
            titleScreen.Visible = currentState == GameState.TitleScreen;
            helpScreen.Visible = currentState == GameState.HelpScreen;
            gameOverScreen.Visible = currentState == GameState.GameOver;
        }

        public override void Update(GameTime gameTime)
        {
            if (currentState == GameState.Playing)
            {
                base.Update(gameTime);
                if (jewelCart.GlobalPosition.X > Size.X - 230)
                    GoToState(GameState.GameOver);
            }
        }

        public override void HandleInput(InputHelper inputHelper)
        {
            if (currentState == GameState.Playing)
            {
                base.HandleInput(inputHelper);
            }
            else if (currentState == GameState.TitleScreen || currentState == GameState.GameOver)
            {
                if (inputHelper.KeyPressed(Keys.Space))
                {
                    Reset();
                    GoToState(GameState.Playing);
                }
            }
            else if (currentState == GameState.HelpScreen)
            {
                if (inputHelper.KeyPressed(Keys.Space))
                {
                    GoToState(GameState.Playing);
                }
            }
        }
    }
}