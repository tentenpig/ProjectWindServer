using ProjectWindServer.DB;

namespace ProjectWindServer.Game;

/// <summary>
/// 체크포인트 주기 저장 매니저
/// 일정 주기마다 dirty 상태인 세션들을 일괄 저장
/// 특정 상황에서는 강제 저장 가능
/// </summary>
public class CheckpointManager
{
    private readonly GameManager _gameManager;
    private readonly AccountRepository _accountRepo;
    private readonly TimeSpan _interval;
    private readonly CancellationTokenSource _cts = new();

    public CheckpointManager(GameManager gameManager, AccountRepository accountRepo, TimeSpan interval)
    {
        _gameManager = gameManager;
        _accountRepo = accountRepo;
        _interval = interval;
    }

    /// <summary>
    /// 주기적 체크포인트 루프 시작
    /// </summary>
    public async Task StartAsync()
    {
        Console.WriteLine($"[Checkpoint] Started. Interval: {_interval.TotalSeconds}s");

        while (!_cts.IsCancellationRequested)
        {
            await Task.Delay(_interval, _cts.Token).ConfigureAwait(false);
            await FlushAllAsync();
        }
    }

    /// <summary>
    /// 모든 dirty 세션 일괄 저장
    /// </summary>
    public async Task FlushAllAsync()
    {
        var dirtySessions = _gameManager.GetAllSessions().Where(s => s.IsDirty).ToList();
        if (dirtySessions.Count == 0) return;

        Console.WriteLine($"[Checkpoint] Saving {dirtySessions.Count} player(s)...");

        var tasks = dirtySessions.Select(async session =>
        {
            await _accountRepo.SavePositionAsync(
                session.AccountId, session.MapId,
                session.Position.X, session.Position.Y);
            session.ClearDirty();
        });

        await Task.WhenAll(tasks);
        Console.WriteLine("[Checkpoint] Done.");
    }

    /// <summary>
    /// 특정 플레이어 강제 저장
    /// </summary>
    public async Task ForceSaveAsync(PlayerSession session)
    {
        await _accountRepo.SavePositionAsync(
            session.AccountId, session.MapId,
            session.Position.X, session.Position.Y);
        session.ClearDirty();
        Console.WriteLine($"[Checkpoint] Force saved player {session.PlayerId}");
    }

    public void Stop() => _cts.Cancel();
}
