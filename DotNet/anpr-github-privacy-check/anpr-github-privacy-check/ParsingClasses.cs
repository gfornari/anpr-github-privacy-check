using System.Collections.Generic;

namespace GraphQLSpike
{
    public class IssueFields
    {
        public string id { get; set; }
        public string number { get; set; }
        public Comments comments { get; set; }
    }

    public class Issue
    {
        public IssueFields node { get; set; }
    }

    public class Issues
    {
        public PageInfo pageInfo { get; set; }
        public List<Issue> edges { get; set; }
    }

    public class RepositoryForIssues
    {
        public Issues issues { get; set; }
    }

    public class DataForIssues
    {
        public RepositoryForIssues repository { get; set; }
    }

    public class RootObjectForIssues
    {
        public DataForIssues data { get; set; }
    }

    public class PageInfo
    {
        public string endCursor { get; set; }
        public bool hasNextPage { get; set; }
        public bool hasPreviousPage { get; set; }
        public string startCursor { get; set; }
    }

    public class Node
    {
        public string id { get; set; }
        public string body { get; set; }
        public int databaseId { get; set; }
    }

    public class Edge
    {
        public Node node { get; set; }
    }

    public class Comments
    {
        public PageInfo pageInfo { get; set; }
        public List<Edge> edges { get; set; }
    }

    public class RepositoryForComments
    {
        public IssueFields issue { get; set; }
    }

    public class DataForComments
    {
        public RepositoryForComments repository { get; set; }
    }

    public class RootObjectForComments
    {
        public DataForComments data { get; set; }
    }
}