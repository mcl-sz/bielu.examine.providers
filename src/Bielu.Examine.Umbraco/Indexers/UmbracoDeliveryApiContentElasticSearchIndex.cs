using Bielu.Examine.Core.Services;
using Examine.Lucene;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;

namespace bielu.Examine.Umbraco.Indexers.Indexers;

public class BieluExamineUmbracoDeliveryApiContentIndex(string? name, ILoggerFactory loggerFactory, IRuntime runtime, ILogger<IBieluExamineIndex> logger, ISearchService searchService, IIndexStateService stateService, IBieluSearchManager manager, IOptionsMonitor<LuceneDirectoryIndexOptions> indexOptions) : BieluExamineUmbracoIndex(name, loggerFactory, runtime, logger, searchService, stateService, manager, indexOptions)
{

}
