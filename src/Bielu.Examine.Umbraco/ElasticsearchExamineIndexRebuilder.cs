using Bielu.Examine.Core.Services;
using Examine;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Runtime;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Infrastructure.HostedServices;

namespace bielu.Examine.Umbraco;

public class ElasticsearchExamineIndexRebuilder :IIndexRebuilder
{
     private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly IExamineManager _examineManager;
    private readonly ILogger<ExamineIndexRebuilder> _logger;
    private readonly IMainDom _mainDom;
    private readonly IEnumerable<IIndexPopulator> _populators;
    private readonly object _rebuildLocker = new();
    private readonly IRuntimeState _runtimeState;
    private readonly IAppPolicyCache _runtimeCache;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExamineIndexRebuilder" /> class.
    /// </summary>
    public ElasticsearchExamineIndexRebuilder(
        IMainDom mainDom,
        IRuntimeState runtimeState,
        ILogger<ExamineIndexRebuilder> logger,
        IExamineManager examineManager,
        IEnumerable<IIndexPopulator> populators,
        IBackgroundTaskQueue backgroundTaskQueue,
        AppCaches appCaches)
    {
        _mainDom = mainDom;
        _runtimeState = runtimeState;
        _logger = logger;
        _examineManager = examineManager;
        _populators = populators;
        _backgroundTaskQueue = backgroundTaskQueue;
        _runtimeCache = appCaches.RuntimeCache;
    }

    public bool CanRebuild(string indexName)
    {
        if (!_examineManager.TryGetIndex(indexName, out IIndex index))
        {
            throw new InvalidOperationException("No index found by name " + indexName);
        }

        return _populators.Any(x => x.IsRegistered(index));
    }

    public virtual void RebuildIndex(string indexName, TimeSpan? delay = null, bool useBackgroundThread = true)
    {
        if (delay == null)
        {
            delay = TimeSpan.Zero;
        }

        if (!CanRun())
        {
            return;
        }

        if (useBackgroundThread)
        {
 #pragma warning disable CA1727
 #pragma warning disable CA1848
            _logger.LogInformation("Starting async background thread for rebuilding index {IndexName}", indexName);
 #pragma warning restore CA1848
 #pragma warning restore CA1727

            _backgroundTaskQueue.QueueBackgroundWorkItem(
                cancellationToken =>
                {
                    // Do not flow AsyncLocal to the child thread
                    using (ExecutionContext.SuppressFlow())
                    {
                        Task.Run(() => RebuildIndex(indexName, delay.Value, cancellationToken), cancellationToken);

                        // immediately return so the queue isn't waiting.
                        return Task.CompletedTask;
                    }
                });
        }
        else
        {
            RebuildIndex(indexName, delay.Value, CancellationToken.None);
        }
    }

    public virtual void RebuildIndexes(bool onlyEmptyIndexes, TimeSpan? delay = null, bool useBackgroundThread = true)
    {
        if (delay == null)
        {
            delay = TimeSpan.Zero;
        }

        if (!CanRun())
        {
            return;
        }

        if (useBackgroundThread)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
 #pragma warning disable CA1848
                _logger.LogDebug($"Queuing background job for {nameof(RebuildIndexes)}.");
 #pragma warning restore CA1848
            }

            _backgroundTaskQueue.QueueBackgroundWorkItem(
                cancellationToken =>
                {
                    // Do not flow AsyncLocal to the child thread
                    using (ExecutionContext.SuppressFlow())
                    {
                        // This is a fire/forget task spawned by the background thread queue (which means we
                        // don't need to worry about ExecutionContext flowing).
                        Task.Run(() => RebuildIndexes(onlyEmptyIndexes, delay.Value, cancellationToken), cancellationToken);

                        // immediately return so the queue isn't waiting.
                        return Task.CompletedTask;
                    }
                });
        }
        else
        {
            RebuildIndexes(onlyEmptyIndexes, delay.Value, CancellationToken.None);
        }
    }

    private bool CanRun() => _mainDom.IsMainDom && _runtimeState.Level == RuntimeLevel.Run;

    private void RebuildIndex(string indexName, TimeSpan delay, CancellationToken cancellationToken)
    {
        if (delay > TimeSpan.Zero)
        {
            Thread.Sleep(delay);
        }

        try
        {
            if (!Monitor.TryEnter(_rebuildLocker))
            {
 #pragma warning disable CA1848
                _logger.LogWarning(
                    "Call was made to RebuildIndexes but the task runner for rebuilding is already running");
 #pragma warning restore CA1848
            }
            else
            {
                if (!_examineManager.TryGetIndex(indexName, out IIndex index))
                {
                    throw new InvalidOperationException($"No index found with name {indexName}");
                }

                index.CreateIndex(); // clear the index
                foreach (IIndexPopulator populator in _populators)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    populator.Populate(index);
                }

                if (index is IBieluExamineIndex elasticIndex)
                {
                    // Reset the ExamineManagementController cache for 1 second  
                    var cacheKey = "temp_indexing_op_" + indexName;
                    _runtimeCache.Insert(cacheKey, () => "tempValue", TimeSpan.FromSeconds(1));

                    elasticIndex.SwapIndex();
                }

            }
        }
        finally
        {
            if (Monitor.IsEntered(_rebuildLocker))
            {
                Monitor.Exit(_rebuildLocker);
            }
        }
    }

    private void RebuildIndexes(bool onlyEmptyIndexes, TimeSpan delay, CancellationToken cancellationToken)
    {
        if (delay > TimeSpan.Zero)
        {
            Thread.Sleep(delay);
        }

        try
        {
            if (!Monitor.TryEnter(_rebuildLocker))
            {
 #pragma warning disable CA1848
                _logger.LogWarning(
                    $"Call was made to {nameof(RebuildIndexes)} but the task runner for rebuilding is already running");
 #pragma warning restore CA1848
            }
            else
            {
                // If an index exists but it has zero docs we'll consider it empty and rebuild
                IIndex[] indexes = (onlyEmptyIndexes
                    ? _examineManager.Indexes.Where(x =>
                        !x.IndexExists() || (x is IIndexStats stats && stats.GetDocumentCount() == 0))
                    : _examineManager.Indexes).ToArray();

                if (indexes.Length == 0)
                {
                    return;
                }

                foreach (IIndex index in indexes)
                {
                    index.CreateIndex(); // clear the index
                }

                // run each populator over the indexes
                foreach (IIndexPopulator populator in _populators)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        populator.Populate(indexes);
                    }
                    catch (Exception e)
                    {
 #pragma warning disable CA1848
                        _logger.LogError(e, "Index populating failed for populator {Populator}", populator.GetType());
 #pragma warning restore CA1848
                    }
                }
                foreach (IIndex index in indexes)
                {
                    if (index is IBieluExamineIndex elasticIndex)
                    {
                        elasticIndex.SwapIndex();
                    }
                }


            }
        }
        finally
        {
            if (Monitor.IsEntered(_rebuildLocker))
            {
                Monitor.Exit(_rebuildLocker);
            }
        }
    }
}
