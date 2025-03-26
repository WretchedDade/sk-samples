using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace SemanticKernelSamples.Abstractions;

public abstract class Sample : ISample
{
    public abstract string Name { get; }

    public virtual Kernel GetKernel(bool enableLogging = false)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        AzureOpenAIConfiguration azureOpenAIConfig = configuration.GetSection("AzureOpenAI").Get<AzureOpenAIConfiguration>()
            ?? throw new Exception("The AzureOpenAI configuration section is invalid or missing from appsettings.json. Please correct any issues and try again.");

        IKernelBuilder builder = Kernel.CreateBuilder();

        foreach (string chatDeployment in azureOpenAIConfig.Deployments.Chat)
        {
            _ = builder.AddAzureOpenAIChatCompletion(chatDeployment, azureOpenAIConfig.Endpoint, new AzureCliCredential());
        }

        if (enableLogging)
        {
            _ = builder.Services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
        }

        return builder.Build();
    }

    public abstract Task Run(Kernel kernel);

}

file class AzureOpenAIConfiguration
{
    public required string Endpoint { get; set; }

    public required DeploymentOptions Deployments { get; set; }
}

file class DeploymentOptions
{
    public required List<string> Chat { get; set; }
}