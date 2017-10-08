using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Newtonsoft.Json;
using Octokit;

namespace GraphQLSpike
{
    public class CommentsAcquirer
    {
        private readonly GraphQLClient _client;
        private readonly string _repoName;
        private readonly string _owner;
        private readonly string _token;
        private readonly TextAnalyzer _textAnalyzer;

        public CommentsAcquirer(GraphQLClient client, string repoName, string owner, string token)
        {
            _textAnalyzer = new TextAnalyzer();
            _client = client;
            _repoName = repoName;
            _owner = owner;
            _token = token;
        }

        public void AcquireCommentsForIssues(IEnumerable<string> issues)
        {
            foreach (var issue in issues) {
                var thread = new Thread(async () =>
                {
                    await AnalyzeCommentsForIssue(issue);
                });
                thread.Start();
            }
        }

        private async Task AnalyzeCommentsForIssue(string issue)
        {
            bool endedCommentScan = false;
            string lastNextId = null;
            while (!endedCommentScan) {
                var query = string.Format(@"
                    {{repository(owner: {2}, name: {3}) 
                        {{
                            issue(number: {0}) {{
                                id
                                number,
                                comments(first: 10,after:{1}) {{
                                    pageInfo {{
                                        endCursor
                                        hasNextPage
                                        hasPreviousPage
                                        startCursor
                                    }}
                                    edges {{
                                        node {{
                                            id
                                            body,
                                            databaseId
                                        }}
                                    }}
                                }}
                            }}
                        }}
                    }}",
                    FormatHelper.FormatWithQuotes(issue),
                    FormatHelper.FormatWithQuotes(lastNextId) ?? "null",
                    FormatHelper.FormatWithQuotes(_owner),
                    FormatHelper.FormatWithQuotes(_repoName));
                var result = await _client.Query(query, null);
                var raw = result.GetRaw();
                RootObjectForComments parsedObject = JsonConvert.DeserializeObject<RootObjectForComments>(raw);
                endedCommentScan = !parsedObject.data.repository.issue.comments.pageInfo.hasNextPage;
                lastNextId = parsedObject.data.repository.issue.id;
                var thread = new Thread(async () =>
                {
                    var comments = parsedObject.data.repository.issue.comments.edges;
                    foreach (var comment in comments) {
                        var responseForEmail = _textAnalyzer.SearchForEmail(comment.node.body);
                        var fiscalCodeResponse = _textAnalyzer.SearchForFiscalCode(responseForEmail.Text);
                        if (responseForEmail.PersonalInformationFound || fiscalCodeResponse.PersonalInformationFound) {
                            await UpdateComment(comment.node.databaseId, fiscalCodeResponse.Text, _token, _repoName, _owner);
                        }
                    }
                });
                thread.Start();
            }            
        }

        public async Task UpdateComment(int issueId, string responseText, string token, string repoName, string repoOwner)
        {
            var client = new GitHubClient(new ProductHeaderValue("DQuaglio"));
            var tokenAuth = new Credentials(token);
            client.Credentials = tokenAuth;
            var response = await client.Issue.Comment.Update(repoOwner, repoName, issueId, responseText);
        }
    }
}