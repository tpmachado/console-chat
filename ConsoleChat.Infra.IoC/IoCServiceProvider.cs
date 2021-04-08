using ConsoleChat.Domain.Core.Interfaces;
using ConsoleChat.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleChat.Infra.IoC
{
    public static class IoCServiceProvider
    {
        public static void IoCServices(this IServiceCollection services)
        {
            services.AddSingleton<IGerenciadorDeConexao, GerenciadorDeConexao>();
            services.AddSingleton<IGerenciadorDeSala, GerenciadorDeSala>();
            services.AddSingleton<IGerenciadorDeChat, GerenciadorDeChat>();
        }
    }
}
