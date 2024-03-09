﻿using Bielu.Examine.Elasticsearch.Configuration;
using Bielu.Examine.Elasticsearch.Services;
using Examine;
using Examine.Lucene;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;

namespace Bielu.Examine.Elasticsearch.Umbraco.Indexers;

public class UmbracoContentElasticsearchIndex(string? name, ILoggerFactory loggerFactory, IElasticSearchClientFactory factory, IRuntime runtime, ILogger<ElasticSearchUmbracoIndex> logger, IElasticsearchService elasticsearchService, IOptionsMonitor<LuceneDirectoryIndexOptions> indexOptions, IOptionsMonitor<BieluExamineElasticOptions> examineElasticOptions) : ElasticSearchUmbracoIndex(name, loggerFactory, factory, runtime, logger,elasticsearchService, indexOptions, examineElasticOptions), IUmbracoContentIndex
{

}
