using System;
using System.Net.WebSockets;

namespace ConsoleChat.Domain.Core.Models
{
    public class Conexao
    {
        public Guid Id { get; private set; }
        public string Apelido { get; private set; }
        public WebSocket Socket { get; private set; }

        public Conexao(string nome, WebSocket socket)
        {
            Id = Guid.NewGuid();
            Apelido = nome;
            Socket = socket;
        }
    }
}
