'use strict';

import {expect, assert} from 'chai';

import './Modal';

describe('Modal test', () => {
  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-modal id="test-modal" title="Test Modal"></ata-modal></div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should open on attribute and close on button click', () => {
    const modal = document.getElementById('test-modal');

    modal.setAttribute('opened', '');
    expect(modal.shadowRoot.querySelector('.modal').classList.contains('show-modal')).to.be.true;

    document.getElementById('test-modal').querySelector('.closeIcon').dispatchEvent(new Event('click'));
    expect(modal.shadowRoot.querySelector('.modal').classList.contains('show-modal')).to.be.false;

    modal.opened = true;
    expect(modal.shadowRoot.querySelector('.modal').classList.contains('show-modal')).to.be.true;

    modal.opened = false;
    expect(modal.shadowRoot.querySelector('.modal').classList.contains('show-modal')).to.be.false;
    expect(modal.hasAttribute('opened')).to.be.false;
  });
});
