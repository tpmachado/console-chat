using ConsoleChat.Domain.Core.Models;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace ConsoleChat.Domain.Core.Interfaces
{
    public interface IGerenciadorDeChat
    {
        Task Conectar(string nome, WebSocket Socket);
        Task Desconectar(WebSocket Socket);
        Task ProcessarMensagem(WebSocket socket, string mensagem);
    }
}
