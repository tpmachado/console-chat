using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleChat.Domain.Core.Models
{
    public class Sala
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public List<Conexao> Conexoes { get; private set; }

        public Sala(string nome)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Conexoes = new List<Conexao>();
        }

        public bool AdicionarConexao(Conexao conexao)
        {
            if (Conexoes.Any(_ => _ == conexao))
                return false;

            Conexoes.Add(conexao);
            return true;
        }

        public bool RemoverConexao(Conexao conexao)
        {
            if (!Conexoes.Any(_ => _ == conexao))
                return false;

            return Conexoes.Remove(conexao);
        }
    }
}
