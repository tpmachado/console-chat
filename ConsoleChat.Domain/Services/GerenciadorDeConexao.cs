using ConsoleChat.Domain.Core.Interfaces;
using ConsoleChat.Domain.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace ConsoleChat.Domain.Services
{
    public class GerenciadorDeConexao : IGerenciadorDeConexao
    {
        private ConcurrentDictionary<string, Conexao> _conexoes = new ConcurrentDictionary<string, Conexao>();

        public bool AdicionarConexao(Conexao conexao)
        {
            return _conexoes.TryAdd(conexao.Apelido, conexao);
        }

        public bool RemoverConexao(Conexao conexao)
        {
            return _conexoes.TryRemove(conexao.Apelido, out _);
        }

        public bool RemoverConexaoPorApelido(string apelido)
        {
            var conexaoRemovida = ObterConexaoPorApelido(apelido);
            if (conexaoRemovida == null)
                return false;

            return RemoverConexao(conexaoRemovida);
        }

        public bool RemoverConexaoPorWebSocket(WebSocket socket)
        {
            var conexaoRemovida = ObterConexaoPorWebSocket(socket);
            if (conexaoRemovida == null)
                return false;

            return RemoverConexao(conexaoRemovida);
        }

        public Conexao ObterConexaoPorApelido(string nome)
        {
            return _conexoes.Where(_ => _.Value.Apelido == nome)
                .FirstOrDefault()
                .Value;
        }

        public Conexao ObterConexaoPorWebSocket(WebSocket socket)
        {
            return _conexoes.Where(_ => _.Value.Socket == socket)
                .FirstOrDefault()
                .Value;
        }

        public IList<Conexao> ObterConexoes()
        {
            return _conexoes.Select(_ => _.Value).ToList();
        }
    }
}
