using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Liero.Components
{
    public class Space : DrawableGameComponent
    {
        public static Space Instance;
        private SpriteBatch _spriteBatch;
        Texture2D canvas;
        Rectangle levelSize;
        uint[] pixels;

        public Space(Game game) 
            : base (game)
        {
            Instance = this;
        }

        public override void Initialize()
        {
            base.Initialize();
            levelSize = new Rectangle(0, 0, 2000, 1000);
            canvas = new Texture2D(GraphicsDevice, levelSize.Width, levelSize.Height, false, SurfaceFormat.Color);
            pixels = new uint[levelSize.Width * levelSize.Height];
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Textures[0] = null;
            canvas.SetData(pixels, 0, levelSize.Width * levelSize.Height);

            _spriteBatch.Begin();
            _spriteBatch.Draw(canvas, new Rectangle(0, 0, levelSize.Width, levelSize.Height), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public int GetGroundPosition(Rectangle rectangle)
        {
            for (var x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (var y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    var i = (int)x + levelSize.Width * (int)y;
                    if (i < 0 || i > pixels.Length)
                    {
                        continue;
                    }

                    if (pixels[i] != 0xFF00FF00)
                    {
                        return y;
                    }
                }
            }
            return -1;
        }

        public bool HasSpace(Rectangle rectangle)
        {
            for (var x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (var y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    var i = (int)x + levelSize.Width * (int)y;
                    if (i > 0 && i < pixels.Length && pixels[i] == 0xFF00FF00)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void CreateCircle(Vector2 origin, int radius)
        {
            for (int x = -radius; x < radius; x++)
            {
                int height = (int)Math.Sqrt(radius * radius - x * x);

                for (int y = -height; y < height; y++)
                {
                    CreateSpace(x + origin.X, y + origin.Y);
                }
            }
        }

        private void CreateSpace(float x, float y)
        {
            var i = (int)x + levelSize.Width * (int)y;
            if (i < 0 || i > pixels.Length)
            {
                return;
            }

            pixels[i] = 0xFF00FF00;
        }
    }
}
