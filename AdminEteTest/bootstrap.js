
// puppeteer options
const opts = {
  headless: false,
  devtools: true,
  slowMo: 1000,
  timeout: 10000
};

// expose variables
before (async function () {
  //global.expect = expect;
  //global.browser = await puppeteer.launch(opts);
});

// close browser and reset global variables
after (function () {
  //global.browser.close();

});
