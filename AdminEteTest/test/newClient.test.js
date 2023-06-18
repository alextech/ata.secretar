const puppeteer = require('puppeteer');

const opts = {
  headless: true,
  devtools: true,
  // slowMo: 500,
  timeout: 20000
};

describe('Client creation', async () => {
  it('Should display and hide new client dialogue', async () => {
      const browser = await puppeteer.launch(opts);
      const page = await browser.newPage();

      await page.goto('https://localhost:5001/clients');

      const newClientButton = await page.$$('#clientListTable tbody tr td');
      expect(numClients.length).toEqual(20);

      browser.close();
  }, 10000);
});

describe('Profile list section of page', () => {

});
