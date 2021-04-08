using ConsoleChat.Domain.Core.Models;
using ConsoleChat.Domain.Services;
using System.Net.WebSockets;
using Xunit;

namespace ConsoleChat.Domain.Tests.Services
{
    public class GerenciadorDeConexaoTests
    {
        private readonly GerenciadorDeConexao _gerenciadorDeConexao;

        public GerenciadorDeConexaoTests()
        {
            _gerenciadorDeConexao = new GerenciadorDeConexao();
        }

        [Fact]
        public void DeveAdicionarConexao()
        {
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(CriarConexao()));
        }

        [Fact]
        public void NaoDeveAdicionarConexaoComMesmoApelido()
        {
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(CriarConexao()));
            Assert.False(_gerenciadorDeConexao.AdicionarConexao(CriarConexao()));
        }

        [Fact]
        public void DeveRemoverConexao()
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(conexao));
            Assert.True(_gerenciadorDeConexao.RemoverConexao(conexao));
        }

        [Fact]
        public void NaoDeveRemoverConexaoInexistente()
        {
            Assert.False(_gerenciadorDeConexao.RemoverConexao(CriarConexao()));
        }

        [Fact]
        public void DeveRemoverConexaoPorApelido()
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(conexao));
            Assert.True(_gerenciadorDeConexao.RemoverConexaoPorApelido(conexao.Apelido));
        }

        [Fact]
        public void NaoDeveRemoverConexaoPorApelidoInexistente()
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(conexao));
            Assert.False(_gerenciadorDeConexao.RemoverConexaoPorApelido("Apelido 1"));
        }

        [Fact]
        public void DeveRemoverConexaoPorWebSocket()
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(conexao));
            Assert.True(_gerenciadorDeConexao.RemoverConexaoPorWebSocket(conexao.Socket));
        }

        [Fact]
        public void NaoDeveRemoverConexaoPorWebSocketInexistente()
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(conexao));
            Assert.False(_gerenciadorDeConexao.RemoverConexaoPorWebSocket(new ClientWebSocket()));
        }

        [Fact]
        public void DeveObterConexaoPorApelido()
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(conexao));

            var conexaoObtida = _gerenciadorDeConexao.ObterConexaoPorApelido(conexao.Apelido);
            Assert.True(conexaoObtida != null);
        }

        [Fact]
        public void NaoDeveObterConexaoPorApelidoInexistente()
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(conexao));

            var conexaoObtida = _gerenciadorDeConexao.ObterConexaoPorApelido("Apelido 1");
            Assert.True(conexaoObtida == null);
        }

        [Fact]
        public void DeveObterConexaoPorWebSocket()
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(conexao));

            var conexaoObtida = _gerenciadorDeConexao.ObterConexaoPorWebSocket(conexao.Socket);
            Assert.True(conexaoObtida != null);
        }

        [Fact]
        public void NaoDeveObterConexaoPorWebSocketInexistente()
        {
            var conexao = CriarConexao();
            Assert.True(_gerenciadorDeConexao.AdicionarConexao(conexao));

            var conexaoObtida = _gerenciadorDeConexao.ObterConexaoPorWebSocket(new ClientWebSocket());
            Assert.True(conexaoObtida == null);
        }

        private Conexao CriarConexao()
        {
            var conexao = new Conexao("Apelido", new ClientWebSocket());
            return conexao;
        }
    }
}
