using GraphQL;

namespace GraphQLSpike
{
    public class PrivacyChecker
    {
        private readonly GraphQLClient _client;
        private readonly string _repoName;
        private readonly string _repoOwner;
        private readonly string _token;

        public PrivacyChecker(GraphQLClient client, string repoOwner, string repoName, string token)
        {
            _client = client;
            _repoName = repoName;
            _repoOwner = repoOwner;
            _token = token;
        }

        public void ExecuteAnalysis()
        {
            var issuesAcquirer = new IssuesAcquirer();
            issuesAcquirer.GetIssues(_client, _repoName, _repoOwner, _token);
        }
    }
}