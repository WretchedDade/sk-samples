using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using SemanticKernelSamples.Abstractions;
using SemanticKernelSamples.Helpers;
using Spectre.Console;
using Spectre.Console.Json;
using System.Text.Json;
using System.Text.Json.Schema;
using System.Text.Json.Serialization;

namespace SemanticKernelSamples.Samples;
internal class DisneyWorldPackingListStructuredSample : BasicChatSample, ISample
{
    public override string Name => "Disney World Packing List Structured";

    public override async Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample demonstrates how a class can be used to define the response format when generate a packing list for a trip to Disney World.");
        AnsiConsole.WriteLine("It is defined as a yaml file and uses the semantic-kernel templating engine to replace variables.");

        AnsiConsole.WriteLine();

        int days = AnsiConsole.Prompt(new TextPrompt<int>("How long in days is your trip to Disney World going to be?"));
        int people = AnsiConsole.Prompt(new TextPrompt<int>("How many people are going?"));

        _ = kernel.ImportPluginFromPromptDirectoryYaml(FileHelper.BuildPathFromBaseDirectory("Plugins", "DisneyWorldPlugin"));

        AzureOpenAIPromptExecutionSettings settings = new()
        {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            ResponseFormat = OpenAI.Chat.ChatResponseFormat.CreateJsonSchemaFormat("PackingList", BinaryData.FromString(typeof(PackingList).ToJsonSchema())),
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed
        };

        KernelArguments arguments = new(settings)
        {
            ["days"] = days.ToString(),
            ["people"] = people.ToString()
        };

        FunctionResult response = await kernel.InvokeAsync("DisneyWorldPlugin", "GenerateDisneyWorldPackingList", arguments);

        string content = response.ToString();

        AnsiConsole.WriteLine();

        AnsiConsole.WriteLine("Packing List:");
        AnsiConsole.Write(new JsonText(content));

        _ = JsonSerializer.Deserialize<PackingList>(content, JsonSerializerOptions.Web) ?? throw new Exception("The content could not be deserialized");

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[gray](press any key to continue)[/]");

        _ = Console.ReadKey();
    }
}

file class PackingList
{
    public required string Name { get; set; }
    public required List<PackingListGroup> Groups { get; set; } = [];
}

file class PackingListGroup
{
    public required string Category { get; set; }
    public required List<PackingListItem> Items { get; set; } = [];
}

file class PackingListItem
{
    public required string Name { get; set; }

    [JsonNumberHandling(JsonNumberHandling.Strict)]
    public required int Quantity { get; set; }

    public required bool IsEssential { get; set; }
}