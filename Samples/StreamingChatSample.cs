using Microsoft.SemanticKernel;
using SemanticKernelSamples.Abstractions;
using Spectre.Console;

namespace SemanticKernelSamples.Samples;
public class StreamingChatSample : BasicChatSample, ISample
{
    public override string Name => "Streaming Chat";

    public override Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample demonstrates how the streaming capability works when enabled for a basic chat scenario.");

        AnsiConsole.WriteLine();

        return Run(kernel, useStreaming: true);
    }
}
