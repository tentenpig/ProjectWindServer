using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ProjectWindServer.Net;

/// <summary>
/// 클라이언트 하나의 TCP 연결을 관리
/// [2byte 패킷타입][2byte 길이][JSON 페이로드] 형식
/// </summary>
public class ClientSession
{
    public int SessionId { get; }
    public TcpClient TcpClient { get; }
    public bool IsConnected => TcpClient.Connected;

    private readonly NetworkStream _stream;
    private readonly CancellationTokenSource _cts = new();

    public event Action<ClientSession, ushort, byte[]>? OnPacketReceived;
    public event Action<ClientSession>? OnDisconnected;

    public ClientSession(int sessionId, TcpClient tcpClient)
    {
        SessionId = sessionId;
        TcpClient = tcpClient;
        _stream = tcpClient.GetStream();
    }

    public async Task StartReceiveAsync()
    {
        var headerBuf = new byte[4]; // 2byte type + 2byte length
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                // 헤더 읽기
                if (!await ReadExactAsync(headerBuf, 4))
                    break;

                ushort packetType = BitConverter.ToUInt16(headerBuf, 0);
                ushort length = BitConverter.ToUInt16(headerBuf, 2);

                // 페이로드 읽기
                var payload = new byte[length];
                if (length > 0 && !await ReadExactAsync(payload, length))
                    break;

                OnPacketReceived?.Invoke(this, packetType, payload);
            }
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException)
        {
            // 연결 끊김
        }
        finally
        {
            Disconnect();
        }
    }

    public void Send<T>(ushort packetType, T packet)
    {
        try
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(packet);
            var header = new byte[4];
            BitConverter.GetBytes(packetType).CopyTo(header, 0);
            BitConverter.GetBytes((ushort)json.Length).CopyTo(header, 2);

            lock (_stream)
            {
                _stream.Write(header);
                _stream.Write(json);
            }
        }
        catch (Exception ex) when (ex is IOException or ObjectDisposedException)
        {
            Disconnect();
        }
    }

    public void Disconnect()
    {
        _cts.Cancel();
        try { TcpClient.Close(); } catch { }
        OnDisconnected?.Invoke(this);
    }

    private async Task<bool> ReadExactAsync(byte[] buffer, int count)
    {
        int offset = 0;
        while (offset < count)
        {
            int read = await _stream.ReadAsync(buffer.AsMemory(offset, count - offset), _cts.Token);
            if (read == 0) return false;
            offset += read;
        }
        return true;
    }
}
