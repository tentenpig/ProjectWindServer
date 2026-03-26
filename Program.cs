using ProjectWindServer.DB;
using ProjectWindServer.Game;
using ProjectWindServer.Net;
using Shared.Models;

const int PORT = 7777;
const string DB_CONNECTION = "Server=localhost;Port=3306;Database=project_wind;User=root;Password=1234;";

Console.WriteLine("=== ToyProject MMORPG Server ===");

// 맵 데이터 로드
var maps = new List<MapData>
{
    new()
    {
        Id = "town_01",
        Name = "마을",
        Width = 20,
        Height = 15,
        StartX = 10,
        StartY = 7,
        CollisionGrid = GenerateTestCollisionGrid(20, 15)
    }
};

var gameManager = new GameManager();
gameManager.LoadMaps(maps);

// DB 초기화
var dbContext = new GameDbContext(DB_CONNECTION);
var accountRepo = new AccountRepository(dbContext);

// 체크포인트 매니저 (2분 주기 저장)
var checkpointManager = new CheckpointManager(gameManager, accountRepo, TimeSpan.FromMinutes(2));

// 네트워크 서버 시작
var packetHandler = new PacketHandler(gameManager, accountRepo, checkpointManager);
var server = new GameServer(PORT);

server.OnClientConnected += session =>
{
    packetHandler.RegisterSession(session);
};

Console.WriteLine("[Server] Press Ctrl+C to stop.");

// Ctrl+C 처리: 전체 강제 저장 후 종료
Console.CancelKeyPress += async (_, e) =>
{
    e.Cancel = true;
    Console.WriteLine("[Server] Shutting down...");
    checkpointManager.Stop();
    await checkpointManager.FlushAllAsync();
    server.Stop();
};

// 체크포인트 루프를 백그라운드로 시작
_ = checkpointManager.StartAsync();

await server.StartAsync();

/// <summary>
/// 테스트용 충돌 그리드 생성 (외벽 + 내부 장애물)
/// 클라이언트 SetupTilemapScene.cs와 동일한 레이아웃
/// </summary>
static bool[] GenerateTestCollisionGrid(int w, int h)
{
    var grid = new bool[w * h];
    for (int y = 0; y < h; y++)
    {
        for (int x = 0; x < w; x++)
        {
            bool blocked = (x == 0 || x == w - 1 || y == 0 || y == h - 1);
            if (x >= 5 && x <= 7 && y == 5) blocked = true;
            if (x == 12 && y >= 3 && y <= 8) blocked = true;
            if (x >= 14 && x <= 16 && y == 11) blocked = true;
            grid[y * w + x] = blocked;
        }
    }
    return grid;
}
