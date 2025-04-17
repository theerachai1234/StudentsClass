using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace ChatRoom.Sockets
{
    public class WebSocketBackgroundService : BackgroundService
    {
        private readonly ConcurrentBag<WebSocket> _webSockets;

        public WebSocketBackgroundService()
        {
            _webSockets = new ConcurrentBag<WebSocket>();
        }

        public async Task AcceptWebSocketAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _webSockets.Add(webSocket);  // เก็บ WebSocket connection ที่เชื่อมต่อมา

                var startMessage = "Start Chat Room";
                var receiveBuffer = Encoding.UTF8.GetBytes(startMessage);
                await webSocket.SendAsync(new ArraySegment<byte>(receiveBuffer), WebSocketMessageType.Text, true, CancellationToken.None);



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

                    // ส่งข้อความไปยังทุกๆ client ที่เชื่อมต่อ
                    foreach (var socket in _webSockets)
                    {
                        if (socket.State == WebSocketState.Open)
                        {
                            var echoMessage = receivedMessage;
                            var responseBytes = Encoding.UTF8.GetBytes(echoMessage);
                            await socket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // ฟังก์ชันนี้ทำงานเมื่อเชื่อมต่อสำเร็จ และส่งข้อความให้ client ทุกๆ 10 วินาที
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    foreach (var socket in _webSockets)
            //    {
            //        if (socket.State == WebSocketState.Open)
            //        {
            //            var message = "Ping: " + DateTime.Now.ToString("HH:mm:ss");
            //            var buffer = Encoding.UTF8.GetBytes(message);
            //            await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            //        }
            //    }
            //    await Task.Delay(10000);  // ส่งข้อมูลทุกๆ 10 วินาที
            //}
        }
    }
}
