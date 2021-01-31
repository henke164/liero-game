using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Game1.GameCamera.Transform);
            _spriteBatch.Draw(canvas, new Rectangle(0, 0, levelSize.Width, levelSize.Height), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public int GetGroundPosition(Rectangle rectangle)
        {
            var lowestY = int.MaxValue;
            for (var x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (var y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    var i = x + levelSize.Width * y;
                    if (i < 0 || i >= pixels.Length)
                    {
                        continue;
                    }

                    if (pixels[i] != 0xFF00FF00)
                    {
                        if (y < lowestY)
                        {
                            lowestY = y;
                        }
                    }
                }
            }
            return lowestY;
        }

        public bool HasSpace(Rectangle rectangle)
        {
            for (var x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (var y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    var i = x + levelSize.Width * y;
                    if (i < 0 || i >= pixels.Length)
                    {
                        return false;
                    }

                    if (pixels[i] != 0xFF00FF00)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void CreateCircle(Point origin, int radius)
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

        private void CreateSpace(int x, int y)
        {
            var i = x + levelSize.Width * y;
            if (i < 0 || i > pixels.Length - 1)
            {
                return;
            }

            pixels[i] = 0xFF00FF00;
        }
    }
}
