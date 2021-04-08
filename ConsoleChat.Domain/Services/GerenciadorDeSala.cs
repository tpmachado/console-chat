using ConsoleChat.Domain.Core.Interfaces;
using ConsoleChat.Domain.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleChat.Domain.Services
{
    public class GerenciadorDeSala : IGerenciadorDeSala
    {
        private ConcurrentBag<Sala> _salas = new ConcurrentBag<Sala>();

        public IList<Sala> ObterSalas()
        {
            return _salas.ToList();
        }

        public Sala ObterSalaPorNome(string nomeDaSala)
        {
            return _salas.Where(_ => _.Nome == nomeDaSala)
                .FirstOrDefault();
        }

        public Sala ObterSalaPorConexao(Conexao conexao)
        {
            return _salas.Where(_ =>
                    _.Conexoes.Contains(conexao))
                .FirstOrDefault();
        }

        public bool AdicionarSala(string nomeDaSala)
        {
            var sala = ObterSalaPorNome(nomeDaSala);
            if (sala != null)
                return false;

            _salas.Add(new Sala(nomeDaSala));
            return true;
        }

        public bool EntrarNaSala(string nomeDaSala, Conexao conexao)
        {
            var sala = ObterSalaPorNome(nomeDaSala);
            if (sala == null)
                return false;

            SairDaSalaAtual(conexao);

            return sala.AdicionarConexao(conexao);
        }

        public bool SairDaSalaAtual(Conexao conexao)
        {
            var salaAtual = _salas.Where(_ =>
                    _.Conexoes.Contains(conexao))
                .FirstOrDefault();

            if (salaAtual == null)
                return false;

            return salaAtual.RemoverConexao(conexao);
        }
    }
}
