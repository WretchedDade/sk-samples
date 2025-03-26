using Microsoft.SemanticKernel;
using SemanticKernelSamples.Abstractions;
using SemanticKernelSamples.Helpers;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelSamples.Samples;
internal class DisneyWorldPackingListPromptSample : Sample, ISample
{
    public override string Name => "Disney World Packing List Prompt";

    public override async Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample demonstrates how to use a prompt to generate a packing list for a trip to Disney World.");
        AnsiConsole.WriteLine("It is defined as a yaml file and uses the semantic-kernel templating engine to replace variables.");

        AnsiConsole.WriteLine();

        int days = AnsiConsole.Prompt(new TextPrompt<int>("How long in days is your trip to Disney World going to be?"));
        int people = AnsiConsole.Prompt(new TextPrompt<int>("How many people are going?"));

        var function = kernel.CreateFunctionFromPromptYaml(FileHelper.LoadPrompt("DisneyWorldPackingList"));

        var response = await kernel.InvokeAsync(function, arguments: new()
        {
            ["days"] = days.ToString(),
            ["people"] = people.ToString()
        });

        AnsiConsole.WriteLine();

        AnsiConsole.WriteLine("Packing List:");
        AnsiConsole.WriteLine(response.ToString());

        _ = Console.ReadKey();
    }
}
