using MySqlConnector;

namespace ProjectWindServer.DB;

/// <summary>
/// MySQL 연결 관리
/// 모든 DB 작업은 stored procedure 호출로만 수행
/// </summary>
public class GameDbContext
{
    private readonly string _connectionString;

    public GameDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public MySqlConnection CreateConnection() => new(_connectionString);
}
