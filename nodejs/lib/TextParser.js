function replaceAt(text, index, toReplace, replacement) {
  return text.substr(0, index) + replacement + text.substr(index + toReplace.length);
}

function parse(text) {
  let matches = [];
  let res;

  res = checkForEmail(text);
  if (res) {
    matches = matches.concat(findEmails(text, res));
  }

  res = checkForItalianTaxId(text);
  if (res) {
    matches = matches.concat(findItalianTaxId(text, res));
  }

  return matches;
}

function checkForEmail(text) {
  const re = /(([^<>()\[\]\.,;:\s@\"]+(\.[^<>()\[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})/ig;
  return text.match(re);
}

function findEmails(text, emails) {
  let matches = [];

  emails.forEach(email => {
    matches.push({
      type: 'email',
      text: email,
      index: text.indexOf(email),
    });
  });

  return matches;
}

function checkForItalianTaxId(text) {
  const re = /[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]/ig;
  return text.match(re);
}

function findItalianTaxId(text, taxIds) {
  let matches = [];

  taxIds.forEach(taxId => {
    matches.push({
      type: 'italianTaxId',
      text: taxId,
      index: text.indexOf(taxId),
    });
  });

  return matches;
}

exports.parse = parse;
exports.replaceAt = replaceAt;