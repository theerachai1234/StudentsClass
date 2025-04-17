// Program.cs
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// เก็บ WebSocket connections ทั้งหมด
var webSockets = new ConcurrentBag<WebSocket>();

app.UseWebSockets();

app.Map("/", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        // เชื่อมต่อ WebSocket
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        // เก็บ WebSocket ที่เชื่อมต่อ
        webSockets.Add(webSocket);

        var buffer = new byte[1024 * 4];

        while (true)
        {
            // รับข้อความจาก client
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                // เมื่อปิดการเชื่อมต่อ
                webSockets.TryTake(out _); // ลบ connection
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                break;
            }

            var receivedText = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"📥 From browser: {receivedText}");

            // ส่งข้อความไปยัง client ทั้งหมด
            var echo = $"✅ {receivedText}";
            var responseBytes = Encoding.UTF8.GetBytes(echo);

            // ส่งข้อความกลับไปที่ทุก ๆ client ที่เชื่อมต่อ
            foreach (var socket in webSockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
    else
    {
        context.Response.StatusCode = 400; // 400 Bad Request ถ้าไม่ใช่ WebSocket request
    }
});

async Task SendNumbersAsync()
{
    int number = 1;

    while (true)
    {
        var message = $"Number: {number++}";
        var responseBytes = Encoding.UTF8.GetBytes(message);

        // ส่งตัวเลขไปยังทุก ๆ client
        foreach (var socket in webSockets)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        await Task.Delay(10000); // หน่วงเวลา 10 วินาที
    }
}

_ = SendNumbersAsync();

app.Run("http://localhost:8888");
