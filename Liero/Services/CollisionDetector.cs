using Liero.Components;
using Microsoft.Xna.Framework;

namespace Liero.Services
{
    public static class CollisionDetector
    {
        public static bool IsIntersecting(Rectangle rectangle)
        {
            return !Space.Instance.HasSpace(rectangle);
        }
    }
}
