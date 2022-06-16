const {series, src, dest} = require("gulp");
const del = require("del");
const concat = require("gulp-concat");

function cleanWwwroot(done) {
  return del("wwwroot/*", done);
}

function bundleCss() {
  return src(["node_modules\\normalize.css\\normalize.css", "Views\\**\\*.css"])
    .pipe(concat("styles.css"))
    .pipe(dest("wwwroot"));
}

exports.default = series(cleanWwwroot, bundleCss)
