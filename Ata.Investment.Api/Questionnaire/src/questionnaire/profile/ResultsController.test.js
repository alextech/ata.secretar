import html from 'raw-loader!../../index.html';
import {ResultsController} from "./ResultsController";
import chai from "chai";
import chaiString from "chai-string";
import {createProfileFixture} from "../../../buildScripts/fixtures";
import '../../questionnaire/Modal';

import fetchMock from 'fetch-mock';

const expect = chai.use(chaiString).expect;

const mockPortfolioRisk = {
  "yearlyPercentReturns": {
    "labels": [
      2011,
      2012,
      2013,
      2014,
      2015,
      2016,
      2017,
      2018
    ],
    "datasets": [
      {
        "offset": 0,
        "backgroundColor": [
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff",
          "red"
        ],
        "data": [
          3.2934131736526946,
          7.9227053140096615,
          4.7847806624888092,
          6.6273027546764229,
          2.4711241451424861,
          3.6018156866884832,
          1.9182333685808244,
          -1.4662320081283713
        ]
      }
    ]
  },
  "returns": {
    "labels": [
      2010,
      2011,
      2012,
      2013,
      2014,
      2015,
      2016,
      2017,
      2018
    ],
    "datasets": [
      {
        "offset": 12000,
        "backgroundColor": [
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff",
          "#007bff"
        ],
        "data": [
          182,
          1817,
          4168,
          6182,
          8631,
          10350,
          12383,
          14070,
          14501
        ]
      }
    ]
  },
  "compoundReturns": {
    "labels": [
      "Week",
      "Month",
      "Three Months",
      "Six Months",
      "Year",
      "Three Years",
      "Five Years",
      "Inception"
    ],
    "datasets": [
      {
        "showLine": true,
        "label": "Total Amount",
        "backgroundColor": "#007bff",
        "data": [
          11995,
          12096,
          12157,
          12517,
          12988,
          16004,
          20287,
          27107
        ]
      }
    ]
  }
};

const mockAllocation = {
  "name": "string",
  "version": 0,
  "scoreRange": {
    "min": 0,
    "max": 0
  },
  "options": [
    {
      "compositionNumber": 0,
      "name": "Some Fund Option",
      "compositionParts": [
        {
          "percent": 0,
          "portfolio": "СIG 1111"
        }
      ]
    },
    {
      "compositionNumber": 0,
      "name": "Some Fund Option",
      "compositionParts": [
        {
          "percent": 0,
          "portfolio": "СIG 1111"
        }
      ]
    }
  ]
};

describe('Results Controller Test', () => {
  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><div id="sub-nav"></div> ' +
      '' + html + '</div>'
    );

    fetchMock.reset();
    fetchMock.get('end:/portfolio/CIG4241/risk?initial=0',
        mockPortfolioRisk
    );

    fetchMock.get('end:/portfolio/CIG90107/risk?initial=0',
        mockPortfolioRisk
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });


  it('should sync with profile on view leave and button click', () => {
    const resultsController = new ResultsController(),
      form = document.forms['classSelection'];

    const profile = createProfileFixture();

    resultsController.activateView(profile);

    form.elements['useClassF'].click();
    resultsController.deactivateView();
    expect(profile.useClassF).to.be.true;
    expect(profile.useClassP).to.be.false;

    profile.useClassP = true;
    resultsController.activateView(profile);
    expect(form.elements['useClassP'].checked).to.be.true;

    // form.elements['useClassF'].click();
    // form.elements['useClassF'].checked = false;
    // document.getElementById('submitProfile').click();
    //TODO verify this does not mess API
    // expect(profile.useClassF).to.be.false;
  });

  it('should add allocation options', async () => {
    // setup
    const resultsController = new ResultsController();
    const profile = createProfileFixture();
    await profile.calculatePlan(); // to trigger score generation
    fetchMock.get('end:/allocations/?score='+profile.score,
      mockAllocation
    );

    // act
    await resultsController.activateView(profile);

    // verify
    const allocationCharts = document.querySelectorAll('ata-allocation-chart');
    expect(allocationCharts.length).to.equal(2);
  });

  it('should clear previous allocation options', async () => {
    // setup
    const resultsController = new ResultsController();
    const profile = createProfileFixture();
    await profile.calculatePlan(); // to trigger score generation
    fetchMock.get('end:/allocations/?score='+profile.score,
      mockAllocation
    );

    // act
    await resultsController.activateView(profile);
    mockAllocation.options.pop();
    fetchMock.get('end:/allocations/?score='+profile.score,
      mockAllocation, {overwriteRoutes: true}
    );
    await resultsController.activateView(profile);
    // verify
    const allocationCharts = document.querySelectorAll('ata-allocation-chart');
    expect(allocationCharts.length).to.equal(1);
  });

  it('should save composition to profile instance', () => {
    const resultsController = new ResultsController();

    const profile = createProfileFixture();

    resultsController.activateView(profile);

    // fetchMock.get('end:/portfolio/CIG4241/alternative_compositions/1',
    //   mockAllocation
    // );



  });
});
