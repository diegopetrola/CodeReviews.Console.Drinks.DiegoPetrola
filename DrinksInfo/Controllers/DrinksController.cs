//When the users open the application, they should be presented with the Drinks Category Menu and invited to choose a category. Then they'll have the chance to choose a drink and see information about it.
//When the users visualise the drink detail, there shouldn't be any properties with empty values.
//You should handle errors so that if the API is down, the application doesn't crash.

using DrinksInfo.Models;
using DrinksInfo.Services;
using DrinksInfo.Utils;
using Spectre.Console;

namespace DrinksInfo.Controllers;

internal class DrinksController(DrinksService drinksService)
{
    internal readonly DrinksService _drinksService = drinksService;

    public async Task CategoryMenuScreen()
    {
        try
        {
            while (true)
            {
                var categories = await _drinksService.GetCategories();
                categories.Add(new Category { strCategory = Shared.exitText });
                var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<Category>()
                            .Title("")
                            .AddChoices(categories)
                            .WrapAround(true)
                            .PageSize(30)
                            .UseConverter(cat => cat.strCategory)
                    );

                if (choice.strCategory == Shared.exitText) Environment.Exit(0);
                await DrinksFromCategoryScreen(choice);
            }
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{Styles.error}]Something went wrong[/]: {e.Message}]");
            Shared.AskForKey();
        }
    }

    public async Task DrinksFromCategoryScreen(Category category)
    {
        try
        {
            var drinks = await _drinksService.GetDrinksByCategory(category);
            drinks.Add(new Drink { strDrink = Shared.goBackText });

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<Drink>()
                    .Title("")
                    .AddChoices(drinks)
                    .WrapAround(true)
                    .UseConverter(drink => drink.strDrink));

            if (choice.strDrink == Shared.goBackText) return;
            //Todo: show drink

        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{Styles.error}]Something went wrong[/]: {e.Message}");
            Shared.AskForKey();
        }
    }
}
