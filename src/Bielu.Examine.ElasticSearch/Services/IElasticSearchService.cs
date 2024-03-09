﻿using Bielu.Examine.Elasticsearch.Model;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Lucene.Net.Documents;

namespace Bielu.Examine.Elasticsearch.Services;

public interface IElasticsearchService
{
    public bool IndexExists(string examineIndexName);
    public IEnumerable<string>? GetCurrentIndexNames(string examineIndexName);
    public void EnsuredIndexExists(string examineIndexName);
    public void CreateIndex(string examineIndexName);
    Properties? GetProperties(string examineIndexName);
    ElasticSearchSearchResults Search(string examineIndexName,SearchRequestDescriptor<ElasticDocument> searchDescriptor);
    ElasticSearchSearchResults Search(string examineIndexName, SearchRequest<Document> searchDescriptor);
    void SwapTempIndex(string? name);
}