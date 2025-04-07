using Azure;
using Polly;
using Spectre.Console;
using System.ClientModel.Primitives;
using System.Net;

namespace SemanticKernelSamples.Abstractions;
internal class PollyRetryPolicy : PipelinePolicy
{
    private static readonly PolicyBuilder<PipelineResponse> policyBuilder = Policy
        .Handle<HttpRequestException>()
        .OrResult<PipelineResponse>(r => r.Status == (int)HttpStatusCode.TooManyRequests);


    private static readonly TimeSpan[] retrySequence =
    [
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(4),
        TimeSpan.FromSeconds(8),
        TimeSpan.FromSeconds(16),
        TimeSpan.FromSeconds(32)
    ];

    private static readonly int retryCount = retrySequence.Length;
    private static readonly TimeSpan maxDelay = retrySequence[retryCount - 1];

    private static TimeSpan GetSleepDuration(int retryNumber, DelegateResult<PipelineResponse> result, Context context)
    {
        // This is a simple exponential backoff strategy
        // The delay increases with each retry attempt

        if(result.Result.Headers.TryGetValue("Retry-After", out string? retryAfterHeaderValue) && int.TryParse(retryAfterHeaderValue, out int retryAfter))
        {
            AnsiConsole.MarkupLine($"[dim]Rate limit detected. Waiting {retryAfter}s before trying again.[/]");
            AnsiConsole.Write(new Rule().RuleStyle("dim"));
            return TimeSpan.FromSeconds(retryAfter);
        }


        int index = retryNumber - 1;
        return index >= retryCount ? maxDelay : retrySequence[index];
    }


    public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        policyBuilder.WaitAndRetry(retryCount, GetSleepDuration).Execute(() =>
        {
            ProcessNext(message, pipeline, currentIndex);
            return message.Response;
        });
    }

    public override async ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        await policyBuilder.WaitAndRetryAsync(retryCount, GetSleepDuration, (_, _, _, _) => Task.CompletedTask).ExecuteAsync(async () =>
        {
            await ProcessNextAsync(message, pipeline, currentIndex);
            return message.Response;
        });
    }


}

