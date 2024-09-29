using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECommons.MathHelpers;

public static class MathHelper
{
    public static List<Vector2> CalculateCircularMovement(Vector2 centerPoint, Vector2 initialPoint, Vector2 exitPoint, out List<List<Vector2>> candidates, float precision = 36f, Vector2? clampRadius = null)
    {
        var step = 360f / precision;
        List<Vector2> points = [];
        var distance = Vector2.Distance(centerPoint, initialPoint);
        if(clampRadius != null) distance.ValidateRange(clampRadius.Value.X, clampRadius.Value.Y);
        for(var x = 0f;x < 360f;x += step)
        {
            var p = MathF.SinCos(x.DegToRad());
            points.Add(new(p.Sin * distance, p.Cos * distance));
        }
        var closestPoints = points.OrderBy(x => Vector2.Distance(initialPoint, x)).Take(2).ToList();
        var finalPoint = points.OrderBy(x => Vector2.Distance(exitPoint, x)).First();
        List<List<Vector2>> retCandidates = [];
        foreach(var point in closestPoints)
        {
            void Process(int mod)
            {
                var pointIndex = points.IndexOf(point);
                var list = new List<Vector2>();
                do
                {
                    list.Add(points.CircularSelect(pointIndex));
                    pointIndex += mod;
                }
                while(points[^1] != finalPoint);
                retCandidates.Add(list);
            }
            Process(1);
            Process(-1);
        }
        candidates = retCandidates;
        return retCandidates.OrderBy(CalculateDistance).First();
    }

    public static float CalculateDistance(IEnumerable<Vector2> vectors)
    {
        var distance = 0f;
        for(var i = 0; i < vectors.Count() - 1; i++)
        {
            distance += Vector2.Distance(vectors.ElementAt(i), vectors.ElementAt(i + 1));
        }
        return distance;
    }

    public static float DegToRad(this float val)
    {
        return (float)(MathF.PI / 180f * val);
    }

    public static Vector3 RotateWorldPoint(Vector3 origin, float angle, Vector3 p)
    {
        if(angle == 0f) return p;
        var s = (float)Math.Sin(angle);
        var c = (float)Math.Cos(angle);

        // translate point back to origin:
        p.X -= origin.X;
        p.Z -= origin.Z;

        // rotate point
        var xnew = p.X * c - p.Z * s;
        var ynew = p.X * s + p.Z * c;

        // translate point back:
        p.X = xnew + origin.X;
        p.Z = ynew + origin.Z;
        return p;
    }

    public static float Float(this int i)
    {
        return (float)i;
    }

    public static Vector2 ToVector2(this Vector3 vector3)
    {
        return new Vector2(vector3.X, vector3.Z);
    }

    public static Vector3 ToVector3(this Vector2 vector2)
    {
        return vector2.ToVector3(Svc.ClientState.LocalPlayer?.Position.Y ?? 0);
    }

    public static Vector3 ToVector3(this Vector2 vector2, float Y)
    {
        return new Vector3(vector2.X, Y, vector2.Y);
    }

    /// <summary>
    /// Degrees
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float GetRelativeAngle(Vector3 origin, Vector3 target)
    {
        return GetRelativeAngle(origin.ToVector2(), target.ToVector2());
    }

    /// <summary>
    /// Degrees
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float GetRelativeAngle(Vector2 origin, Vector2 target)
    {
        var vector2 = target - origin;
        var vector1 = new Vector2(0, 1);
        return ((MathF.Atan2(vector2.Y, vector2.X) - MathF.Atan2(vector1.Y, vector1.X)) * (180 / MathF.PI) + 360 + 180) % 360;
    }

    public static float RadToDeg(this float f)
    {
        return (f * (180 / MathF.PI) + 360) % 360;
    }

    public static CardinalDirection GetCardinalDirection(Vector3 origin, Vector3 target)
    {
        return GetCardinalDirection(GetRelativeAngle(origin, target));
    }

    public static CardinalDirection GetCardinalDirection(Vector2 origin, Vector2 target)
    {
        return GetCardinalDirection(GetRelativeAngle(origin, target));
    }

    /// <summary>
    /// From angle in degrees
    /// </summary>
    /// <param name="angle">Degrees</param>
    /// <returns></returns>
    public static CardinalDirection GetCardinalDirection(float angle)
    {
        if(angle.InRange(45, 135, false)) return CardinalDirection.East;
        if(angle.InRange(135, 225, false)) return CardinalDirection.South;
        if(angle.InRange(225, 315, false)) return CardinalDirection.West;
        return CardinalDirection.North;
    }

    public static bool InRange(this double f, double inclusiveStart, double end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }

    public static bool InRange(this float f, float inclusiveStart, float end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }

    public static bool InRange(this long f, long inclusiveStart, long end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }

    public static bool InRange(this int f, int inclusiveStart, int end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }

    public static bool InRange(this short f, short inclusiveStart, short end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }

    public static bool InRange(this byte f, byte inclusiveStart, byte end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }

    public static bool InRange(this ulong f, ulong inclusiveStart, ulong end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }

    public static bool InRange(this uint f, uint inclusiveStart, uint end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }

    public static bool InRange(this ushort f, ushort inclusiveStart, ushort end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }

    public static bool InRange(this sbyte f, sbyte inclusiveStart, sbyte end, bool includeEnd = false)
    {
        return f >= inclusiveStart && (includeEnd ? f <= end : f < end);
    }
}
