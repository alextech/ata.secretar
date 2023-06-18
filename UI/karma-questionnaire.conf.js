const path = require('path');
let MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = function(config) {
  config.set({
    frameworks: ['mocha', 'chai'],
    files: [
      {pattern: 'Areas/Questionnaire/**/*.test.js', type:"module", watched: true},
      {pattern: 'Components/**/*.test.js', type: "module", watched: true}
    ],
    reporters: ['progress', 'junit'],
    port: 9877,  // karma web server port
    colors: true,
    logLevel: config.LOG_INFO,
    browsers: ['ChromeHeadless_Custom'],
    customLaunchers: {
      ChromeHeadless_Custom: {
        base: 'ChromeHeadless',
        flags: []
      }
    },
    autoWatch: false,
    // singleRun: false, // Karma captures browsers, runs the tests and exits
    // concurrency: Infinity,
    preprocessors: {
      'Areas/Questionnaire/**/*.test.js': ['webpack', 'sourcemap'],
      'Components/**/*.test.js': ['webpack', 'sourcemap']
    },
    webpack: {
      devtool: 'inline-source-map',
      resolve: {
        alias: {
        }
      },
      optimization: {
        minimize: false
      },
      module: {
        rules: [
          {test: /\.js$/, exclude: /node_modules/, loader: 'babel-loader'},
          // {
          //   test: /\.scss$/, use: [
          //     MiniCssExtractPlugin.loader,
          //     'css-loader?sourceMap',
          //     'sass-loader?sourceMap'
          //   ]
          // },
          {test: /\.(svg)$/, use: 'raw-loader'},
        ]
      },
    },
    webpackMiddleware: {
      stats: 'errors-only'
    }
    // webpackServer: {
    //   noInfo: true
    // }
  });
};
