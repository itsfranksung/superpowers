// Complete implementation of condition-based waiting utilities
// From: Lace test infrastructure improvements (2025-10-03)
// Context: Fixed 15 flaky tests by replacing arbitrary timeouts

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestInfrastructure.Waiting;

/// <summary>
/// Thread event types used in the application
/// </summary>
public enum LaceEventType
{
    ToolCall,
    ToolResult,
    AgentMessage
}

/// <summary>
/// Represents an event in a thread
/// </summary>
public record LaceEvent(LaceEventType Type, string Id, object? Data = null);

/// <summary>
/// Interface for thread manager 
/// </summary>
public interface IThreadManager
{
    IReadOnlyList<LaceEvent> GetEvents(string threadId);
}

/// <summary>
/// Condition-based waiting utilities for reliable async testing
/// </summary>
public static class ConditionWaiter
{
    private const int DefaultPollIntervalMs = 10;
    
    /// <summary>
    /// Wait for a specific event type to appear in thread
    /// </summary>
    /// <param name="threadManager">The thread manager to query</param>
    /// <param name="threadId">Thread to check for events</param>
    /// <param name="eventType">Type of event to wait for</param>
    /// <param name="timeoutMs">Maximum time to wait (default 5000ms)</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Task resolving to the first matching event</returns>
    /// <example>
    /// await ConditionWaiter.WaitForEventAsync(threadManager, agentThreadId, LaceEventType.ToolResult);
    /// </example>
    public static async Task<LaceEvent> WaitForEventAsync(
        IThreadManager threadManager,
        string threadId,
        LaceEventType eventType,
        int timeoutMs = 5000,
        CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeoutMs);

        while (!cts.Token.IsCancellationRequested)
        {
            var events = threadManager.GetEvents(threadId);
            var matchingEvent = events.FirstOrDefault(e => e.Type == eventType);

            if (matchingEvent is not null)
            {
                return matchingEvent;
            }

            await Task.Delay(DefaultPollIntervalMs, cts.Token);
        }

        throw new TimeoutException($"Timeout waiting for {eventType} event after {timeoutMs}ms");
    }

    /// <summary>
    /// Wait for a specific number of events of a given type
    /// </summary>
    /// <param name="threadManager">The thread manager to query</param>
    /// <param name="threadId">Thread to check for events</param>
    /// <param name="eventType">Type of event to wait for</param>
    /// <param name="count">Number of events to wait for</param>
    /// <param name="timeoutMs">Maximum time to wait (default 5000ms)</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Task resolving to all matching events once count is reached</returns>
    /// <example>
    /// // Wait for 2 AgentMessage events (initial response + continuation)
    /// await ConditionWaiter.WaitForEventCountAsync(threadManager, agentThreadId, LaceEventType.AgentMessage, 2);
    /// </example>
    public static async Task<IReadOnlyList<LaceEvent>> WaitForEventCountAsync(
        IThreadManager threadManager,
        string threadId,
        LaceEventType eventType,
        int count,
        int timeoutMs = 5000,
        CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeoutMs);

        while (!cts.Token.IsCancellationRequested)
        {
            var events = threadManager.GetEvents(threadId);
            var matchingEvents = events.Where(e => e.Type == eventType).ToList();

            if (matchingEvents.Count >= count)
            {
                return matchingEvents;
            }

            await Task.Delay(DefaultPollIntervalMs, cts.Token);
        }

        var finalEvents = threadManager.GetEvents(threadId);
        var finalCount = finalEvents.Count(e => e.Type == eventType);
        throw new TimeoutException(
            $"Timeout waiting for {count} {eventType} events after {timeoutMs}ms (got {finalCount})");
    }

    /// <summary>
    /// Wait for an event matching a custom predicate.
    /// Useful when you need to check event data, not just type.
    /// </summary>
    /// <param name="threadManager">The thread manager to query</param>
    /// <param name="threadId">Thread to check for events</param>
    /// <param name="predicate">Function that returns true when event matches</param>
    /// <param name="description">Human-readable description for error messages</param>
    /// <param name="timeoutMs">Maximum time to wait (default 5000ms)</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Task resolving to the first matching event</returns>
    /// <example>
    /// // Wait for ToolResult with specific ID
    /// await ConditionWaiter.WaitForEventMatchAsync(
    ///     threadManager,
    ///     agentThreadId,
    ///     e => e.Type == LaceEventType.ToolResult && e.Id == "call_123",
    ///     "ToolResult with id=call_123"
    /// );
    /// </example>
    public static async Task<LaceEvent> WaitForEventMatchAsync(
        IThreadManager threadManager,
        string threadId,
        Func<LaceEvent, bool> predicate,
        string description,
        int timeoutMs = 5000,
        CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeoutMs);

        while (!cts.Token.IsCancellationRequested)
        {
            var events = threadManager.GetEvents(threadId);
            var matchingEvent = events.FirstOrDefault(predicate);

            if (matchingEvent is not null)
            {
                return matchingEvent;
            }

            await Task.Delay(DefaultPollIntervalMs, cts.Token);
        }

        throw new TimeoutException($"Timeout waiting for {description} after {timeoutMs}ms");
    }
}

// Usage example from actual debugging session:
//
// BEFORE (flaky):
// ---------------
// var messageTask = agent.SendMessageAsync("Execute tools");
// await Task.Delay(300); // Hope tools start in 300ms
// agent.Abort();
// await messageTask;
// await Task.Delay(50);  // Hope results arrive in 50ms
// Assert.Equal(2, toolResults.Count); // Fails randomly
//
// AFTER (reliable):
// ----------------
// var messageTask = agent.SendMessageAsync("Execute tools");
// await ConditionWaiter.WaitForEventCountAsync(threadManager, threadId, LaceEventType.ToolCall, 2); // Wait for tools to start
// agent.Abort();
// await messageTask;
// await ConditionWaiter.WaitForEventCountAsync(threadManager, threadId, LaceEventType.ToolResult, 2); // Wait for results
// Assert.Equal(2, toolResults.Count); // Always succeeds
//
// Result: 60% pass rate → 100%, 40% faster execution
