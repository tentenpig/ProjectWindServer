using System.Data;
using MySqlConnector;

namespace ProjectWindServer.DB;

public class AccountRepository
{
    private readonly GameDbContext _db;

    public AccountRepository(GameDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// 계정 조회 또는 생성 (sp_get_or_create_account)
    /// </summary>
    public async Task<AccountEntity> GetOrCreateAccountAsync(string accountName, string passwordHash)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_get_or_create_account";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("p_account_name", accountName);
        cmd.Parameters.AddWithValue("p_password_hash", passwordHash);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new AccountEntity
        {
            Id = reader.GetInt32("id"),
            AccountName = reader.GetString("account_name"),
            PasswordHash = reader.GetString("password_hash"),
            PlayerName = reader.GetString("player_name"),
            Level = reader.GetInt32("level"),
            MapId = reader.GetString("map_id"),
            PosX = reader.GetInt32("pos_x"),
            PosY = reader.GetInt32("pos_y"),
            CreatedAt = reader.GetDateTime("created_at"),
            LastLogin = reader.GetDateTime("last_login")
        };
    }

    /// <summary>
    /// 위치 저장 (sp_save_position)
    /// </summary>
    public async Task SavePositionAsync(int accountId, string mapId, int x, int y)
    {
        await using var conn = _db.CreateConnection();
        await conn.OpenAsync();

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_save_position";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("p_account_id", accountId);
        cmd.Parameters.AddWithValue("p_map_id", mapId);
        cmd.Parameters.AddWithValue("p_pos_x", x);
        cmd.Parameters.AddWithValue("p_pos_y", y);

        await cmd.ExecuteNonQueryAsync();
    }
}
