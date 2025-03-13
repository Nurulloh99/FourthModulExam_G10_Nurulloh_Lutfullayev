using Bll.Services;
using Dal;
using Microsoft.Extensions.DependencyInjection;

namespace UserRegistration.Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));


            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IBotUserService, BotUserService>();
            serviceCollection.AddScoped<IUserInfoService, UserInfoService>();
            serviceCollection.AddSingleton<BotListenerService>();
            serviceCollection.AddSingleton<MainContext>();
            await serviceCollection.BuildServiceProvider().GetRequiredService<BotListenerService>().StartBot();


            Console.ReadKey();
        }
    }
}
