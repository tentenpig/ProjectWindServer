using Shared.Models;

namespace ProjectWindServer.Game;

/// <summary>
/// 접속한 플레이어 한 명의 세션 정보
/// </summary>
public class PlayerSession
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = "";
    public string MapId { get; set; } = "town_01";
    public Vec2Int Position { get; set; }

    // TODO: 네트워크 연결 참조 추가
}
