using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelSamples.Abstractions;
using Spectre.Console;

namespace SemanticKernelSamples.Samples;
internal class ParkCapacityTrackingToolSample : BasicChatSample, ISample
{
    public override string Name => "Park Capacity Tracking Tool";
    public override Task Run(Kernel kernel)
    {
        AnsiConsole.WriteLine("This sample demonstrates how to use a plugin and native functions with a chat assistant.");
        AnsiConsole.WriteLine("The plugin keeps track of the number of people currently in a park and allows you to add or subtract from that count.");

        AnsiConsole.WriteLine();

        const string SYSTEM_PROMPT = """
            You are a helpful assistant. You help keep track of the number of people in the park.
            Feel free to name the park, it's just fictional.

            Please introduce yourself and explain what you can do and how you will be helping.

            Whenever the number of the people in the park changes, please recap the changes and share the updapted count.
        """;

        _ = kernel.Plugins.AddFromObject(new ParkCapacityPlugin(), "ParkCapacity");

        return Run(
            kernel,
            initialHistory: [new(AuthorRole.System, SYSTEM_PROMPT)],
            settings: new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            },
            useStreaming: true
        );
    }
}

file class ParkCapacityPlugin
{
    private int peopleInPark = 0;

    [KernelFunction("get_num_people_in_park")]
    public int GetPeopleInPark() => peopleInPark;

    [KernelFunction("add_people")]
    public int AddPeople(int count)
    {
        peopleInPark += count;
        return peopleInPark;
    }

    [KernelFunction("remove_people")]
    public int RemovePeople(int count)
    {
        peopleInPark -= count;
        return peopleInPark;
    }
}
