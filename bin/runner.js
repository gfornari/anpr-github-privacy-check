const util = require('util');
const agpc = require('../index.js');

const token = process.env.GITHUB_ACCESS_TOKEN;

const client = new agpc.GitHubClient(token);
const pc = new agpc.PrivacyChecker(client);

pc.check({
  owner: 'gfornari',
  name: 'anpr-github-privacy-check-test',
});


// .then(res => console.log(util.inspect(res, { showHidden: true, depth: null })))

// .catch(console.error);