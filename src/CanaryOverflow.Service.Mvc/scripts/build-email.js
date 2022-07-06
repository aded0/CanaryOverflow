const inlineCss = require("inline-css")
const {readFile, writeFile} = require("fs/promises");

readFile(".\\Email\\ConfirmationEmailTemplate.cshtml")
  .then(buf => buf.toString())
  .then(html => inlineCss(html, {url: "."}))
  .then(data => writeFile(".\\Email\\ConfirmationEmail.cshtml", data))
  .catch(console.log);
