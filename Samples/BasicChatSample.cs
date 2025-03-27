using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelSamples.Abstractions;
using Spectre.Console;
using System.Text;

namespace SemanticKernelSamples.Samples;
public class BasicChatSample : Sample, ISample
{
    public override string Name => "Basic Chat";

    public override Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample how a basic chat with the assistant works.");
        AnsiConsole.WriteLine("It uses the chat completion service to generate responses to user input.");

        AnsiConsole.WriteLine();

        return Run(kernel);
    }

    protected static async Task Run(Kernel kernel, ChatHistory? initialHistory = null, PromptExecutionSettings? settings = null, bool useStreaming = false)
    {
        ChatHistory history = [.. initialHistory ?? []];

        IChatCompletionService chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        if (!history.Any(message => message.Role == AuthorRole.System))
        {
            // Seed with initial system message if another one wasn't already specified
            history.AddSystemMessage("You are a friendly assistant here to chat with the user about whatever they want. Do your best!");
        }

        history.AddSystemMessage("Please introduce yourself.");

        do
        {
            if (useStreaming)
            {
                IAsyncEnumerable<StreamingChatMessageContent> response = chatCompletionService.GetStreamingChatMessageContentsAsync(history, settings, kernel);

                StringBuilder stringBuilder = new();
                AnsiConsole.Write("Assistant: ");

                await foreach (StreamingChatMessageContent message in response)
                {
                    if (message.Content is string content)
                    {
                        Console.Write(content);
                        _ = stringBuilder.Append(content);
                    }
                }

                AnsiConsole.WriteLine();
                history.AddAssistantMessage(stringBuilder.ToString());
            }
            else
            {
                IReadOnlyList<ChatMessageContent> response = await chatCompletionService.GetChatMessageContentsAsync(history, settings, kernel);

                foreach (ChatMessageContent message in response)
                {
                    if (message.Content is string content)
                    {
                        AnsiConsole.WriteLine($"Assistant: {content}");
                        history.AddAssistantMessage(content);
                    }
                }
            }

            string userPrompt = AnsiConsole.Prompt(new TextPrompt<string>("User: "));

            if (userPrompt.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            history.AddUserMessage(userPrompt);

        } while (true);
    }
}
