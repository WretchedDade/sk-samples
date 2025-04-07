using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console;

namespace SemanticKernelSamples.Abstractions;

#pragma warning disable SKEXP0110 // AgentGroupChat is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // response.AuthorName is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

internal abstract class AgentSample : Sample, ISample
{
    protected abstract Dictionary<string, Spectre.Console.Color> AgentColorMap { get; }

    protected async Task ExecuteChatLoop(AgentGroupChat chat)
    {
        do
        {
            chat.IsComplete = false;

            string userPrompt = AnsiConsole.Prompt(new TextPrompt<string>("User: "));

            if (userPrompt.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            AnsiConsole.Write(new Rule().RuleStyle("dim"));

            chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, userPrompt));

            await foreach (ChatMessageContent response in chat.InvokeAsync())
            {
                if (response.AuthorName is not null && AgentColorMap.TryGetValue(response.AuthorName, out Color color))
                {
                    AnsiConsole.MarkupLineInterpolated($"[{color}][[{response.AuthorName}]] {response.Content}[/]");
                }
                else
                {
                    AnsiConsole.WriteLine($"[{response.Role}] {response.Content}");
                }

                AnsiConsole.Write(new Rule().RuleStyle("dim"));
            }
        } while (true);
    }
}
