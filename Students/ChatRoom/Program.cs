
using ChatRoom.Sockets;

namespace ChatRoom
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<WebSocketBackgroundService>();
            builder.Services.AddHostedService(provider => provider.GetRequiredService<WebSocketBackgroundService>());


            var app = builder.Build();

            app.UseWebSockets();

            app.Map("/ws", (HttpContext context) =>
            {
                // �Ѻ WebSocket connection �ҡ client
                var backgroundService = context.RequestServices.GetRequiredService<WebSocketBackgroundService>();
                return backgroundService.AcceptWebSocketAsync(context);  // ���¡��ѧ��ѹ���Ѵ��� WebSocket connection
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.Run();
        }
    }
}
