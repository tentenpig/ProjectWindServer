using System;

namespace Shared.Models;

/// <summary>
/// 2D 정수 좌표 (Unity int2 대체, 공유용)
/// </summary>
public struct Vec2Int
{
    public int X { get; set; }
    public int Y { get; set; }

    public Vec2Int(int x, int y) { X = x; Y = y; }

    public static Vec2Int operator +(Vec2Int a, Vec2Int b) => new(a.X + b.X, a.Y + b.Y);
    public static bool operator ==(Vec2Int a, Vec2Int b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vec2Int a, Vec2Int b) => !(a == b);
    public override bool Equals(object? obj) => obj is Vec2Int v && this == v;
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public override string ToString() => $"({X}, {Y})";
}
