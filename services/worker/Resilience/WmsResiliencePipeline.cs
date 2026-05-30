using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Omnichannel.Worker.Resilience;

/// <summary>
/// Builds the resilience pipeline for the worker → WMS HTTP call. This is the
/// "explicit reliability decision" the assignment requires (retry + circuit
/// breaker), and it is what makes QA-1 (resilience under WMS pressure) provable.
///
/// SCAFFOLD: thresholds come from <see cref="WorkerOptions"/>. The exponential
/// backoff and breaker are wired, but they have NOT yet been tuned against the
/// QA-1 measures (checkout P95 ≤ 1.5× baseline during a 30 s WMS 503; backlog
/// drain ≤ 60 s). Tune in Phase 3 and record the chosen numbers in README.md.
/// </summary>
public static class WmsResiliencePipeline
{
    public static ResiliencePipeline Build(WorkerOptions options, ILogger logger)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                MaxRetryAttempts = options.MaxRetryAttempts,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = args =>
                {
                    logger.LogWarning(
                        "WMS call retry {Attempt} after {Delay}ms due to {Exception}",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.Message);
                    return ValueTask.CompletedTask;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                // Open the breaker once failures dominate a sampling window.
                FailureRatio = 0.9,
                MinimumThroughput = options.CircuitBreakerFailureThreshold,
                SamplingDuration = TimeSpan.FromSeconds(30),
                BreakDuration = TimeSpan.FromSeconds(options.CircuitBreakerBreakSeconds),
                OnOpened = args =>
                {
                    logger.LogError(
                        "WMS circuit breaker OPENED for {BreakDuration}s — marking fulfillments degraded",
                        args.BreakDuration.TotalSeconds);
                    return ValueTask.CompletedTask;
                },
                OnClosed = _ =>
                {
                    logger.LogInformation("WMS circuit breaker CLOSED — draining backlog");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }
}
