namespace Shared.Models;

/// <summary>
/// 맵 설정 데이터 (클라이언트-서버 공유)
/// </summary>
public class MapData
{
    public string Id { get; set; } = "town_01";
    public string Name { get; set; } = "마을";
    public int StartX { get; set; } = 10;
    public int StartY { get; set; } = 7;
    public int Width { get; set; }
    public int Height { get; set; }

    /// <summary>
    /// 충돌 그리드 (true = 이동 불가)
    /// [y * Width + x] 형태의 1차원 배열
    /// </summary>
    public bool[]? CollisionGrid { get; set; }

    public bool IsBlocked(int x, int y)
    {
        if (CollisionGrid == null) return false;
        if (x < 0 || x >= Width || y < 0 || y >= Height) return true;
        return CollisionGrid[y * Width + x];
    }
}
