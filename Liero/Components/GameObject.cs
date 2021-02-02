using Liero.Services;
using Microsoft.Xna.Framework;
using System;

namespace Liero.Components
{
    public class GameObject : DrawableGameComponent
    {
        protected bool IsGrounded = true;
        protected float Gravity = 450f;
        protected Vector2 FaceDirection = new Vector2(10, 0);
        protected Point Position;
        protected float MaxSpeed = 350f;

        private Vector2 _force = Vector2.Zero;
        private Point _size = new Point(50, 100);
        private float _startFallTime = 0;

        protected Point Center
        {
            get
            {
                return new Point(Position.X + (_size.X / 2), Position.Y + (_size.Y / 2));
            }
        }

        public GameObject(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            Space.Instance.CreateCircle(Center, (int)(_size.X * 1.5f));
            base.Initialize();
        }

        protected void AddForce(Vector2 force)
        {
            _force += force;
        }

        protected float GetXVelocity()
        {
            if (_force.X > 0 && IsGrounded)
            {
                _force.X -= (float)Math.Sqrt(MaxSpeed);
                if (_force.X < 0)
                {
                    _force.X = 0;
                }
            }

            if (_force.X < 0 && IsGrounded)
            {
                _force.X += (float)Math.Sqrt(MaxSpeed);
                if (_force.X > 0)
                {
                    _force.X = 0;
                }
            }

            if (_force.X > MaxSpeed)
            {
                _force.X = MaxSpeed;
            }

            if (_force.X < -MaxSpeed)
            {
                _force.X = -MaxSpeed;
            }

            return _force.X;
        }

        public override void Update(GameTime gameTime)
        {
            var velocity = Vector2.Zero;

            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.X += GetXVelocity() * time;
            velocity.Y += (Gravity - _force.Y) * time;

            HandleGravity(gameTime, velocity);
            HandleGrounding(gameTime);
            HandleStrafing(velocity);
        }

        protected float FallDuration(GameTime gameTime)
        {
            return ((float)gameTime.TotalGameTime.TotalSeconds - _startFallTime);
        }

        private void HandleStrafing(Vector2 velocity)
        {
            if (_force.X < 0 && !CollisionDetector.IsIntersecting(GetNextFrameLeftBoundingBox(velocity)))
            {
                Position.X += (int)velocity.X;
            }

            if (_force.X > 0 && !CollisionDetector.IsIntersecting(GetNextFrameRightBoundingBox(velocity)))
            {
                Position.X += (int)velocity.X;
            }
        }

        private void HandleGravity(GameTime gameTime, Vector2 velocity)
        {
            var time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsGrounded && _startFallTime == 0)
            {
                _force.Y += (float)Math.Sqrt(Gravity - Math.Abs(GetXVelocity()));
            }
            else
            {
                _force.Y = 0;
            }

            if (!IsGrounded && _force.Y > Gravity * time && _startFallTime == 0)
            {
                _startFallTime = (float)gameTime.TotalGameTime.TotalSeconds;
            }

            // Is falling
            if (_force.Y >= 0)
            {
                var fallSpeedMultiplier = _startFallTime > 0 ? FallDuration(gameTime) * 5 : 1f;
                velocity.Y = velocity.Y * fallSpeedMultiplier;

                if (!CollisionDetector.IsIntersecting(GetNextFrameBottomBoundingBox(velocity)))
                {
                    Position.Y += (int)velocity.Y;
                    IsGrounded = false;
                }
                else if (!IsGrounded)
                {
                    IsGrounded = true;
                    _startFallTime = 0;
                    _force.Y = 0;
                }

                return;
            }

            if (!CollisionDetector.IsIntersecting(GetNextFrameTopBoundingBox(velocity)))
            {
                Position.Y -= (int)velocity.Y;
            }
            else
            {
                 IsGrounded = false;
                _force.Y = -_force.Y * 2;
                _force.X = 0;
                _startFallTime = (float)gameTime.TotalGameTime.TotalSeconds;
            }

        }

        private void HandleGrounding(GameTime gameTime)
        {
            var rect = GetNextFrameBottomBoundingBox(Vector2.Zero);
            var ground = CollisionDetector.GetGroundY(rect);
            if (IsGrounded)
            {
                if (ground < Position.Y + _size.Y && ground > Position.Y + _size.Y - 20f)
                {
                    Position.Y = (int)ground - _size.Y;
                }
            }
        }

        private Rectangle GetNextFrameRightBoundingBox(Vector2 velocity)
        {
            Console.WriteLine("Check right");
            var nextXPosition = Position + new Vector2(velocity.X, 0).ToPoint();
            return new Rectangle(nextXPosition.X + 48, nextXPosition.Y + 4, 2, 83);
        }

        private Rectangle GetNextFrameLeftBoundingBox(Vector2 velocity)
        {
            Console.WriteLine("Check left");
            var nextXPosition = Position + new Vector2(velocity.X, 0).ToPoint();
            return new Rectangle(nextXPosition.X, nextXPosition.Y + 4, 2, 83);
        }

        private Rectangle GetNextFrameTopBoundingBox(Vector2 velocity)
        {
            var nextYPosition = Position + new Vector2(0, velocity.Y).ToPoint();
            return new Rectangle(nextYPosition.X + 2, nextYPosition.Y, 46, 6);
        }

        private Rectangle GetNextFrameBottomBoundingBox(Vector2 velocity)
        {
            var nextYPosition = Position + new Vector2(0, velocity.Y).ToPoint();
            return new Rectangle(nextYPosition.X + 2, nextYPosition.Y + 85, 46, 15);
        }
    }
}
