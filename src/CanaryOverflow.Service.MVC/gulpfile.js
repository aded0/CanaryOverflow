const {series, src, dest, parallel} = require("gulp");
const del = require("del");
const concat = require("gulp-concat");
const gulpEsbuild = require("gulp-esbuild");

function cleanWwwroot() {
  return del("wwwroot/*");
}

function bundleCss() {
  return src([
    "node_modules\\normalize.css\\normalize.css",
    "Views\\**\\*.css"
  ])
    .pipe(concat("styles.css"))
    .pipe(dest("wwwroot"));
}

function bundleJs() {
  return src("Views\\application.js")
    .pipe(gulpEsbuild({
      bundle: true,
      outfile: "bundle.js",
      sourcemap: true
    }))
    .pipe(dest("wwwroot"));
}

exports.default = series(cleanWwwroot, parallel(bundleCss, bundleJs))
