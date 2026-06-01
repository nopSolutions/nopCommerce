using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VerdeMart.OpenBoxesBridge.Messaging;

public enum CircuitState { Closed, Open, HalfOpen }

// Singleton — tracks OpenBoxes reachability across all message deliveries.
// Closed  : normal operation, calls go through.
// Open    : OpenBoxes known down, all calls blocked, messages routed to overflow queue.
// HalfOpen: timeout elapsed, one probe call allowed to test recovery.
public class CircuitBreaker
{
    private readonly int _threshold;
    private readonly TimeSpan _openTimeout;
    private readonly ILogger<CircuitBreaker> _logger;

    private CircuitState _state = CircuitState.Closed;
    private int _consecutiveFailures;
    private DateTime _openedAt = DateTime.MinValue;

    public CircuitBreaker(IOptions<BridgeSettings> options, ILogger<CircuitBreaker> logger)
    {
        _threshold = options.Value.CircuitBreakerFailureThreshold;
        _openTimeout = TimeSpan.FromSeconds(options.Value.CircuitBreakerOpenTimeoutSeconds);
        _logger = logger;
    }

    public CircuitState State
    {
        get
        {
            if (_state == CircuitState.Open && DateTime.UtcNow - _openedAt >= _openTimeout)
                return CircuitState.HalfOpen;
            return _state;
        }
    }

    // Returns true when the caller should attempt the OpenBoxes call (Closed or HalfOpen).
    public bool ShouldAttempt() => State != CircuitState.Open;

    // Call after a successful OpenBoxes response. Resets the circuit to Closed.
    public void RecordSuccess()
    {
        if (_state != CircuitState.Closed)
            _logger.LogInformation("Circuit closed — OpenBoxes responding normally");
        _state = CircuitState.Closed;
        _consecutiveFailures = 0;
    }

    // Call after a transient OpenBoxes failure.
    public void RecordFailure()
    {
        _consecutiveFailures++;
        if (_state == CircuitState.Closed && _consecutiveFailures >= _threshold)
        {
            _state = CircuitState.Open;
            _openedAt = DateTime.UtcNow;
            _logger.LogWarning(
                "Circuit opened after {Failures} consecutive transient failures — consumer will be paused",
                _consecutiveFailures);
            return;
        }

        if (_state == CircuitState.Open || State == CircuitState.HalfOpen)
        {
            _state = CircuitState.Open;
            _openedAt = DateTime.UtcNow;
            _logger.LogWarning("Half-open probe failed — circuit re-opened, timer reset");
        }
    }
}
