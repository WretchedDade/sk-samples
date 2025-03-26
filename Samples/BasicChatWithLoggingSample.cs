using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SemanticKernelSamples.Abstractions;
using Spectre.Console;

namespace SemanticKernelSamples.Samples;
public class BasicChatWithLoggingSample : BasicChatSample, ISample
{
    public override string Name => "Basic Chat w/ Logging";

    public override Kernel GetKernel(bool enableLogging = false)
    {

        return base.GetKernel(enableLogging: true);
    }

    public override Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample demonstrates how the logging capability works when enabled for a basic chat scenario.");

        AnsiConsole.WriteLine();

        return Run(kernel);
    }
}
