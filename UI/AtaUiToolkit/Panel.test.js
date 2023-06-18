'use strict';

import chai from 'chai';
import chaiString from 'chai-string';
import './Panel';

const expect = chai.use(chaiString).expect;

describe('Panel test', () => {
  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-panel id="test-panel" data-title="Initial Title"></ata-panel></div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should modify title on title attribute change', () => {
    const panel = document.getElementById('test-panel');
    expect(panel.shadowRoot.querySelector('.title span').innerHTML).to.equal('Initial Title');
    panel.dataset.title = 'Alternate Title';

    expect(panel.shadowRoot.querySelector('.title span').innerHTML).to.equal('Alternate Title');
  });

});
