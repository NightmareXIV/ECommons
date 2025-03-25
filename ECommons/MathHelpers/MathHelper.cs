using ECommons.DalamudServices;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECommons.MathHelpers;

public static class MathHelper
{
    public static bool IsPointOnLine(Vector2 point, Vector2 a, Vector2 b, float tolerance = 0f)
    {
        return Math.Abs(Vector2.Distance(a, b) - (Vector2.Distance(a, point) + Vector2.Distance(point, b))) <= tolerance;
    }

    ///<inheritdoc cref="CalculateCircularMovement(Vector2, Vector2, Vector2, out List{List{Vector2}}, float, int, ValueTuple{float, float}?)"/>
    public static List<Vector3> CalculateCircularMovement(Vector3 centerPoint, Vector3 initialPoint, Vector3 exitPoint, out List<List<Vector3>> candidates, float precision = 36f, int exitPointTolerance = 1, (float Min, float Max)? clampRadius = null)
    {
        var ret = CalculateCircularMovement(centerPoint.ToVector2(), initialPoint.ToVector2(), exitPoint.ToVector2(), out var cand, precision, exitPointTolerance, clampRadius);
        candidates = cand.Select(x => x.Select(s => s.ToVector3(initialPoint.Y)).ToList()).ToList();
        return ret.Select(s => s.ToVector3(initialPoint.Y)).ToList();
    }

    /// <summary>
    /// Calculates perpendicular distance drawn from <paramref name="point"/> towards infinite line defined by (<paramref name="lineA"/>, <paramref name="lineB"/>)
    /// </summary>
    /// <param name="point"></param>
    /// <param name="lineA"></param>
    /// <param name="lineB"></param>
    /// <returns></returns>
    public static Vector2 FindClosestPointOnLine(Vector2 point, Vector2 lineA, Vector2 lineB)
    {
        var D = Vector2.Normalize(lineB - lineA);
        var d = Vector2.Dot(point - lineA, D);
        return lineA + Vector2.Multiply(D, d);
    }

    /// <summary>
    /// Tests whether perpendicular drawn from <paramref name="point"/> towards line will intersect line segment (<paramref name="lineA"/>, <paramref name="lineB"/>)
    /// </summary>
    /// <param name="point"></param>
    /// <param name="lineA"></param>
    /// <param name="lineB"></param>
    /// <returns></returns>
    public static bool IsPointPerpendicularToLineSegment(Vector2 point, Vector2 lineA, Vector2 lineB)
    {
        var ac = point - lineA;
        var ab = lineB - lineA;
        return (Vector2.Dot(ac, ab) >= 0 && Vector2.Dot(ac, ab) <= Vector2.Dot(ab, ab));
    }

    /// <summary>
    /// Calculates circular path around specified point required to arrive towards a specified point.
    /// </summary>
    /// <param name="centerPoint">Center point around which movement will be calculated</param>
    /// <param name="initialPoint">Starting point</param>
    /// <param name="exitPoint">Point towards which you aim to move after completing circulat move. Must be greater than the radius from <paramref name="centerPoint"/> to <paramref name="initialPoint"/>.</param>
    /// <param name="candidates">List of all pathes that were tested, for debugging.</param>
    /// <param name="precision">How much points on circle to generate</param>
    /// <param name="exitPointTolerance">How much points that are closest to exit point to consider to be valid stopping points</param>
    /// <param name="clampRadius">If set, radius of circular path will be restricted to these values inclusively.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static List<Vector2> CalculateCircularMovement(Vector2 centerPoint, Vector2 initialPoint, Vector2 exitPoint, out List<List<Vector2>> candidates, float precision = 36f, int exitPointTolerance = 1, (float Min, float Max)? clampRadius = null)
    {
        var step = 360f / precision;
        List<Vector2> points = [];
        var distance = Vector2.Distance(centerPoint, initialPoint);
        //if(clampRadius != null) distance.ValidateRange(clampRadius.Value.Min, clampRadius.Value.Max);
        for(var x = 0f; x < 360f; x += step)
        {
            var p = MathF.SinCos(x.DegToRad());
            points.Add(new Vector2(p.Sin * distance, p.Cos * distance) + centerPoint);
        }
        var closestPoints = points.OrderBy(x => Vector2.Distance(initialPoint, x)).Take(2).ToList();
        List<List<Vector2>> retCandidates = [];
        var finalPoints = points.OrderBy(x => Vector2.Distance(exitPoint, x)).Take(exitPointTolerance).ToArray();
        if(finalPoints.Length > 1)
        {
            for(var i = 0; i < finalPoints.Length - 1; i++)
            {
                if(IsPointPerpendicularToLineSegment(initialPoint, finalPoints[i], finalPoints[i + 1]) && Vector2.Distance(initialPoint, FindClosestPointOnLine(initialPoint, finalPoints[i], finalPoints[i + 1])) < distance / 2f)
                {
                    candidates = retCandidates;
                    return [];
                }
            }
        }
        foreach(var finalPoint in finalPoints)
        {
            foreach(var point in closestPoints)
            {
                void Process(int mod)
                {
                    var pointIndex = points.IndexOf(point);
                    if(pointIndex == -1) throw new Exception($"Could not find {point} in \n{points.Print("\n")}");
                    var list = new List<Vector2>();
                    var iterations = 0;
                    do
                    {
                        iterations++;
                        if(iterations > 1000) throw new Exception("Iteration limit exceeded");
                        list.Add(points.CircularSelect(pointIndex));
                        pointIndex += mod;
                    }
                    while(list[^1] != finalPoint);
                    retCandidates.Add(list);
                }
                Process(1);
                Process(-1);
            }
        }
        retCandidates = [.. retCandidates.OrderBy(CalculateDistance)];
        if(clampRadius != null)
        {
            foreach(var list in retCandidates)
            {
                for(var i = 0; i < list.Count; i++)
                {
                    if(GetAngleBetweenLines(list[i], centerPoint, initialPoint, centerPoint).RadToDeg() > step / 10)
                    {
                        if(Vector2.Distance(list[i], centerPoint) > clampRadius.Value.Max)
                        {
                            list[i] = MovePoint(centerPoint, list[i], clampRadius.Value.Max);
                        }
                        if(Vector2.Distance(list[i], centerPoint) < clampRadius.Value.Min)
                        {
                            list[i] = MovePoint(centerPoint, list[i], clampRadius.Value.Min);
                        }
                    }
                }
            }
        }
        candidates = retCandidates;
        return retCandidates.First();
    }

    /// <summary>
    /// Gets angle between two lines.
    /// </summary>
    /// <param name="a1"></param>
    /// <param name="a2"></param>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    /// <returns>Radians</returns>
    public static float GetAngleBetweenLines(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        return MathF.Acos(((a2.X - a1.X) * (b2.X - b1.X) + (a2.Y - a1.Y) * (b2.Y - b1.Y)) /
                (MathF.Sqrt(Square(a2.X - a1.X) + Square(a2.Y - a1.Y)) * MathF.Sqrt(Square(b2.X - b1.X) + Square(b2.Y - b1.Y))));
    }

    /// <summary>
    /// Returns squared value of a number.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static float Square(float x)
    {
        return x * x;
    }

    /// <summary>
    /// Given points A and B and a distance, returns a second point that origins from A, directs towards B and has a specified distance.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static Vector2 MovePoint(Vector2 a, Vector2 b, float distance)
    {
        return a + Vector2.Normalize(b - a) * distance;
    }

    /// <summary>
    /// Calculates angle between two points.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>Radians</returns>
    public static float GetAngleBetweenPoints(Vector2 a, Vector2 b)
    {
        return MathF.Atan2(b.Y - a.Y, b.X - a.X);
    }

    /// <summary>
    /// Gets a second point given initial point, angle and distance.
    /// </summary>
    /// <param name="initialPoint"></param>
    /// <param name="angle">Radians</param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static Vector2 GetPointFromAngleAndDistance(Vector2 initialPoint, float angle, float distance)
    {
        return new(initialPoint.X + distance * MathF.Sin(angle), initialPoint.Y + distance * MathF.Cos(angle));
    }

    /// <summary>Calculates the positive remainder when dividing a dividend by a divisor.</summary>
    public static double Mod(double dividend, double divisor)
    {
        var remainder = dividend % divisor;
        if(remainder < 0)
        {
            if(divisor < 0)
            {
                return remainder - divisor;
            }
            return remainder + divisor;
        }
        return remainder;
    }

    /// <summary>Calculates the positive remainder when dividing a dividend by a divisor.</summary>
    public static float Mod(float dividend, float divisor)
    {
        var remainder = dividend % divisor;
        if(remainder < 0)
        {
            if(divisor < 0)
            {
                return remainder - divisor;
            }
            return remainder + divisor;
        }
        return remainder;
    }

    /// <summary>Calculates the positive remainder when dividing a dividend by a divisor.</summary>
    public static int Mod(int dividend, int divisor)
    {
        var remainder = dividend % divisor;
        if(remainder < 0)
        {
            if(divisor < 0)
            {
                return remainder - divisor;
            }
            return remainder + divisor;
        }
        return remainder;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="angle">Radians</param>
    /// <param name="p"></param>
    /// <returns></returns>
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

    public static Vector2 ToVector2(this int i) => new(i);
    public static Vector2 ToVector2(this float i) => new(i);
    public static Vector2 ToVector2(this System.Drawing.Point p) => new(p.X, p.Y);
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

    public static Vector3 ToVector3(this (float X, float Y, float Z) t) => new(t.X, t.Y, t.Z);

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
