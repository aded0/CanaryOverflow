const gulp = require('gulp');
const sourcemaps = require('gulp-sourcemaps')
const rename = require('gulp-rename');
const sass = require('gulp-sass')(require('sass'));
const postcss = require('gulp-postcss')
const autoprefixer = require('autoprefixer');
const inlineCss = require('gulp-inline-css');

function buildStyles() {
  return gulp.src('./Styles/index.scss')
    .pipe(sourcemaps.init())
    .pipe(sass({outputStyle: 'compressed'}).on('error', sass.logError))
    .pipe(postcss([autoprefixer()]))
    .pipe(rename({basename: 'styles', extname: '.css'}))
    .pipe(sourcemaps.write('.'))
    .pipe(gulp.dest('wwwroot/css'));
}

function buildEmail() {
  return gulp.src('../CanaryOverflow.Email/ConfirmationEmail.html')
    .pipe(inlineCss({
      removeHtmlSelectors: true
    }))
    .pipe(gulp.dest('../CanaryOverflow.Email/Generated/'));
}

exports.default = gulp.parallel(buildStyles, buildEmail);
exports.styles = buildStyles;
exports.email = buildEmail;
