using ConsoleChat.Domain.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleChat.Api.Middlewares
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private IGerenciadorDeChat _gerenciadorDeChat { get; set; }

        public WebSocketManagerMiddleware
        (
            RequestDelegate next,
            IGerenciadorDeChat gerenciadorDeChat
        )
        {
            _next = next;
            _gerenciadorDeChat = gerenciadorDeChat;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            var apelido = (string)context.Request.Query["apelido"];

            if (string.IsNullOrEmpty(apelido))
                return;

            await _gerenciadorDeChat.Conectar(apelido, socket);
            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string mensagem = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await _gerenciadorDeChat.ProcessarMensagem(socket, mensagem);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                    await _gerenciadorDeChat.Desconectar(socket);
            });

            await _next(context);
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}
