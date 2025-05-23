﻿using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelSamples.Abstractions;
using SemanticKernelSamples.Helpers;
using Spectre.Console;

namespace SemanticKernelSamples.Samples;
internal class DisneyWorldPackingListToolSample : BasicChatSample, ISample
{
    public override string Name => "Disney World Packing List Tool";

    public override async Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample demonstrates how to make a prompt available as a tool for the assistant to invoke.");
        AnsiConsole.WriteLine("It creates a plugin for the yaml file and turns on tool calling.");

        AnsiConsole.WriteLine();

        _ = kernel.ImportPluginFromPromptDirectoryYaml(FileHelper.BuildPathFromBaseDirectory("Plugins", "DisneyWorldPlugin"));

        const string system_prompt = @"""
            You are a helpful assistant who helps people plan their trips to Disney World.
            Please begin by helping the user put together a packing list.
        """;

        ChatHistory history = [new(AuthorRole.System, system_prompt)];

        await Run(
            kernel,
            history,
            settings: new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            },
            useStreaming: true
        );
    }
}
