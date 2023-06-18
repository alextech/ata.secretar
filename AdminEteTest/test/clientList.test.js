const puppeteer = require('puppeteer');

const opts = {
  headless: true,
  devtools: true,
  // slowMo: 500,
  timeout: 20000
};

describe('sample test', function () {
  it('should work', function () {
    expect(true).toBeTruthy();
  });
});



describe('Client list section of page', () => {
  it('Should display grid with clients', async () => {
      const browser = await puppeteer.launch(opts);
      const page = await browser.newPage();

      await page.goto('https://localhost:5001/clientlist');

      const numClients = await page.$$('#clientListTable tbody tr td');
      expect(numClients.length).toEqual(20);

      browser.close();
  }, 10000);
});

describe('Profile list section of page', () => {

});
