using Sharprompt;

namespace ConsoleAppChatAIProdutos.Inputs;

public class InputHelper
{
    public const string AZURE_OPEN_AI = "AzureOpenAI";
    public const string OLLAMA = "Ollama";

    public static int GetNumberOfNewProducts()
    {
        var answer = Prompt.Select<int>(options =>
        {
            options.Message = "Selecione a quantidade de produtos novos a serem criados";
            options.Items = new[] { 0, 1, 5, 10 };
        });
        Console.WriteLine();
        return answer;
    }

    public static string GetAISolution()
    {
        var answer = Prompt.Select<string>(options =>
        {
            options.Message = "Selecione a quantidade de registros a ser gerada";
            options.Items = new[] { AZURE_OPEN_AI, OLLAMA };
        });
        Console.WriteLine();
        return answer;
    }
}