using ConsoleChat.Domain.Core.Models;
using System.Collections.Generic;

namespace ConsoleChat.Domain.Core.Interfaces
{
    public interface IGerenciadorDeSala
    {
        IList<Sala> ObterSalas();
        Sala ObterSalaPorNome(string nomeDaSala);
        Sala ObterSalaPorConexao(Conexao conexao);
        bool AdicionarSala(string nomeDaSala);
        bool EntrarNaSala(string nomeDaSala, Conexao conexao);
        bool SairDaSalaAtual(Conexao conexao);
    }
}
