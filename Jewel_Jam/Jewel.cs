﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Net.Security;

namespace Jewel_Jam
{
    internal class Jewel : SpriteGameObject
    {
        public int ColorType { get; private set; }
        public int ShapeType { get; private set; }
        public int NumberType { get; private set; }
        public Vector2 TargetPosition { get; set; }

        Rectangle spriteRectangle;

        GlitterField glitters;

        public Jewel() : base("spr_jewels")
        {
            ColorType = ExtendedGame.Random.Next(3);
            ShapeType = ExtendedGame.Random.Next(3);
            NumberType = ExtendedGame.Random.Next(3);

            int index = 9 * ColorType + 3 * ShapeType + NumberType;
            spriteRectangle = new Rectangle(index * sprite.Height, 0, sprite.Height, sprite.Height);

            TargetPosition = Vector2.Zero;

            glitters = new GlitterField(sprite, 2, spriteRectangle);
            glitters.Parent = this;
            glitters.Position = -spriteRectangle.Location.ToVector2();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, GlobalPosition, spriteRectangle, Color.White);
            glitters.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 diff = TargetPosition - Position;
            velocity = diff * 8;
            base.Update(gameTime);
            glitters.Update(gameTime);
        }

    }
}