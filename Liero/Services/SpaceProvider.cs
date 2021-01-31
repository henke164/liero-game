using Liero.Components;
using Microsoft.Xna.Framework;
using System;

namespace Liero.Services
{
    public class SpaceDetector
    {
        private Point _position;
        private Point _size;
        private int _speed;

        public SpaceDetector(Point position, Point size, int speed)
        {
            UpdateTransform(position, size, speed);
        }

        public void UpdateTransform(Point position, Point size, int speed)
        {
            _position = position;
            _size = size;
            _speed = speed;
        }

        public int GetHighestGround()
        {
            var spaceScan = new Rectangle
            {
                X = _position.X,
                Y = _position.Y + (_size.Y / 2),
                Width = _size.X,
                Height = _size.Y
            };

            return Space.Instance.GetGroundPosition(spaceScan);
        }

        public bool CanGoLeft(float gameTime)
        {
            var spaceScan = new Rectangle
            {
                X = _position.X - (int)Math.Ceiling(_speed * gameTime),
                Y = _position.Y - (int)(_size.Y * 0.1f),
                Width = (int)Math.Ceiling(_speed * gameTime),
                Height = _size.Y - (int)(_size.Y * 0.1f)
            };

            return Space.Instance.HasSpace(spaceScan);
        }

        public bool CanGoRight(float gameTime)
        {
            var spaceScan = new Rectangle
            {
                X = _position.X + _size.X + (int)Math.Ceiling(_speed * gameTime),
                Y = _position.Y - (int)(_size.Y * 0.1f),
                Width = (int)Math.Ceiling(_speed * gameTime),
                Height = _size.Y - (int)(_size.Y * 0.1f)
            };

            return Space.Instance.HasSpace(spaceScan);
        }
    }
}
