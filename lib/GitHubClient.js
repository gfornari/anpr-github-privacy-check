const GraphQLClient = require('graphql-request').GraphQLClient;
const co = require('co');
const endpoint = 'https://api.github.com/graphql';

class GitHubClient extends GraphQLClient {
  constructor(token) {
    super(endpoint, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  checkPrivacy(repo) {
    const query = `query getCommentsIssuesOfRepo($owner: String!, $name: String!, $first: String!, $after: String!) {
      repository(owner: $owner, name: $name) {
        id,
        issues(first: 2) {
          edges {
            node {
              id,
              title,
              body,
              comments(first: 2) {
                edges {
                  node {
                    id,
                    body
                  }
                }
              }
            }
          }
        }
      }
    }`;

    const variables = {
      owner: repo.owner,
      name: repo.name,
    }

    return this.request(query, variables);
  }

  async getIssues(repoOwner, repoName) {
    let cursor = '';
    let hasNext = true;
    let variables;
    let issues = [];

    const query = `query getIssues($owner: String!, $name: String!, $first: Int!, $after: String) {
      repository(owner: $owner, name: $name) {
        id,
        issues(first: $first, after: $after) {
          pageInfo {
            endCursor
            hasNextPage
            hasPreviousPage
            startCursor
          },
          edges {
            node {
              id
              body
              title
              author {
                login
              }
            }
          }
        }
      }
    }`;

    while (hasNext) {
      variables = {
        owner: repoOwner,
        name: repoName,
        first: 1,
        after: cursor || null,
      };

      let res = await this.request(query, variables);
      hasNext = res.repository.issues.pageInfo.hasNextPage;
      cursor = res.repository.issues.pageInfo.endCursor;
      issues = issues.concat(res.repository.issues.edges);
    }

    return issues;
  }
}

module.exports = GitHubClient;
