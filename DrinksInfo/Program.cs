using DrinksInfo.Controllers;
using DrinksInfo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DrinksInfo;

class Program
{
    static async Task Main()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddHttpClient<DrinksService>(client =>
        {
            client.BaseAddress = new Uri("http://www.thecocktaildb.com/api/json/v1/1/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddTransient<DrinksController>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        var mainMenu = serviceProvider.GetRequiredService<DrinksController>();
        await mainMenu.CategoryMenuScreen();
    }
}
