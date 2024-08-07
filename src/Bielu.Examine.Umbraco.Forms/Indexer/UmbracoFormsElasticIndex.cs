using Bielu.Examine.Core.Services;
using bielu.Examine.Umbraco.Indexers.Indexers;
using Examine;
using Examine.Lucene;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Examine.Indexes;

namespace Bielu.Examine.ElasticSearch.Umbraco.Form.Indexer;

public class BieluExamineUmbracoFormsIndex(string? name, ILoggerFactory loggerFactory, IRuntime runtime, ILogger<IBieluExamineIndex> logger, ISearchService searchService, IIndexStateService stateService, IBieluSearchManager manager, IOptionsMonitor<LuceneDirectoryIndexOptions> indexOptions) : BieluExamineUmbracoIndex(name, loggerFactory, runtime, logger, searchService, stateService, manager, indexOptions), IUmbracoFormsRecordIndex
{

}
