using Liero.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Liero.Components
{
    public class Player : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Texture2D _crosshairTexture;
        private Vector2 _direction = new Vector2(10, 0);
        private Point _position;
        private Point _size = new Point(50, 100);

        private int _speed = 150;
        private Vector2 _force = Vector2.Zero;
        private float _gravity = 450f;

        private float _startFallTime = 0;
        private bool _isGrounded = true;

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

            if (!_isGrounded && _startFallTime == 0)
            {
                _force.Y += (float)Math.Sqrt(_gravity);
            }

            if (!_isGrounded && _force.Y > _gravity * time && _startFallTime == 0)
            {
                _startFallTime = (float)gameTime.TotalGameTime.TotalSeconds;
            }

            if (kbState.IsKeyDown(Keys.A))
            {
                velocity.X -= _speed * time;
            }

            if (kbState.IsKeyDown(Keys.D))
            {
                velocity.X += _speed * time;
            }

            if (_isGrounded)
            {
                if (kbState.IsKeyDown(Keys.Space))
                {
                    _force.Y = -_gravity;
                    _isGrounded = false;
                }
            }

            velocity.Y += (_gravity - _force.Y) * time;

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                var digSite = (CrosshairCenter + (_direction * 70)).ToPoint();
                Space.Instance.CreateCircle(digSite, (int)(_size.X * 1.5f));
            }

            // IsFalling
            if (_force.Y >= 0)
            {
                Console.WriteLine((float)gameTime.TotalGameTime.TotalSeconds - _startFallTime);
                var fallSpeedMultiplier = _startFallTime > 0 ? ((float)gameTime.TotalGameTime.TotalSeconds - _startFallTime) * 5 : 1f;
                velocity.Y = velocity.Y * fallSpeedMultiplier;

                if (!CollisionDetector.IsIntersecting(GetNextFrameBottomBoundingBox(velocity)))
                {
                    _position.Y += (int)velocity.Y;
                    _isGrounded = false;
                }
                else if (!_isGrounded)
                {
                    _isGrounded = true;
                    _startFallTime = 0;
                    _force.Y = 0;
                }
            }
            else
            {
                if (!CollisionDetector.IsIntersecting(GetNextFrameTopBoundingBox(velocity)))
                {
                    _position.Y -= (int)velocity.Y;
                }
                else
                {
                    _isGrounded = false;
                    _force.Y -= _force.Y;
                    _startFallTime = (float)gameTime.TotalGameTime.TotalSeconds;
                }
            }

            var nextXPosition = _position + new Vector2(velocity.X, 0).ToPoint();
            _position.X = nextXPosition.X;

            UpdateDirection(mouseState);

            Game1.GameCamera.SetPosition(Center.ToVector2());
        }

        private void UpdateDirection(MouseState mouseState)
        {
            var matrix = Matrix.Invert(Game1.GameCamera.TransformNoZoom);
            var mouseWorldPosition = Vector2.Transform(mouseState.Position.ToVector2() - Center.ToVector2(), matrix);
            _direction = Vector2.Normalize(mouseWorldPosition);
        }

        private Rectangle GetNextFrameTopBoundingBox(Vector2 velocity)
        {
            var nextYPosition = _position + new Vector2(0, velocity.Y).ToPoint();
            var boxSize = _size.X / 4;
            return new Rectangle(nextYPosition.X + (boxSize / 2), nextYPosition.Y, boxSize, boxSize);
        }

        private Rectangle GetNextFrameBottomBoundingBox(Vector2 velocity)
        {
            var nextYPosition = _position + new Vector2(0, velocity.Y).ToPoint();
            var boxSize = _size.X / 4;
            return new Rectangle(nextYPosition.X, nextYPosition.Y + _size.Y - boxSize, _size.X, boxSize);
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
