using Microsoft.SemanticKernel;

namespace SemanticKernelSamples.Abstractions;
public interface ISample
{
    string Name { get; }

    Kernel GetKernel(bool enableLogging = false);
    Task Run(Kernel kernel);
}