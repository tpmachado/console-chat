using ConsoleChat.Domain.Core.Models;
using ConsoleChat.Domain.Services;
using System.Net.WebSockets;
using Xunit;

namespace ConsoleChat.Domain.Tests.Services
{
    public class GerenciadorDeSalaTests
    {
        private readonly GerenciadorDeSala _gerenciadorDeSala;

        public GerenciadorDeSalaTests()
        {
            _gerenciadorDeSala = new GerenciadorDeSala();
        }

        [Theory]
        [InlineData("Principal")]
        public void DeveAdicionarUmaSala(string nomeDaSala)
        {
            Assert.True(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
        }

        [Theory]
        [InlineData("Principal")]
        public void NaoDeveAdicionarUmaSalaComMesmoNome(string nomeDaSala)
        {
            Assert.True(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
            Assert.False(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
        }

        [Theory]
        [InlineData("Principal")]
        public void DeveEntrarNaSala(string nomeDaSala)
        {
            Assert.True(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
            Assert.True(_gerenciadorDeSala.EntrarNaSala(nomeDaSala, CriarConexao()));
        }

        [Theory]
        [InlineData("Principal")]
        public void NaoDeveEntrarEmUmaSalaInexistente(string nomeDaSala)
        {
            Assert.True(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
            Assert.False(_gerenciadorDeSala.EntrarNaSala("General", CriarConexao()));
        }

        [Theory]
        [InlineData("Principal")]
        public void DeveSairDaSalaAtual(string nomeDaSala)
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
            Assert.True(_gerenciadorDeSala.EntrarNaSala(nomeDaSala, conexao));
            Assert.True(_gerenciadorDeSala.SairDaSalaAtual(conexao));
        }

        [Theory]
        [InlineData("Principal")]
        public void DeveObterSalaPorNome(string nomeDaSala)
        {
            Assert.True(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
            var sala = _gerenciadorDeSala.ObterSalaPorNome(nomeDaSala);
            Assert.True(sala != null);
        }

        [Theory]
        [InlineData("Principal")]
        public void NaoDeveObterSalaPorNome(string nomeDaSala)
        {
            Assert.True(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
            var sala = _gerenciadorDeSala.ObterSalaPorNome("General");
            Assert.True(sala == null);
        }

        [Theory]
        [InlineData("Principal")]
        public void DeveObterSalaPorConexao(string nomeDaSala)
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
            Assert.True(_gerenciadorDeSala.EntrarNaSala(nomeDaSala, conexao));
            var sala = _gerenciadorDeSala.ObterSalaPorConexao(conexao);
            Assert.True(sala != null);
        }

        [Theory]
        [InlineData("Principal")]
        public void NaoDeveObterSalaPorConexao(string nomeDaSala)
        {
            Assert.True(_gerenciadorDeSala.AdicionarSala(nomeDaSala));
            Assert.True(_gerenciadorDeSala.EntrarNaSala(nomeDaSala, CriarConexao()));
            var sala = _gerenciadorDeSala.ObterSalaPorConexao(CriarConexao());
            Assert.True(sala == null);
        }

        private Conexao CriarConexao()
        {
            var conexao = new Conexao("Apelido", new ClientWebSocket());
            return conexao;
        }
    }
}
