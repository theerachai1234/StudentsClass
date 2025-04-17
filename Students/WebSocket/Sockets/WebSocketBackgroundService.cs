using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocket.Sockets
{
    public class WebSocketBackgroundService : BackgroundService
    {
        private readonly ConcurrentBag<WebSocket> _webSockets;

        public WebSocketBackgroundService()
        {
            _webSockets = new ConcurrentBag<WebSocket>();
        }

        // ฟังก์ชันหลักที่ใช้ในการรับ WebSocket Connection
        public async Task AcceptWebSocketAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _webSockets.Add(webSocket);  // เก็บ WebSocket connection ที่เชื่อมต่อมา

                var buffer = new byte[1024];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        _webSockets.TryTake(out _);  // ลบ WebSocket ที่ปิดการเชื่อมต่อแล้ว
                        break;
                    }

                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received: {receivedMessage}");

                    // ส่งข้อความกลับไปยัง client
                    var echoMessage = $"Echo: {receivedMessage}";
                    var responseBytes = Encoding.UTF8.GetBytes(echoMessage);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // ตรวจสอบ WebSocket connections ทุกๆ 10 วินาที
                foreach (var webSocket in _webSockets)
                {
                    if (webSocket.State == WebSocketState.Open)
                    {
                        // ส่งข้อความไปยังทุกๆ client ที่เชื่อมต่อ
                        var message = "Ping: " + DateTime.Now.ToString("HH:mm:ss");
                        var buffer = Encoding.UTF8.GetBytes(message);
                        await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                await Task.Delay(10000);  // ส่งข้อมูลทุกๆ 10 วินาที
            }
        }
    }
}
