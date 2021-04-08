using ConsoleChat.Domain.Core.Interfaces;
using ConsoleChat.Domain.Core.Models;
using ConsoleChat.Domain.Services;
using Moq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Xunit;

namespace ConsoleChat.Domain.Tests.Services
{
    public class GerenciadorDeChatTests
    {
        public readonly Mock<IGerenciadorDeConexao> _gerenciadorDeConexaoMock;
        public readonly Mock<IGerenciadorDeSala> _gerenciadorDeSalaMock;

        public readonly GerenciadorDeChat _gerenciadorDeChat;

        public GerenciadorDeChatTests()
        {
            _gerenciadorDeConexaoMock = new Mock<IGerenciadorDeConexao>();
            _gerenciadorDeSalaMock = new Mock<IGerenciadorDeSala>();

            _gerenciadorDeChat = 
                new GerenciadorDeChat(
                    _gerenciadorDeConexaoMock.Object,
                    _gerenciadorDeSalaMock.Object
                );

            SetupGerenciadorDeSala();
        }

        [Fact]
        public async Task DeveConectar()
        {
            var conexao = CriarConexao();
            await _gerenciadorDeChat.Conectar(conexao.Apelido, conexao.Socket);

            _gerenciadorDeConexaoMock.Verify(_ =>
                _.AdicionarConexao(It.Is<Conexao>(_ => 
                    _.Apelido == conexao.Apelido &&
                    _.Socket == conexao.Socket)),
                Times.Once);
        }

        [Fact]
        public async Task NaoDeveConectarComMesmoApelido()
        {
            var conexao = CriarConexao();
            await _gerenciadorDeChat.Conectar(conexao.Apelido, conexao.Socket);

            SetupGerenciadorDeConexao();

            await _gerenciadorDeChat.Conectar(conexao.Apelido, new ClientWebSocket());

            _gerenciadorDeConexaoMock.Verify(_ =>
                _.AdicionarConexao(It.Is<Conexao>(_ =>
                    _.Apelido == conexao.Apelido)),
                Times.Exactly(1));
        }

        [Fact]
        public async Task DeveDesconectar()
        {
            var conexao = CriarConexao();
            await _gerenciadorDeChat.Conectar(conexao.Apelido, conexao.Socket);

            SetupGerenciadorDeConexao();

            await _gerenciadorDeChat.Desconectar(conexao.Socket);

            _gerenciadorDeConexaoMock.Verify(_ =>
                _.RemoverConexao(It.IsAny<Conexao>()),
                Times.Once);
        }

        [Fact]
        public async Task NaoDeveDesconectarComConexaoInexistente()
        {
            var conexao = CriarConexao();
            await _gerenciadorDeChat.Conectar(conexao.Apelido, conexao.Socket);
            await _gerenciadorDeChat.Desconectar(conexao.Socket);

            _gerenciadorDeConexaoMock.Verify(_ =>
                _.RemoverConexao(It.IsAny<Conexao>()),
                Times.Never);
        }

        [Fact]
        public async Task DeveProcessarMensagem()
        {
            var conexao = CriarConexao();
            await _gerenciadorDeChat.Conectar(conexao.Apelido, conexao.Socket);

            SetupGerenciadorDeConexao();

            await _gerenciadorDeChat.ProcessarMensagem(conexao.Socket, "Mensagem");

            _gerenciadorDeSalaMock.Verify(_ =>
                _.ObterSalaPorNome(It.IsAny<string>()),
                Times.AtLeast(1));
        }

        [Fact]
        public async Task NaoDeveProcessarMensagemVazia()
        {
            var conexao = CriarConexao();
            await _gerenciadorDeChat.Conectar(conexao.Apelido, conexao.Socket);

            SetupGerenciadorDeConexao();

            await _gerenciadorDeChat.ProcessarMensagem(conexao.Socket, string.Empty);

            _gerenciadorDeSalaMock.Verify(_ =>
                _.ObterSalaPorNome(It.IsAny<string>()),
                Times.AtMost(1));
        }

        private Conexao CriarConexao()
        {
            var conexao = new Conexao("Apelido", new ClientWebSocket());
            return conexao;
        }

        private void SetupGerenciadorDeConexao()
        {
            var conexao = CriarConexao();
            _gerenciadorDeConexaoMock.Setup(_ =>
                _.ObterConexaoPorApelido(It.Is<string>(_ => _ == conexao.Apelido)))
                .Returns(conexao);

            _gerenciadorDeConexaoMock.Setup(_ =>
                _.ObterConexaoPorWebSocket(It.IsAny<WebSocket>()))
                .Returns(conexao);
        }

        private void SetupGerenciadorDeSala()
        {
            _gerenciadorDeSalaMock.Setup(_ => 
                _.ObterSalaPorConexao(It.IsAny<Conexao>()))
                    .Returns(new Sala("Principal"));

            _gerenciadorDeSalaMock.Setup(_ =>
                _.ObterSalaPorNome(It.IsAny<string>()))
                    .Returns(new Sala("Principal"));
        }
    }
}
