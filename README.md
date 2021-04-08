Para a implementação do teste utilizei as seguintes tecnologias:

- C# (.Net 5.0)
- WebSockets

O projeto foi organizado na seguinte estrutura:

- ConsoleChat.Api
Nesta pasta estão os projetos de API e o client para conexão a api via Console.

- ConsoleChat.Domain
Nesta pasta estão os projetos que são o dmínio do sistema, nela estão contidas as classes de comunicação, interfaces e implementações dos serviços de comunicação.

- ConsoleChat.Infra.IoC
Nesta pasta está o projeto para a injeção de dependência dos projetos, optei por fazer separado pois havendo outro projeto de API eu posso utilizar essa mesma injeção de dependência.

- ConsoleChat.Tests
Nesta pasta está o projeto de testes da solução com a implementação dos testes dos serviços de comunicação.

Detalhes de implementação:

- Para conseguir capturar as requisições de socket, criei um Middleware para fazer a interceptação e controle das conexões para os serviços de comunicação.

- Segui uma ideia parecida com o Mockup do teste com todas as implentações solicitadas, incluindo as opcionais.

- Para rodar os projetos, criei os arquivos .bat.

- Para obter ajuda no projeto, utilizei o comando /help que lista todos os comandos permitidos e a forma de utilização de cada um deles.# console-chat
