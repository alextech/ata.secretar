'use strict';
import {expect} from 'chai';
import './Numeric';

describe('Numeric currency type', () => {

  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<ata-numeric type="currency" id="testInput">'
    );
  });

  // it('should keep track of dispalay vs raw value', () => {
  //   const input = document.querySelector('#testInput input');
  //   input.setAttribute('data-rawValue', '1000');
  //   expect(input.value).to.equal('1,000');
  //
  //   input.value = '10,000';
  //   expect(parseInt(input.getAttribute('data-rawValue'))).to.equal(10000);
  // });

  it('should show raw value on focus, display value on leave.', () => {
    const input = document.querySelector('#testInput input');
    input.dispatchEvent(new Event('focus'));
    input.value = 1000000;
    input.dispatchEvent(new Event('blur'));
    expect(input.value).to.equal('1,000,000');
    input.dispatchEvent(new Event('focus'));
    expect(parseInt(input.value)).to.equal(1000000);
  })
});
