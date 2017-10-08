using System;
using GraphQL;

namespace GraphQLSpike
{
    class Program
    {
        static void Main(string[] args)
        { 
            var client = new GraphQLClient("https://api.github.com/graphql", "token", "userAgent");
            PrivacyChecker pc = new PrivacyChecker(client, "repoOwnerName", "repoName", "token");
            pc.ExecuteAnalysis();
            Console.ReadLine();
        }
    }
}
