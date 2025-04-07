using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using SemanticKernelSamples.Abstractions;
using SemanticKernelSamples.Helpers;
using Spectre.Console;

namespace SemanticKernelSamples.Samples.QuestionGenerationAgentSamples;

#pragma warning disable SKEXP0110 // AgentGroupChat is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0010 // ResponseFormat is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

internal class BasicQuestionGenerationAgentSample : QuestionGenerationAgent, ISample
{
    public override string Name => "Question Generation Agent / Basic ";
    public override async Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample demonstrates how to use an agent group chat to solve a problem with multi-turn invocations.");
        AnsiConsole.WriteLine("It uses two agents with max iterations set to 10.");

        AnsiConsole.WriteLine();

        ChatCompletionAgent generationAgent = new()
        {
            Name = Constants.GenerationAgent,
            Instructions = Constants.GenerationInstructions,
            Kernel = kernel,
            Arguments = new KernelArguments(QuestionStructuredOutput)
        };

        ChatCompletionAgent evaluationAgent = new()
        {
            Name = Constants.EvaluationAgent,
            Instructions = Constants.EvaluationInstructions,
            Kernel = kernel,
        };

        AgentGroupChat chat = new(generationAgent, evaluationAgent)
        {
            // Override default execution settings
            ExecutionSettings =
            {
                TerminationStrategy = { MaximumIterations = 10 }
            }
        };

        await ExecuteChatLoop(chat);
    }
}