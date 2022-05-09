using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.MathHelpers
{
    public static class MathHelper
    {
        public static float GetRelativeAngle(Vector3 origin, Vector3 target)
        {
            var vector2 = target - origin;
            var vector1 = new Vector2(0, 1);
            return ((MathF.Atan2(vector2.Z, vector2.X) - MathF.Atan2(vector1.Y, vector1.X)) * (180 / MathF.PI) + 360+180) % 360;
        }

        public static CardinalDirection GetQuadrant(Vector3 origin, Vector3 target)
        {
            return GetCardinalDirection(GetRelativeAngle(origin, target));
        }

        public static CardinalDirection GetCardinalDirection(float angle)
        {
            if (angle.InRange(45, 135)) return CardinalDirection.East;
            if (angle.InRange(135, 225)) return CardinalDirection.South;
            if (angle.InRange(225, 315)) return CardinalDirection.West;
            return CardinalDirection.North;
        }

        public static bool InRange(this float f, float inclusiveStart, float exclusiveEnd)
        {
            return f >= inclusiveStart && f < exclusiveEnd;
        }
    }
}
