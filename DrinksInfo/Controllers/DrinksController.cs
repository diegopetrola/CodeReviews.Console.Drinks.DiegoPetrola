using DrinksInfo.Models;
using DrinksInfo.Services;
using DrinksInfo.Utils;
using Spectre.Console;

namespace DrinksInfo.Controllers;

internal class DrinksController(DrinksService drinksService)
{
    //Logo credits: https://patorjk.com/
    private readonly string logo = "[blue]  ___  ___ ___ _  _ _  _____   ___ _  _ ___ ___  \r\n |   \\| _ \\_ _| \\| | |/ / __| |_ _| \\| | __/ _ \\ \r\n | |) |   /| || .` | ' <\\__ \\  | || .` | _| (_) |\r\n |___/|_|_\\___|_|\\_|_|\\_\\___/ |___|_|\\_|_| \\___/ \r\n                                                 [/]";
    internal readonly DrinksService _drinksService = drinksService;

    public async Task CategoryMenuScreen()
    {
        try
        {
            while (true)
            {
                var categories = await _drinksService.GetCategories();
                categories.Add(new Category { strCategory = Shared.exitText });
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine(logo);
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
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<Drink>()
                    .Title("")
                    .AddChoices(drinks)
                    .WrapAround(true)
                    .PageSize(30)
                    .UseConverter(drink => drink.strDrink));

            if (choice.strDrink == Shared.goBackText) return;
            await DrinkScreen(choice);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{Styles.error}]Something went wrong[/]: {e.Message}");
            Shared.AskForKey();
        }
    }

    public async Task DrinkScreen(Drink drink)
    {
        try
        {
            var drinkDetail = await _drinksService.GetDrink(drink.idDrink);
            var panel = new Panel(FormatDrinkString(drinkDetail))
                .Border(BoxBorder.Rounded)
                .Header(drinkDetail.strDrink);

            panel.Header.Centered();
            panel.BorderStyle(Color.Blue);
            AnsiConsole.Clear();
            AnsiConsole.Write(panel);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[{Styles.error}]Error[/]: {e.Message}");
        }
        Shared.AskForKey();
    }

    private static string FormatDrinkString(DrinkDetail drink)
    {
        var formattedStr = $"""
            {$"[{Styles.subtle}]Category[/] {drink.strCategory}"}

            [{Styles.subtle}]Alcoholic:[/] [{Styles.warn}]{drink.strAlcoholic}[/] 
            [{Styles.subtle}]Glass:[/] {drink.strGlass}      {(drink.strIBA is not null && drink.strIBA != "" ? $"[{Styles.subtle}]IBA:[/] {drink.strIBA}" : "")}

            [{Styles.subtle}]Instructions:[/] {drink.strInstructions}

            """;

        for (var i = 1; i < 13; i++)
        {
            var ingredientVal = drink.GetType().GetProperty($"strIngredient{i}").GetValue(drink);
            var measureVal = drink.GetType().GetProperty($"strMeasure{i}").GetValue(drink);

            if (ingredientVal is not null && ingredientVal != "")
            {
                formattedStr += $"[{Styles.subtle}]Ingredient[/]: {ingredientVal}" +
                    $"{(measureVal is not null && measureVal != "" ? $" — {measureVal}" : "")}\n";
            }
            else { break; }
        }

        formattedStr += $"\n[{Styles.subtle}]Date modified:[/] {drink.dateModified}";
        return formattedStr;
    }
}
