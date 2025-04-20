using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace ChatRoom.Sockets
{
    public class WebSocketBackgroundService : BackgroundService
    {
        private readonly ConcurrentDictionary<Guid, WebSocket> _webSockets = new();

        public async Task AcceptWebSocketAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var id = Guid.NewGuid();
            _webSockets.TryAdd(id, webSocket);

            // ส่งข้อความเริ่มต้น
            var welcomeMessage = Encoding.UTF8.GetBytes("Start Chat Room");
            await webSocket.SendAsync(new ArraySegment<byte>(welcomeMessage), WebSocketMessageType.Text, true, CancellationToken.None);

            var buffer = new byte[1024];

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        break;
                    }

                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received: {receivedMessage}");

                    // ส่งข้อความไปยังทุก client ที่ยังเชื่อมต่ออยู่
                    foreach (var pair in _webSockets)
                    {
                        if (pair.Value.State == WebSocketState.Open)
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(receivedMessage);
                            await pair.Value.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                _webSockets.TryRemove(id, out _);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    foreach (var pair in _webSockets)
            //    {
            //        var socket = pair.Value;
            //        if (socket.State == WebSocketState.Open)
            //        {
            //            var ping = Encoding.UTF8.GetBytes("Ping: " + DateTime.Now.ToString("HH:mm:ss"));
            //            await socket.SendAsync(new ArraySegment<byte>(ping), WebSocketMessageType.Text, true, CancellationToken.None);
            //        }
            //    }

            //    await Task.Delay(10000, stoppingToken);
            //}
        }
    }
}
