var paths = {};
paths.webroot = './wwwroot/';
paths.bower_components = './bower_components/';
paths.assetsSource = './app/assets/**';
paths.htmlSource = './app/*.html';
paths.sassSource = './app/scss/*.scss';
paths.jsAppSource = './app/*.js';
paths.jsLibSource = paths.bower_components + '*/dist/**/*.min.js';
paths.cssOutput = paths.webroot + 'css';
paths.libOutput = paths.webroot + 'lib';

var gulp = require('gulp'),
    sass = require('gulp-sass'),
    babel = require('gulp-babel'),
    cachebust = require('gulp-cache-bust'),
    del = require('del');

gulp.task('sass', ['clean'], function () {
    return gulp.src(paths.sassSource)
        .pipe(sass({
            includePaths: [
                paths.bower_components + 'foundation-sites/scss'
            ]
        }))
        .on('error', sass.logError)
        .pipe(gulp.dest(paths.cssOutput));
});

gulp.task('assetsCopy', ['clean'], function() {
    return gulp.src(paths.assetsSource)
        .pipe(gulp.dest(paths.webroot + 'assets/'));
});

gulp.task('htmlCopy', ['clean'], function () {
    return gulp.src(paths.htmlSource)
        .pipe(gulp.dest(paths.webroot));
});

gulp.task('jsCopy', ['jsCopy:lib', 'jsCopy:app']);

gulp.task('jsCopy:app', ['clean'], function () {
    return gulp.src(paths.jsAppSource)
        .pipe(babel())
        .pipe(gulp.dest(paths.webroot));
});

gulp.task('jsCopy:lib', ['clean'], function () {
    return gulp.src(paths.jsLibSource)
        .pipe(gulp.dest(paths.libOutput));
});

gulp.task('cacheBust', ['sass', 'htmlCopy', 'jsCopy', 'assetsCopy'], function () {
    return gulp.src(paths.webroot + '*.html')
        .pipe(cachebust({
            type: 'timestamp'
        }))
        .pipe(gulp.dest(paths.webroot));
});

gulp.task('clean', [], function () {
    return del(paths.webroot + '**', '!' + paths.webroot).then(
        console.log('Build directory cleaned.'));
});

gulp.task('watch', function() {
    return gulp.watch([paths.htmlSource, paths.sassSource, paths.jsAppSource], ['build']);
});

gulp.task('build', ['cacheBust']);