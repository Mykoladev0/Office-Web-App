module.exports = {
  "globDirectory": "build/",
  "globPatterns": [
    "**/*.css",
    'index.html',
    'pages/404.html',
    'pages/offline.html',
    // 'images/home/*.jpg',
    // 'images/icon/*.svg'
  ],
  "swDest": "build\\sw.js",
  // "swSrc": "src/sw.js",
  "swSrc": "build/static/js/sw.js",
  "globIgnores": [
    '../workbox-config.js' //don't want the configuration file in all the JS
  ]
};