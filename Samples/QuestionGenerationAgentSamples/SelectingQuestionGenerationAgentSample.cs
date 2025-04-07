using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console;

namespace SemanticKernelSamples.Samples.QuestionGenerationAgentSamples;

#pragma warning disable SKEXP0110 // AgentGroupChat is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // ChatHistoryTruncationReducer is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

internal class SelectingQuestionGenerationAgentSample : QuestionGenerationAgent
{
    public override string Name => "Question Generation Agent / Selecting";
    public override async Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample demonstrates how to use an agent group chat to solve a problem with multi-turn invocations.");
        AnsiConsole.WriteLine("It uses three agents with a custom selection strategy to a separate agent to handle initial generation and improvement");

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

        ChatCompletionAgent improvementAgent = new()
        {
            Name = Constants.ImprovementAgent,
            Instructions = Constants.ImprovementInstructions,
            Kernel = kernel,
            Arguments = new KernelArguments(QuestionStructuredOutput)
        };

        KernelFunction selectionFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Determine which participant takes the next turn in a conversation based on the the most recent participant.
                State only the name of the participant to take the next turn.
                No participant should take more than one turn in a row.

                Choose only from these participants:
                - {{{Constants.GenerationAgent}}}
                - {{{Constants.EvaluationAgent}}}
                - {{{Constants.ImprovementAgent}}}

                Always follow these rules when selecting the next participant:
                - If the user is asking to generate a question it is {{{Constants.GenerationAgent}}}'s turn.    
                - If the user is providing feedback or suggestions, it is {{{Constants.EvaluationAgent}}}'s turn.
                - If the user is looking for further evaluation, it is {{{Constants.EvaluationAgent}}}'s turn.
                - After {{{Constants.GenerationAgent}}} it is {{{Constants.EvaluationAgent}}}'s turn.
                - After {{{Constants.EvaluationAgent}}} it is {{{Constants.ImprovementAgent}}}'s turn.
                - After {{{Constants.ImprovementAgent}}} it is {{{Constants.EvaluationAgent}}}'s turn.

                History:
                {{$history}}
                """,
                safeParameterNames: "history"
        );

        KernelFunctionSelectionStrategy selectionStrategy = new(selectionFunction, kernel)
        {
            // Always start with the writer agent.
            InitialAgent = generationAgent,

            // Parse the function response.
            ResultParser = (result) => result.GetValue<string>() ?? Constants.EvaluationAgent,

            // The prompt variable name for the history argument.
            HistoryVariableName = "history",

            // Save tokens by not including the entire history in the prompt
            HistoryReducer = new ChatHistoryTruncationReducer(1),
        };

        KernelFunction terminationFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                Determine if the evaluate scored the question high enough. If the score is 9 or higher, say yes.

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

        AgentGroupChat chat = new(generationAgent, evaluationAgent, improvementAgent)
        {
            ExecutionSettings = new()
            {
                SelectionStrategy = selectionStrategy,
                TerminationStrategy = terminationStrategy,
            }
        };

        await ExecuteChatLoop(chat);
    }
}