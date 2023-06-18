module.exports = function(config) {
  config.set({
    frameworks: ['mocha', 'chai'],
    files: [
      {pattern: 'wwwroot/**/*.test.js', type:"module", watched: false},
      {pattern: 'wwwroot/allocations/allocations.js', type:'module', included: true, served: true, nocache: true},
      {pattern: 'wwwroot/allocations/AllocationOption.js', type:'module', included: true, served: true, nocache: true},
      {pattern: 'wwwroot/allocations/AllocationOptionControls.js',  type:'module', included: true, served: true, nocache: true},
      {pattern: 'wwwroot/allocations/AllocationRow.js',  type:'module', included: true, served: true, nocache: true}
    ],
    reporters: ['progress', 'junit'],
    port: 9877,  // karma web server port
    colors: true,
    logLevel: config.LOG_INFO,
    browsers: ['ChromeHeadless'],
    customLaunchers: {
      ChromeHeadless_Custom: {
        base: 'ChromeHeadless',
        flags: []
      }
    }
  });
};
