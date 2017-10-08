const TextParser = require('./TextParser');

class PrivacyChecker {
  constructor(gitHubClient) {
    this.client = gitHubClient;
  }

  check(repo) {
    this.repo = repo;

    this.client.on('comment', comment => {
      let matches = this.analyseComment(comment);

      if (matches.length) {
        matches.forEach(match => {
          if (match.type == 'email') {
            comment.body = TextParser.replaceAt(comment.body, match.index, match.text, '[hidden email]');
          }

          if (match.type == 'italianTaxId') {
            comment.body = TextParser.replaceAt(comment.body, match.index, match.text, '[hidden TaxID]');
          }
        });

        this.client.updateComment(this.repo.owner, this.repo.name, comment.databaseId, comment.body)
        .then(res => {
          this.client.addComment(comment.issue.id, `[Automatic message] @${comment.author.login}, we have detected some personal information in some of your comments. We have hidden them due to privacy issues.`)
        })
        .catch(console.error);
      }
    });

    this.client.getAllIssuesComments(this.repo.owner, this.repo.name);
  }

  analyseComment(comment) {
    return TextParser.parse(comment.body);
  }
}

module.exports = PrivacyChecker;