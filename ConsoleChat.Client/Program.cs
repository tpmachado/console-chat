using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleChat.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var clientWebSocket = new ClientWebSocket();

            Console.WriteLine("Bem vindo(a) ao chat!");
            Console.WriteLine("Por favor, informe um apelido para se conectar: ");
            var apelido = Console.ReadLine();

            clientWebSocket.ConnectAsync(
                uri: new Uri($"ws://localhost:5000/ws?apelido={apelido}"),
                cancellationToken: CancellationToken.None).Wait();

            ProcessarMensagemRecebida(clientWebSocket);

            while (clientWebSocket.State == WebSocketState.Open)
            {
                var mensagem = Console.ReadLine();
                byte[] bytesEnvio = Encoding.UTF8.GetBytes(mensagem);
                var bufferEnvio = new ArraySegment<byte>(bytesEnvio);

                clientWebSocket.SendAsync(
                    buffer: bufferEnvio, 
                    messageType: WebSocketMessageType.Text,
                    endOfMessage: true,
                    cancellationToken: CancellationToken.None).Wait();

                if (mensagem == "/exit")
                    clientWebSocket.CloseAsync(
                        closeStatus: WebSocketCloseStatus.NormalClosure,
                        statusDescription: null,
                        cancellationToken: CancellationToken.None).Wait();
            }
        }

        static void ProcessarMensagemRecebida(ClientWebSocket clientWebSocket)
        {
            Action acaoRecebimento = async () =>
            {
                var bytesRecebidos = new byte[1024 * 4];
                var bufferRecebido = new ArraySegment<byte>(bytesRecebidos);
                while (clientWebSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(bufferRecebido, CancellationToken.None);
                    byte[] bytes = bufferRecebido.Skip(bufferRecebido.Offset).Take(result.Count).ToArray();
                    string mensagemRecebida = Encoding.UTF8.GetString(bytes);
                    Console.WriteLine(mensagemRecebida);
                }
            };

            Task.Factory.StartNew(
                acaoRecebimento, CancellationToken.None,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
