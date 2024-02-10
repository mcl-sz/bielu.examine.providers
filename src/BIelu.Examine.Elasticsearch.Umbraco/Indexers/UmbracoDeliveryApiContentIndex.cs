﻿using Bielu.Examine.Elasticsearch2.Configuration;
using Bielu.Examine.Elasticsearch2.Services;
using Examine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Services;

namespace BIelu.Examine.Umbraco.Indexers;

public class UmbracoDeliveryApiContentElasticSearchIndex(string? name, ILoggerFactory loggerFactory, IElasticSearchClientFactory factory, IRuntime runtime, ILogger<ElasticSearchUmbracoIndex> logger, IOptionsMonitor<IndexOptions> indexOptions, IOptionsMonitor<BieluExamineElasticOptions> examineElasticOptions) : ElasticSearchUmbracoIndex(name, loggerFactory, factory, runtime, logger, indexOptions, examineElasticOptions)
{

}
