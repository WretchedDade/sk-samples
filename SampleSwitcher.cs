using Microsoft.SemanticKernel;
using SemanticKernelSamples.Abstractions;
using SemanticKernelSamples.Samples;
using SemanticKernelSamples.Samples.QuestionGenerationAgentSamples;
using Spectre.Console;
using System.Text;

namespace SemanticKernelSamples;
public static class SampleSwitcher
{
    private static readonly List<ISample> samples = [
        new BasicChatSample(),
        new BasicChatWithLoggingSample(),
        new StreamingChatSample(),
        new DisneyWorldPackingListPromptSample(),
        new DisneyWorldPackingListToolSample(),
        new DisneyWorldPackingListStructuredSample(),
        new ParkCapacityTrackingToolSample(),
        new BasicQuestionGenerationAgentSample(),
        new TerminatingQuestionGenerationAgentSample(),
        new SelectingQuestionGenerationAgentSample(),

        new Exit(),
    ];

    public static async Task Run(bool forceLoggingOn = false)
    {
        try
        {
            do
            {
                Console.Clear();

                SelectionPrompt<ISample> prompt = new SelectionPrompt<ISample>()
                    .Title("Which sample would you like to run?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more samples)[/]")
                    .UseConverter(sample => sample.Name)
                    .AddChoices(samples);

                ISample? sample = AnsiConsole.Prompt(prompt);

                if (sample is null or Exit)
                {
                    AnsiConsole.Status()
                        .Start("[red]Exiting...[/]", _ => Thread.Sleep(TimeSpan.FromSeconds(2)));

                    break;
                }

                AnsiConsole.Status()
                    .Start($"You selected [green]{sample.Name}[/]. Starting...", _ => Thread.Sleep(TimeSpan.FromSeconds(1)));

                AnsiConsole.Clear();

                AnsiConsole.Write(new FigletText(sample.Name).LeftJustified().Color(Color.Blue));
                AnsiConsole.WriteLine();

                Kernel kernel = sample.GetKernel(enableLogging: forceLoggingOn);
                await sample.Run(kernel);

            } while (true);
        }
        catch (Exception exception)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule());
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
            AnsiConsole.WriteException(exception, ExceptionFormats.ShortenEverything);
        }
    }
}

file class Exit : ISample
{
    public string Name => "Exit";

    public Kernel GetKernel(bool enableLogging = false) => throw new NotImplementedException();
    public Task Run(Kernel kernel) => throw new NotImplementedException();
}