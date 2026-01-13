using DrinksInfo.Controllers;
using DrinksInfo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DrinksInfo;

class Program
{
    static async Task Main(string[] args)
    {
        IServiceCollection services = new ServiceCollection();
        //services.AddDbContext<FlashcardContext>();
        services.AddTransient<DrinksService>();
        services.AddTransient<DrinksController>();
        ServiceProvider serviceProvider = services.BuildServiceProvider();


        var mainMenu = serviceProvider.GetService<DrinksController>()!;
        await mainMenu.CategoryMenuScreen();
    }
}
