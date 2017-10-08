# anpr-github-privacy-check

Node.js module implementation of the challenge [Piattaforma di ricerca e rimozione di informazioni personali da issue github](https://github.com/italia/anpr/issues/313) developed during the first hackathon dedicated to the digital transformation of the italian Public Administration.

## Requirements

* Node.js version 8 or greater
* A file `.env` in the root directory with something like `GITHUB_ACCESS_TOKEN=xxxxxxxxxxxxxxxxxxx`, that is your GitHub Personal Access Token (or an equivalent environmental variable).

## Usage

There's an example of the usage in the file `bin/runner.js`, that is:

```
const util = require('util');
const agpc = require('../index.js');

const token = process.env.GITHUB_ACCESS_TOKEN;

const client = new agpc.GitHubClient(token);
const pc = new agpc.PrivacyChecker(client);

pc.check({
  owner: 'gfornari',
  name: 'anpr-github-privacy-check-test',
});
```

The function `check` takes an object with the name of the repository and its owner's name. It retrives every comment in the given repository, checks if there are email addresses or italian TaxIDs in each comment, updates them hiding those information, and adds a comment notifying the user about what happened.
