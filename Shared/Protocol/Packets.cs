using Shared.Models;

namespace Shared.Protocol;

// ─────────────────────────────────────
// 클라이언트 → 서버
// ─────────────────────────────────────

public class C_Login
{
    public string AccountName { get; set; } = "";
}

public class C_Move
{
    public Vec2Int Position { get; set; }
    public Vec2Int Direction { get; set; }
}

// ─────────────────────────────────────
// 서버 → 클라이언트
// ─────────────────────────────────────

public class S_Login
{
    public bool Success { get; set; }
    public int PlayerId { get; set; }
    public string MapId { get; set; } = "";
    public Vec2Int Position { get; set; }
}

public class S_Move
{
    public int PlayerId { get; set; }
    public Vec2Int Position { get; set; }
    public Vec2Int Direction { get; set; }
}

public class S_Spawn
{
    public int PlayerId { get; set; }
    public string Name { get; set; } = "";
    public string MapId { get; set; } = "";
    public Vec2Int Position { get; set; }
}

public class S_Despawn
{
    public int PlayerId { get; set; }
}
