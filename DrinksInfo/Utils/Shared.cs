using Spectre.Console;

namespace DrinksInfo.Utils;

public static class Shared
{
    public static readonly string goBackText = $"[{Styles.subtle}]Go Back[/]";
    public static readonly string exitText = $"[{Styles.subtle}]Exit[/]";

    public static void AskForKey(string message = "Press any key to continue...")
    {
        AnsiConsole.MarkupLine($"[{Styles.subtle}]{message}[/]");
        Console.ReadKey();
    }
}
