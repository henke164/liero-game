using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Liero.Components
{
    public class Character
    {
        private Texture2D _head;
        private Texture2D _torso;
        private Texture2D _legs;
        private Texture2D _explosion;
        private Vector2 _direction;

        public Character(Game game)
        {
            _head = game.Content.Load<Texture2D>("images/head");
            _torso = game.Content.Load<Texture2D>("images/torso");
            _legs = game.Content.Load<Texture2D>("images/legs");
            _explosion = game.Content.Load<Texture2D>("images/explosion");
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        public void Draw(Point position, bool isShooting, SpriteBatch spriteBatch)
        {
            var torsoPosition = new Point(position.X, position.Y + _head.Height);
            var legsPosition = new Point(position.X, torsoPosition.Y);

            var angle = (float)Math.Atan(_direction.Y / _direction.X);   //radians

            spriteBatch.Draw(_legs, legsPosition.ToVector2(), Color.White);

            var explosionCenterX = _direction.X > 0 ? -(_torso.Width / 2) : _torso.Width + _torso.Width / 2;

            if (isShooting)
            {
                spriteBatch.Draw(_explosion,
                    new Rectangle(
                        new Point(
                            (int)(torsoPosition.X + _torso.Width / 2),
                            (int)(torsoPosition.Y + _torso.Height / 2)
                        ),
                        new Point(_torso.Width, _torso.Height)
                    ),
                    null,
                    Color.White,
                    angle,
                    new Vector2(explosionCenterX, _torso.Height / 2),
                    _direction.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0);
            }

            spriteBatch.Draw(_torso, 
                new Rectangle(
                    new Point(
                        (int)(torsoPosition.X + _torso.Width / 2),
                        (int)(torsoPosition.Y + _torso.Height / 2)
                    ),
                    new Point(_torso.Width, _torso.Height)
                ),
                null,
                Color.White,
                angle,
                new Vector2(_torso.Width / 2, _torso.Height / 2),
                _direction.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0);

            spriteBatch.Draw(_head,
                new Rectangle(
                    new Point(
                        (int)(torsoPosition.X + _torso.Width / 2),
                        (int)(torsoPosition.Y + _torso.Height / 2)
                    ),
                    new Point(_torso.Width, _torso.Height)
                ),
                null,
                Color.White,
                angle,
                new Vector2(_head.Width / 2, _head.Height + (_torso.Height / 2) + 2),
                _direction.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0);
        }
    }
}
