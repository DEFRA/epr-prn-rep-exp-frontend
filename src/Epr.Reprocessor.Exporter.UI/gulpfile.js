const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const cleanCSS = require('gulp-clean-css');
const sourcemaps = require('gulp-sourcemaps');
const rename = require('gulp-rename');
const { exec } = require('child_process');
const clean = require('gulp-clean');
const path = require('path');

const paths = {
    scss: {
        src: './assets/scss/**/*.scss',
        dest: './wwwroot/css/'
    },
    js: {
        src: 'assets/js/**/*.js',
        dest: 'wwwroot/js/'
    }
};

function cleanAssets() {
    return gulp.src(['wwwroot/css/*', 'wwwroot/js/*'], { read: false, allowEmpty: true })
        .pipe(clean());
}
let loadPaths = [
    path.join(__dirname, 'node_modules'),
    path.join(__dirname, 'node_modules/govuk-frontend/govuk')
];

function scssDev() {
    return gulp.src(paths.scss.src)
        .pipe(sourcemaps.init())
        .pipe(sass({ loadPaths: loadPaths }).on('error', sass.logError))
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest(paths.scss.dest));
}
function scssProd() {
    return gulp.src(paths.scss.src)
        .pipe(sass({ loadPaths: loadPaths }).on('error', sass.logError))
        .pipe(cleanCSS())
        //.pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(paths.scss.dest));
}

function jsDev(cb) {
    exec('npx esbuild assets/js/app.js --bundle --sourcemap --outfile=wwwroot/js/app.bundle.js', (err, stdout, stderr) => {
        console.log(stdout);
        console.error(stderr);
        cb(err);
    });
}

function jsProd(cb) {
    exec('npx esbuild assets/js/app.js --bundle --minify --outfile=wwwroot/js/app.bundle.js', (err, stdout, stderr) => {
        console.log(stdout);
        console.error(stderr);
        cb(err);
    });
}

function watchFiles() {
    gulp.watch(paths.scss.src, scssDev);
    gulp.watch(paths.js.src, jsDev);
}

// Define composite tasks
const dev = gulp.series(cleanAssets, gulp.parallel(scssDev, jsDev));
const prod = gulp.series(cleanAssets, gulp.parallel(scssProd, jsProd));

// Export tasks
exports.clean = cleanAssets;
exports.dev = dev;
exports.prod = prod;
exports.scssDev = scssDev;
exports.scssProd = scssProd;
exports.jsDev = jsDev;
exports.jsProd = jsProd
exports.watch = watchFiles;
