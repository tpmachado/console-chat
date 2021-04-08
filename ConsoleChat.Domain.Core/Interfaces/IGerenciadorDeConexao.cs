using ConsoleChat.Domain.Core.Models;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace ConsoleChat.Domain.Core.Interfaces
{
    public interface IGerenciadorDeConexao
    {
        bool AdicionarConexao(Conexao conexao);
        bool RemoverConexao(Conexao conexao);
        bool RemoverConexaoPorApelido(string nome);
        bool RemoverConexaoPorWebSocket(WebSocket socket);
        Conexao ObterConexaoPorApelido(string nome);
        Conexao ObterConexaoPorWebSocket(WebSocket socket);
        IList<Conexao> ObterConexoes();
    }
}
