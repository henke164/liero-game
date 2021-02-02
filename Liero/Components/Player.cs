using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Liero.Components
{
    public class Player : GameObject
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _crosshairTexture;
        private Character _character;
        private Point _size = new Point(50, 100);
        private float _acceleration = 150;
        private float _jumpTime = 0;
        private float _shootAt = 0f;

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
            Position = new Point(10, 0);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _crosshairTexture = Game.Content.Load<Texture2D>("images/crosshair");
            _character = new Character(Game);
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.A))
            {
                AddForce(new Vector2(-_acceleration, 0));
            }

            if (kbState.IsKeyDown(Keys.D))
            {
                AddForce(new Vector2(_acceleration, 0));
            }

            if (kbState.IsKeyDown(Keys.Space))
            {
                var now = (float)gameTime.TotalGameTime.TotalSeconds;
                var lastJump = now - _jumpTime;
                if (lastJump > 0.5f && IsGrounded)
                {
                    var xVel = GetXVelocity();
                    AddForce(new Vector2(0, -Gravity + Math.Abs(xVel / 2)));
                    IsGrounded = false;
                    _jumpTime = (float)gameTime.TotalGameTime.TotalSeconds;
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                var now = (float)gameTime.TotalGameTime.TotalSeconds;
                var digSite = (CrosshairCenter + (FaceDirection * 70)).ToPoint();
                Space.Instance.CreateCircle(digSite, (int)(_size.X * 1.5f));
                _shootAt = now;
            }

            UpdateDirection(mouseState);

            Game1.GameCamera.SetPosition(Center.ToVector2());

            base.Update(gameTime);
        }

        private void UpdateDirection(MouseState mouseState)
        {
            var matrix = Matrix.Invert(Game1.GameCamera.TransformNoZoom);
            var mouseWorldPosition = Vector2.Transform(mouseState.Position.ToVector2() - Center.ToVector2(), matrix);
            FaceDirection = Vector2.Normalize(mouseWorldPosition);
            _character.SetDirection(FaceDirection);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Game1.GameCamera.Transform);


            var crosshairPosition = CrosshairCenter + (FaceDirection * 155);
            _spriteBatch.Draw(_crosshairTexture, crosshairPosition, Color.White);

            var now = (float)gameTime.TotalGameTime.TotalSeconds;
            var isShooting = now - _shootAt < 0.1f;
            _character.Draw(Position, isShooting, _spriteBatch);
            _spriteBatch.End();
        }
    }
}
