using Liero.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Liero.Components
{
    public class Player : DrawableGameComponent
    {
        private SpaceDetector _spaceDetector;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Texture2D _crosshairTexture;
        private Vector2 _direction = new Vector2(10, 0);
        private Point _position;
        private Point _size = new Point(50, 100);

        private int _speed = 150;
        private Vector2 _force = Vector2.Zero;
        private float _gravity = 300f;

        private Point Center
        {
            get
            {
                return new Point(_position.X + (_size.X / 2), _position.Y + (_size.Y / 2));
            }
        }

        private Vector2 CrosshairCenter
        {
            get
            {
                return new Vector2(Center.X - (_crosshairTexture.Width / 2), Center.Y - (_crosshairTexture.Height / 2));
            }
        }

        public Player(Game game) 
            : base (game)
        {
            _position = new Point(0, 0);
            _spaceDetector = new SpaceDetector(_position, _size, _speed);
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
            _crosshairTexture = Game.Content.Load<Texture2D>("images/crosshair");
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var kbState = Keyboard.GetState();

            var velocity = Vector2.Zero;

            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kbState.IsKeyDown(Keys.A) && _spaceDetector.CanGoLeft(time))
            {
                velocity.X -= _speed * time;
            }

            if (kbState.IsKeyDown(Keys.D) && _spaceDetector.CanGoRight(time))
            {
                velocity.X += _speed * time;
            }

            var currentGravity = _force.Y == 0 ? _gravity * time
                : (-_force.Y > _gravity * time) ? _gravity * time : -_force.Y;
                
            if (_force.Y <= 0)
            {
                if (kbState.IsKeyDown(Keys.Space))
                {
                    _force = new Vector2(0, _gravity);
                }

                var highestGround = _spaceDetector.GetHighestGround();
                if (highestGround == int.MaxValue || highestGround > _position.Y + _size.Y - currentGravity) 
                {
                    velocity.Y += currentGravity;
                }
                else if (highestGround != int.MaxValue && highestGround < _position.Y + _size.Y + (_speed * time))
                {
                    velocity.Y -= _speed * time;
                }
            }
            else
            {
                velocity.Y -= _force.Y * time;
            }

            _force.Y -= _gravity * time;

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                var digSite = (CrosshairCenter + (_direction * 70)).ToPoint();
                Space.Instance.CreateCircle(digSite, (int)(_size.X * 1.5f));
            }

            _position += velocity.ToPoint();

            _spaceDetector.UpdateTransform(_position, _size, _speed);

            UpdateDirection(mouseState);

            Game1.GameCamera.SetPosition(Center.ToVector2());
        }

        private void UpdateDirection(MouseState mouseState)
        {
            var matrix = Matrix.Invert(Game1.GameCamera.TransformNoZoom);
            var mouseWorldPosition = Vector2.Transform(mouseState.Position.ToVector2() - Center.ToVector2(), matrix);
            _direction = Vector2.Normalize(mouseWorldPosition);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Game1.GameCamera.Transform);

            _spriteBatch.Draw(_texture, new Rectangle(_position, _size), Color.White);

            var crosshairPosition = CrosshairCenter + (_direction * 155);
            _spriteBatch.Draw(_crosshairTexture, crosshairPosition, Color.White);

            _spriteBatch.End();
        }
    }
}
