using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelSamples.Abstractions;
using Spectre.Console;

namespace SemanticKernelSamples.Samples.QuestionGenerationAgentSamples;

#pragma warning disable SKEXP0110 // AgentGroupChat is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // ChatHistoryTruncationReducer is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

internal class TerminatingQuestionGenerationAgentSample : QuestionGenerationAgent
{
    public override string Name => "Question Generation Agent / Terminating";
    public override async Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample demonstrates how to use an agent group chat to solve a problem with multi-turn invocations.");
        AnsiConsole.WriteLine("It uses two agents with a custom termination strategy to allow the iteration to end before reaching the max turn count.");

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

        KernelFunction terminationFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Determine if the evaluate scored the question high enough. If the score is 8 or higher, say yes.

                History:
                {{$history}}
                """,
            safeParameterNames: "history"
        );

        KernelFunctionTerminationStrategy terminationStrategy = new(terminationFunction, kernel)
          {
              // Only the evaluator may signal approval
              Agents = [evaluationAgent],

              // Parse the function response.
              ResultParser = (result) => result.GetValue<string>()?.Contains("yes", StringComparison.OrdinalIgnoreCase) ?? false,

              // The prompt variable name for the history argument.
              HistoryVariableName = "history",

              // Save tokens by not including the entire history in the prompt
              HistoryReducer = new ChatHistoryTruncationReducer(1, 1),

              // Limit total number of turns no matter what
              MaximumIterations = 10,
          };

        AgentGroupChat chat = new(generationAgent, evaluationAgent)
        {
            ExecutionSettings = new() { TerminationStrategy = terminationStrategy }
        };

        await ExecuteChatLoop(chat);
    }
}