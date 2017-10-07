class PrivacyChecker {
  constructor(gitHubClient) {
    this.client = gitHubClient;
  }

  check(repo) {
    this.repo = repo;

    this.client.getIssues(this.repo.owner, this.repo.name).then(console.log);
  }

  analyseIssue(issue) {

  }
}

module.exports = PrivacyChecker;