using Microsoft.SemanticKernel;
using SemanticKernelSamples.Abstractions;
using SemanticKernelSamples.Samples;
using Spectre.Console;
using System.Text;

namespace SemanticKernelSamples;
public static class SampleSwitcher
{
    private static readonly List<ISample> _samples = [
        new BasicChatSample(),
        new BasicChatWithLoggingSample(),
        new StreamingChatSample(),
        new DisneyWorldPackingListPromptSample(),
        new DisneyWorldPackingListToolSample(),
        new ParkCapacityTrackingToolSample(),

        new Exit(),
    ];

    public static async Task Run(bool forceLoggingOn = false)
    {
        // Show emojis! 😁
        Console.OutputEncoding = Encoding.UTF8;

        do
        {
            Console.Clear();

            SelectionPrompt<ISample> prompt = new SelectionPrompt<ISample>()
                .Title("Which sample would you like to run?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                .UseConverter(sample => sample.Name)
                .AddChoices(_samples);

            ISample? sample = AnsiConsole.Prompt(prompt);

            if (sample is null or Exit)
            {
                AnsiConsole.Status()
                    .Start("[red]Exiting...[/]", _ => Thread.Sleep(TimeSpan.FromSeconds(2)));

                break;
            }

            AnsiConsole.Status()
                .Start($"You selected [green]{sample.Name}[/]. Starting...", _ => Thread.Sleep(TimeSpan.FromSeconds(2)));

            AnsiConsole.Clear();

            AnsiConsole.Write(new FigletText(sample.Name).LeftJustified().Color(Color.Blue));
            AnsiConsole.WriteLine();

            Kernel kernel = sample.GetKernel(enableLogging: forceLoggingOn);
            await sample.Run(kernel);

        } while (true);
    }
}

file class Exit : ISample
{
    public string Name => "Exit";

    public Kernel GetKernel(bool enableLogging = false) => throw new NotImplementedException();
    public Task Run(Kernel kernel) => throw new NotImplementedException();
}