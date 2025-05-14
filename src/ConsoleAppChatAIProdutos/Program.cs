using Bogus;
using ConsoleAppChatAIProdutos.Data;
using ConsoleAppChatAIProdutos.Inputs;
using ConsoleAppChatAIProdutos.Plugins;
using ConsoleAppChatAIProdutos.Tracing;
using Grafana.OpenTelemetry;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

Console.WriteLine("***** Testes com Semantic Kernel + Plugins (Kernel Functions) + PostgreSQL *****");
Console.WriteLine();

var aiSolution = InputHelper.GetAISolution();

var numberOfRecords = InputHelper.GetNumberOfNewProducts();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService(OpenTelemetryExtensions.ServiceName);

AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource(OpenTelemetryExtensions.ServiceName)
    .AddSource("Microsoft.SemanticKernel*")
    .AddNpgsql()
    .AddHttpClientInstrumentation()
    .UseGrafana()
    .Build();

var connectionString = configuration.GetConnectionString("BaseCatalogo")!;
CatalogoContext.ConnectionString = connectionString;

var db = new DataConnection(new DataOptions().UsePostgreSQL(connectionString));

var random = new Random();
var fakeEmpresas = new Faker<ConsoleAppChatAIProdutos.Data.Fake.Produto>("pt_BR").StrictMode(false)
            .RuleFor(p => p.Nome, f => f.Commerce.Product())
            .RuleFor(p => p.CodigoBarras, f => f.Commerce.Ean13())
            .RuleFor(p => p.Preco, f => random.Next(10, 30))
            .Generate(numberOfRecords);

if (numberOfRecords > 0)
{
    Console.WriteLine($"Gerando {numberOfRecords} produtos...");
    await db.BulkCopyAsync<ConsoleAppChatAIProdutos.Data.Fake.Produto>(fakeEmpresas);
    Console.WriteLine($"Produtos gerados com sucesso!");
}
else
    Console.WriteLine($"Nenhum novo foi produto gerado!");
Console.WriteLine();

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var kernelBuilder = Kernel.CreateBuilder();
PromptExecutionSettings settings;

if (aiSolution == InputHelper.OLLAMA)
{
    kernelBuilder.AddOllamaChatCompletion(
        modelId: configuration["Ollama:Model"]!,
        endpoint: new Uri(configuration["Ollama:Endpoint"]!),
        serviceId: "chat");
    settings = new OllamaPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };
}
else if (aiSolution == InputHelper.AZURE_OPEN_AI)
{
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: configuration["AzureOpenAI:DeploymentName"]!,
        endpoint: configuration["AzureOpenAI:Endpoint"]!,
        apiKey: configuration["AzureOpenAI:ApiKey"]!,
        serviceId: "chat");
    settings = new OpenAIPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };
}
else
    throw new Exception($"Solucao de AI invalida: {aiSolution}");

kernelBuilder.Plugins.AddFromType<ProdutosPlugin>();
Kernel kernel = kernelBuilder.Build();

#pragma warning restore SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var aiChatService = kernel.GetRequiredService<IChatCompletionService>();
var chatHistory = new ChatHistory();
while (true)
{    
    Console.WriteLine("Sua pergunta:");
    var userPrompt = Console.ReadLine();

    using var activity1 = OpenTelemetryExtensions.ActivitySource
        .StartActivity("PerguntaChatIAProdutos")!;

    chatHistory.Add(new ChatMessageContent(AuthorRole.User, userPrompt));

    Console.WriteLine();
    Console.WriteLine("Resposta da IA:");
    Console.WriteLine();

    ChatMessageContent chatResult = await aiChatService
        .GetChatMessageContentAsync(chatHistory, settings, kernel);
    Console.WriteLine();
    Console.WriteLine(chatResult.Content);
    chatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, chatResult.Content));

    Console.WriteLine();
    Console.WriteLine();
    
    activity1.Stop();
}