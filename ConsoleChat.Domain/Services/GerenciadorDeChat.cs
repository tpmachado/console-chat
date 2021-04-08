using ConsoleChat.Domain.Core.Interfaces;
using ConsoleChat.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleChat.Domain.Services
{
    public class GerenciadorDeChat : IGerenciadorDeChat
    {
        private const string _salaPrincipal = "Principal";
        private readonly List<string> _comandos;

        private IGerenciadorDeConexao _gerenciadorDeConexao;
        private IGerenciadorDeSala _gerenciadorDeSala;

        public GerenciadorDeChat
        (
            IGerenciadorDeConexao gerenciadorDeConexao,
            IGerenciadorDeSala gerenciadorDeSala
        )
        {
            _gerenciadorDeConexao = gerenciadorDeConexao;
            _gerenciadorDeSala = gerenciadorDeSala;

            _gerenciadorDeSala.AdicionarSala(_salaPrincipal);
            _comandos = new List<string>
            {
                "/u", // Usuários
                "/s", // Salas
                "/s/c", // Criar sala
                "/s/e", // Entrar em uma sala
                "/p", // Mensagem publica para usuario
                "/pm", // Mensagem privada para usuario
                "/exit", // Sair
                "/help", // Ajuda
            };
        }

        public async Task Conectar(string apelido, WebSocket socket)
        {
            var conexaoExistente = _gerenciadorDeConexao.ObterConexaoPorApelido(apelido);
            if (conexaoExistente != null)
            {
                await EnviarMensagemDirecionada(socket, $"O apelido {apelido} ja está em uso, por favor escolha outro");
                return;
            }

            var conexao = new Conexao(apelido, socket);
            _gerenciadorDeConexao.AdicionarConexao(conexao);
            _gerenciadorDeSala.EntrarNaSala(_salaPrincipal, conexao);

            var sala = _gerenciadorDeSala.ObterSalaPorConexao(conexao);
            await EnviarMensagemParaTodos(sala.Nome, $"{conexao.Apelido} entrou na sala");
        }

        public async Task Desconectar(WebSocket socket)
        {
            var conexao = _gerenciadorDeConexao.ObterConexaoPorWebSocket(socket);
            if (conexao == null)
                return;

            await Sair(conexao);
        }

        public async Task ProcessarMensagem(WebSocket socket, string mensagem)
        {
            var conexao = _gerenciadorDeConexao.ObterConexaoPorWebSocket(socket);
            var sala = _gerenciadorDeSala.ObterSalaPorConexao(conexao);

            if (string.IsNullOrEmpty(mensagem) || sala == null)
            {
                await ComandoIncompleto(conexao);
                return;
            }

            var mensagemPorPartes = mensagem.Split(" ");
            if (!_comandos.Contains(mensagemPorPartes[0]))
                await EnviarMensagemParaTodos(sala.Nome, $"{conexao.Apelido} disse: {mensagem}");
            else
                await ProcessarComando(conexao, mensagemPorPartes);
        }

        private async Task ProcessarComando(Conexao conexao, string[] mensagemCompleta)
        {
            var comando = mensagemCompleta[0];
            switch (comando)
            {
                case "/u":
                    await ListarUsuarios(conexao);
                    break;

                case "/s":
                    await ListarSalas(conexao);
                    break;

                case "/s/c":
                    await CriarSala(conexao, mensagemCompleta);
                    break;

                case "/s/e":
                    await EntrarNaSala(conexao, mensagemCompleta);
                    break;

                case "/p":
                    await MensagemParaUsuario(conexao, mensagemCompleta);
                    break;

                case "/pm":
                    await MensagemPrivada(conexao, mensagemCompleta);
                    break;

                case "/exit":
                    await Sair(conexao);
                    break;

                case "/help":
                    await ObterAjuda(conexao);
                    break;

                default:
                    await EnviarMensagemDirecionada(conexao, "Comando inválido");
                    break;
            }
        }

        #region Comandos

        private async Task ListarUsuarios(Conexao conexao)
        {
            var sala = _gerenciadorDeSala.ObterSalaPorConexao(conexao);
            var apelidos = sala.Conexoes.Select(_ => _.Apelido);
            await EnviarMensagemDirecionada(conexao, string.Join(Environment.NewLine, apelidos));
        }

        private async Task ListarSalas(Conexao conexao)
        {
            var salas = _gerenciadorDeSala.ObterSalas();
            var nomes = salas.Select(_ => _.Nome);
            await EnviarMensagemDirecionada(conexao, string.Join(Environment.NewLine, nomes));
        }

        private async Task CriarSala(Conexao conexao, string[] mensagemCompleta)
        {
            if (mensagemCompleta.Length <= 1)
            {
                await ComandoIncompleto(conexao);
                return;
            }

            var nomeDaSala = mensagemCompleta[1];

            var salaCriada = _gerenciadorDeSala.AdicionarSala(nomeDaSala);
            var mensagemDeSalaFormatada = salaCriada ? $"Sala {nomeDaSala} criada" : "Não foi possível criar a sala";

            await EnviarMensagemDirecionada(conexao, mensagemDeSalaFormatada);
        }

        private async Task EntrarNaSala(Conexao conexao, string[] mensagemCompleta)
        {
            if (mensagemCompleta.Length <= 1)
            {
                await ComandoIncompleto(conexao);
                return;
            }

            var nomeDaSalaDestino = mensagemCompleta[1];

            var salaDestino = _gerenciadorDeSala.ObterSalaPorNome(nomeDaSalaDestino);
            if (salaDestino == null)
            {
                await EnviarMensagemDirecionada(conexao, $"Sala {nomeDaSalaDestino} não encontrada");
                return;
            }

            var sala = _gerenciadorDeSala.ObterSalaPorConexao(conexao);

            await EnviarMensagemParaTodos(sala.Nome, $"{conexao.Apelido} saiu da sala");
            await EnviarMensagemParaTodos(salaDestino.Nome, $"{conexao.Apelido} entrou na sala");
            await EnviarMensagemDirecionada(conexao, $"Você entrou na sala {salaDestino.Nome}");

        }

        private async Task MensagemParaUsuario(Conexao conexao, string[] mensagemCompleta)
        {
            if (mensagemCompleta.Length < 3)
            {
                await ComandoIncompleto(conexao);
                return;
            }

            var apelido = mensagemCompleta[1];

            var conexaoDestino = _gerenciadorDeConexao.ObterConexaoPorApelido(apelido);
            if (conexaoDestino == null)
            {
                await EnviarMensagemDirecionada(conexao, $"Usuário {apelido} não encontrado");
                return;
            }

            var sala = _gerenciadorDeSala.ObterSalaPorConexao(conexao);
            var mensagem = ConcatenarMensagemAPartirDoIndice(2, mensagemCompleta);
            var mensagemFormatada = $"{conexao.Apelido} disse para {conexaoDestino.Apelido}: {mensagem}";

            await EnviarMensagemParaTodos(sala.Nome, mensagemFormatada);
        }

        private async Task MensagemPrivada(Conexao conexao, string[] mensagemCompleta)
        {
            if (mensagemCompleta.Length < 3)
            {
                await ComandoIncompleto(conexao);
                return;
            }

            var apelido = mensagemCompleta[1];

            var conexaoPrivadaDestino = _gerenciadorDeConexao.ObterConexaoPorApelido(apelido);
            if (conexaoPrivadaDestino == null)
            {
                await EnviarMensagemDirecionada(conexao, $"Usuário {apelido} não encontrado");
                return;
            }

            var mensagem = ConcatenarMensagemAPartirDoIndice(2, mensagemCompleta);
            var mensagemPrivadaFormatada = $"{conexao.Apelido} disse de modo privado para {conexaoPrivadaDestino.Apelido}: {mensagem}";

            await EnviarMensagemDirecionada(conexao, mensagemPrivadaFormatada);
            await EnviarMensagemDirecionada(conexaoPrivadaDestino, mensagemPrivadaFormatada);
        }

        private string ConcatenarMensagemAPartirDoIndice(int indice, string[] mensagemCompleta)
        {
            var mensagemConcatenada = string.Empty;
            for (int i = indice; i < mensagemCompleta.Length; i++)
                mensagemConcatenada += $"{mensagemCompleta[i]} ";

            return mensagemConcatenada;
        }

        private async Task Sair(Conexao conexao)
        {
            var sala = _gerenciadorDeSala.ObterSalaPorConexao(conexao);
            await EnviarMensagemParaTodos(sala.Nome, $"{conexao.Apelido} se desconectou");
            _gerenciadorDeSala.SairDaSalaAtual(conexao);
            _gerenciadorDeConexao.RemoverConexao(conexao);
        }

        private async Task ObterAjuda(Conexao conexao)
        {
            StringBuilder sbAjuda = new StringBuilder();
            sbAjuda.AppendLine($"/u | Listar usuarios na sala");
            sbAjuda.AppendLine($"/s | Listar salas criadas");
            sbAjuda.AppendLine($"/s/c [nomeDaSala] | Criar uma sala");
            sbAjuda.AppendLine($"/s/e [nomeDaSala] | Entrar em uma sala");
            sbAjuda.AppendLine($"/p [apelido] | Mandar mensagem para um usuario da sala");
            sbAjuda.AppendLine($"/pm [apelido] | Mandar mensagem privada para um usuario");
            sbAjuda.AppendLine($"/exit | Sair");
            sbAjuda.AppendLine($"/help | Obter ajuda");
            await EnviarMensagemDirecionada(conexao, sbAjuda.ToString());
        }

        private async Task ComandoIncompleto(Conexao conexao)
        {
            await EnviarMensagemDirecionada(conexao, "O comando informado está incompleto, utilize /help para visualizar os comandos.");
        }

        #endregion

        private async Task EnviarMensagemDirecionada(WebSocket socket, string mensagem)
        {
            if (socket.State != WebSocketState.Open)
                return;

            var bytesEnvio = Encoding.ASCII.GetBytes(mensagem);
            var bufferEnvio = new ArraySegment<byte>(bytesEnvio, 0, mensagem.Length);

            await socket.SendAsync(
                buffer: bufferEnvio,
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: CancellationToken.None);
        }

        private async Task EnviarMensagemDirecionada(Conexao conexao, string mensagem)
        {
            await EnviarMensagemDirecionada(conexao.Socket, mensagem);
        }

        private async Task EnviarMensagemParaTodos(string nomeDaSala, string mensagem)
        {
            var sala = _gerenciadorDeSala.ObterSalaPorNome(nomeDaSala);
            if (sala == null)
                return;

            foreach (var conexao in sala.Conexoes)
            {
                if (conexao.Socket.State != WebSocketState.Open)
                    continue;

                await EnviarMensagemDirecionada(conexao, mensagem);
            }
        }
    }
}
