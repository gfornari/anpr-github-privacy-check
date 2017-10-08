const EventEmitter = require('events').EventEmitter;
const GraphQLClient = require('graphql-request').GraphQLClient;
const GitHubApi = require('github');
const endpoint = 'https://api.github.com/graphql';


class GitHubClient extends EventEmitter {
  constructor(token) {
    super();

    this.clientv3 = new GitHubApi({
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    this.clientv4 = new GraphQLClient(endpoint, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  addComment(subjectId, body) {
    const query = `mutation addComment($input: AddCommentInput!) {
      addComment(input: $input) {
        clientMutationId
      }
    }`;

    const variables = {
      input: {
        subjectId: subjectId,
        body: body,
      },
    };

    return this.clientv4.request(query, variables);
  }

  async getAllIssuesComments(repoOwner, repoName) {
    const query = `query getIssues($owner: String!, $name: String!, $first: Int!, $after: String) {
      repository(owner: $owner, name: $name) {
        id
        issues(first: $first, after: $after) {
          pageInfo {
            endCursor
            hasNextPage
          }
          edges {
            node {
              id
              body
              title
              number
              author {
                login
              }
            }
          }
        }
      }
    }`;

    let cursor = '';
    let hasNext = true;
    let variables = {
      owner: repoOwner,
      name: repoName,
      first: 100,
      after: null,
    };

    while (hasNext) {
      variables.after = cursor || null;

      let res = await this.clientv4.request(query, variables);
      hasNext = res.repository.issues.pageInfo.hasNextPage;
      cursor = res.repository.issues.pageInfo.endCursor;

      res.repository.issues.edges.forEach(issue => {
        this.getAllComments(repoOwner, repoName, issue.node);
      });
    }

    return true;
  }

  async getAllComments(repoOwner, repoName, issue) {
    const query = `query getComments($owner: String!, $name: String!, $number: Int!, $first: Int!, $after: String) {
      repository(owner: $owner, name: $name) {
        id
        issue(number: $number) {
          id
          comments(first: $first, after: $after) {
            pageInfo {
              endCursor
              hasNextPage
            }
            edges {
              node {
                id
                body
                author {
                  login
                }
                databaseId
                issue {
                  id
                }
              }
            }
          }
        }
      }
    }`;

    let cursor = '';
    let hasNext = true;
    let variables = {
      owner: repoOwner,
      name: repoName,
      number: issue.number,
      first: 100,
      after: null,
    };

    while (hasNext) {
      variables.after = cursor || null;

      let res = await this.clientv4.request(query, variables);
      hasNext = res.repository.issue.comments.pageInfo.hasNextPage;
      cursor = res.repository.issue.comments.pageInfo.endCursor;

      res.repository.issue.comments.edges.forEach(comment => {
        this.emit('comment', comment.node);
      });
    }

    return true;
  }

  updateComment(repoOwner, repoName, commentId, body) {
    return this.clientv3.issues.editComment({
      owner: repoOwner,
      repo: repoName,
      id: commentId,
      body: body,
    });
  }
}

module.exports = GitHubClient;
