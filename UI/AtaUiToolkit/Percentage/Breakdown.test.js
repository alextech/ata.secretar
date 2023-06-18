'use strict';

import {expect} from 'chai';

import Breakdown from "./Breakdown";

describe('Breakdown component test', () => {
  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture">' +
      '<ata-percentage-breakdown id="testBreakdown">' +
      '<ata-numeric name="field_1">' +
      '<ata-numeric name="field_2">' +
      '</ata-percentage-breakdown>' +
      '</div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should update total field', () => {
    const breakdownElement = document.getElementById('testBreakdown'),
      keyupEvent = new KeyboardEvent('input', {bubbles: true, composed: true});

    let input = breakdownElement.querySelector('ata-numeric[name="field_1"]');
    input.value = 33;
    input.dispatchEvent(keyupEvent);

    const total = breakdownElement.shadowRoot.querySelector('ata-numeric[name="total"]');
    expect(parseInt(total.value)).to.equal(33);

    input = breakdownElement.querySelector('ata-numeric[name="field_2"]');
    input.value = 20;
    input.dispatchEvent(keyupEvent);

    expect(parseInt(total.value)).to.equal(53);
  });

  it('should dispatch validation change event when total changes to/from 100', () => {
    const breakdownElement = document.getElementById('testBreakdown'),
      keyupEvent = new KeyboardEvent('input', {bubbles: true, composed: true});

    let input;
    let valid = undefined;
    breakdownElement.addEventListener('validityChanged', e => {
      valid = e.detail.valid;
    });

    input = breakdownElement.querySelector('input[name="field_1"]');
    input.value = 60;
    input = breakdownElement.querySelector('input[name="field_2"]');
    input.value = 40;
    input.dispatchEvent(keyupEvent);

    expect(valid).to.not.be.undefined;
    expect(valid).to.be.true;

    input.value = 30;
    input.dispatchEvent(keyupEvent);
    expect(valid).to.be.false;

  });

  it('on field disable should update total and validation', () => {
    const breakdownElement = document.getElementById('testBreakdown');
    let input = breakdownElement.querySelector('input[name="field_2"]');
    input.value = 60;
    input = breakdownElement.querySelector('input[name="field_1"]');
    input.value = 40;
    expect(breakdownElement.isValid).to.be.true;

    let validationWasCalled = false;
    const validityChangeStub = function(e) {
      validationWasCalled = true;
      expect(e.detail.valid).to.be.false;
    };
    breakdownElement.addEventListener('validityChanged', validityChangeStub);

    input.setAttribute('disabled', 'true');
    breakdownElement.removeEventListener('validityChanged', validityChangeStub);

    //TODO mutation listener happens asynchronously so currently no way
    // to reliably find if validation obsrever was called
    // expect(validationWasCalled).to.be.true;
  });
});
