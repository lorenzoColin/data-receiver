using Elasticsearch.Net;
using Nest;

namespace data_receiver.ElasticConnection
{
    public class ElasticSearchClient
    {

        public ElasticClient EsClient()
        {

            var connection = new ConnectionSettings(new Uri("https://elastic:6j3iG7RMNJxvzBk6FlkMeodf@e72a9bd694ed48a1a4b453cec4229c9d.europe-west4.gcp.elastic-cloud.com:9243"))
                .DefaultIndex("sea_klanten").DefaultFieldNameInferrer(p => p);

          return new ElasticClient(connection);

        }

    }
}
