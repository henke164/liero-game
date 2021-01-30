using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Liero.Components
{
    public class Player : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Vector2 _position;
        private Vector2 _size = new Vector2(100, 100);

        private float speed = 3f;

        public Player(Game game) 
            : base (game)
        {
            _position = new Vector2(0, 0);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texture = Game.Content.Load<Texture2D>("images/player");
        }

        private int GetHighestGround()
        {
            var spaceScan = new Rectangle
            {
                X = (int)_position.X,
                Y = (int)_position.Y + ((int)_size.Y / 2),
                Width = (int)_size.X,
                Height = (int)_size.Y
            };

            return Space.Instance.GetGroundPosition(spaceScan);
        }

        private bool CanGoLeft()
        {
            var spaceScan = new Rectangle
            {
                X = (int)_position.X - (int)speed,
                Y = (int)_position.Y,
                Width = (int)speed,
                Height = (int)_size.Y
            };

            return Space.Instance.HasSpace(spaceScan);
        }

        private bool CanGoRight()
        {
            var spaceScan = new Rectangle
            {
                X = (int)_position.X + (int)_size.X,
                Y = (int)_position.Y,
                Width = (int)speed,
                Height = (int)_size.Y
            };

            return Space.Instance.HasSpace(spaceScan);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var velocity = new Vector2(0, 0);
            var kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.A) && CanGoLeft())
            {
                velocity.X = -speed;
            }

            if (kbState.IsKeyDown(Keys.D) && CanGoRight())
            {
                velocity.X += speed;
            }

            var highestGround = GetHighestGround();
            if (highestGround == -1 || highestGround > _position.Y + (int)_size.Y)
            {
                velocity.Y += speed;
            }
            else if (
                highestGround > _position.Y + (int)_size.Y * 0.6 &&
                highestGround < _position.Y + (int)_size.Y * 0.8)
            {
                _position.Y -= speed;// highestGround - (int)_size.Y;
            }

            if (kbState.IsKeyDown(Keys.Space))
            {
                var midPoint = new Vector2(_position.X + _size.X / 2, _position.Y + _size.Y / 2);
                Space.Instance.CreateCircle(midPoint, (int)(_size.X * 1f));
            }

            _position += velocity;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(_texture,
                new Rectangle((int)_position.X, (int)_position.Y, (int)_size.X, (int)_size.Y),
                Color.White);
            
            _spriteBatch.End();
        }
    }
}
