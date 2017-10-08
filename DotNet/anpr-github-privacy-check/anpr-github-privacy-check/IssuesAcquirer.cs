using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using GraphQL;
using Newtonsoft.Json;

namespace GraphQLSpike
{
    public class IssuesAcquirer
    {
        private GraphQLClient _client;
        private CommentsAcquirer _commentsAcquirer;
        private string _repoName;
        private string _repoOwner;

        public void GetIssues(GraphQLClient client, string repoName, string repoOwner, string token)
        {
            _client = client;
            _repoName = repoName;
            _repoOwner = repoOwner;
            _commentsAcquirer = new CommentsAcquirer(_client, repoName, repoOwner, token);

            var observable = StartAcquisitionOfIssues();
            observable.Subscribe(
                HandleThings,
                onError => throw onError,
                () =>
                {
                   Console.WriteLine("Acquired all Issues");
                }
                );
        }

        private void HandleThings(RootObjectForIssues rootObjectForIssuesOnNext)
        {
            _commentsAcquirer.AcquireCommentsForIssues(rootObjectForIssuesOnNext.data.repository.issues.edges.Select(x => x.node.number));
        }

        private IObservable<RootObjectForIssues> StartAcquisitionOfIssues()
        {
            return Observable.Create<RootObjectForIssues>(
                async obs =>
                {
                    RootObjectForIssues callResponse = null;
                    bool ended = false;
                    while (!ended) {
                        callResponse = await GetBatchIssues(callResponse?.data.repository.issues.pageInfo.endCursor);
                        obs.OnNext(callResponse);
                        if (!callResponse.data.repository.issues.pageInfo.hasNextPage) {
                            ended = true;
                        }
                    }
                   obs.OnCompleted();
                });
            
        }

        private async Task<RootObjectForIssues> GetBatchIssues(string after = null)
        {
            after = after ?? "null";
            var query = string.Format("{{" +
                                      "repository(owner:{0},name:{1}) {{" +
                                      "issues(first: 1, after: {2}) {{" +
                                      "pageInfo {{" +
                                      " endCursor" +
                                      " hasNextPage" +
                                      " hasPreviousPage" +
                                      " startCursor" +
                                      "}}" +
                                      "edges {{" +
                                      "node {{" +
                                      "id, number" +
                                      "}}" +
                                      "}}" +
                                      "}}" +
                                      "}}" +
                                      "}}"
                , FormatHelper.FormatWithQuotes(_repoOwner),
                FormatHelper.FormatWithQuotes(_repoName),
                after);
            var result = await _client.Query(query, null);
            var raw = result.GetRaw();
            var rawObject = JsonConvert.DeserializeObject<RootObjectForIssues>(raw);
            return rawObject;
        }
    }
}