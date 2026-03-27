using System.Net;
using System.Net.Sockets;

namespace ProjectWindServer.Net;

/// <summary>
/// TCP 서버 리스너
/// 클라이언트 접속을 받아 ClientSession을 생성
/// </summary>
public class GameServer
{
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cts = new();
    private int _nextSessionId = 1;

    public event Action<ClientSession>? OnClientConnected;

    public GameServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine($"[GameServer] Listening on port {((IPEndPoint)_listener.LocalEndpoint).Port}...");

        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(_cts.Token);
                var session = new ClientSession(_nextSessionId++, tcpClient);

                Console.WriteLine($"[GameServer] Client connected: Session {session.SessionId} from {tcpClient.Client.RemoteEndPoint}");
                OnClientConnected?.Invoke(session);

                _ = session.StartReceiveAsync();
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _listener.Stop();
            Console.WriteLine("[GameServer] Stopped.");
        }
    }

    public void Stop()
    {
        _cts.Cancel();
    }
}
