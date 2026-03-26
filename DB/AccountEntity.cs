namespace ProjectWindServer.DB;

public class AccountEntity
{
    public int Id { get; set; }
    public string AccountName { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public int Level { get; set; } = 1;
    public string MapId { get; set; } = "town_01";
    public int PosX { get; set; }
    public int PosY { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
}
