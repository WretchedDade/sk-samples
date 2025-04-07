using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using SemanticKernelSamples.Abstractions;
using SemanticKernelSamples.Helpers;
using Spectre.Console;

namespace SemanticKernelSamples.Samples.QuestionGenerationAgentSamples;

#pragma warning disable SKEXP0010 // ResponseFormat is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

internal abstract class QuestionGenerationAgent : AgentSample
{
    protected override Dictionary<string, Color> AgentColorMap => new()
    {
        { Constants.GenerationAgent, Color.Green },
        { Constants.EvaluationAgent, Color.Blue },
        { Constants.ImprovementAgent, Color.Yellow },
    };

    protected AzureOpenAIPromptExecutionSettings QuestionStructuredOutput = new AzureOpenAIPromptExecutionSettings()
    {
        ResponseFormat = OpenAI.Chat.ChatResponseFormat.CreateJsonSchemaFormat("Question", BinaryData.FromString(typeof(Question).ToJsonSchema())),
    };
}
