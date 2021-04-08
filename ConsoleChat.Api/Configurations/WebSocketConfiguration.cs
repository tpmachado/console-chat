using ConsoleChat.Api.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace ConsoleChat.Api.Configurations
{
    public static class WebSocketConfiguration
    {
        public static IApplicationBuilder UseWebSocketMiddleware(this IApplicationBuilder app)
        {
            return app.Map("/ws", (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>());
        }
    }
}
