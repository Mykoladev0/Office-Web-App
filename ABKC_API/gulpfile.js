let gulp = require('gulp');
let ts = require('gulp-typescript');
let sourcemaps = require('gulp-sourcemaps');

let tsProj = ts.createProject('tsconfig.json');

const paths = {
  node: "node_modules/",
  appjs: "Scripts/",
  appcss: "Content/",
  jsdist: "wwwroot/js",
  cssdist: "wwwroot/css",
  fontdist: "wwwroot/fonts",
  imgdist: "wwwroot/css/images",
  sasspath: "Content/scss/src/**/*.scss",
};

//start fresh.  clean out the destination build folder
gulp.task('clean', function () {
  const rmPaths = [
    paths.jsdist,
    paths.cssdist,
    paths.fontdist,
  ];
  const clean1 = gulp.src(rmPaths, {
    read: false
  }).pipe(clean());
  var clean2 = gulp.src(paths.sassdist + "/**/*.css", {
    read: false
  }).pipe(clean());
  return merge(clean1, clean2);
});


//compile all of the typescript files.
// Skip over test files.
gulp.task("tsScripts", function () {
    let tsResult = gulp.src([
      '!' + paths.appjs + "App/**/*.*",
      paths.appjs + "**/*.ts",
      '!' + paths.appjs + "**/*test.ts"
    ])
      .pipe(sourcemaps.init())
      .pipe(tsProj());
    return tsResult.js
      .pipe(sourcemaps.write('.'))
      .pipe(gulp.dest(paths.jsdist));
  });

  // Default Task
gulp.task('default', ['tsScripts']);