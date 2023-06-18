'use strict';

import html from 'raw-loader!../../index.html';
import chai, {assert} from 'chai';
import chaiString from "chai-string";
import sinon from 'sinon';

import '../../Numeric';
import {ClientInfoController} from './ClientInfoController';
import '../../questionnaire/Modal';
import '../../navigation/Navigation';
import Client from "../../investment/clients/Client";

const expect = chai.use(chaiString).expect;

function setupFixture() {
  document.body.insertAdjacentHTML(
    'afterBegin',
    '<div id="fixture">'+html+'</div>'
  );
}

function destroyFixture() {
  document.body.removeChild(document.getElementById('fixture'));
}

describe('ClientInfoController synchs with passed client object', () => {
  beforeEach(() => setupFixture());

  afterEach(() => destroyFixture());

  it('should update household income on client income change', () => {
    const client = new Client('testClient', 'clientInfo@controllers.test'),
      controller = new ClientInfoController(),
      annualIncome = document.getElementById('annualIncome'),
      householdIncome = document.getElementById('householdIncome');

    controller.activateView(client);
    annualIncome.value = 105;
    annualIncome.dispatchEvent(new Event('input', {bubbles: true, composed: true}));
    expect(parseInt(householdIncome.value)).to.equal(105);
    controller.deactivateView();
  });

  it('should update networth income on client change', () => {
    const client = new Client('testClient', 'clientInfo@controllers.test'),
      controller = new ClientInfoController(),
      fixedAssets = document.getElementById('fixedAssets'),
      liquidAssets = document.getElementById('liquidAssets'),
      liabilities = document.getElementById('liabilities'),
      total = document.getElementById('networthTotal'),

      inputEvent = new Event('input', {bubbles: true, composed: true});

    controller.activateView(client);
    fixedAssets.value = 50;
    fixedAssets.dispatchEvent(inputEvent);
    liquidAssets.value = 40;
    liquidAssets.dispatchEvent(inputEvent);
    liabilities.value = 10;
    liabilities.dispatchEvent(inputEvent);
    expect(parseInt(total.value)).to.equal(80);

    const networth = client.networth;
    expect(networth.fixedAssets).to.equal(50);
    expect(networth.liquidAssets).to.equal(40);
    expect(networth.liabilities).to.equal(10);
    controller.deactivateView();
  });

  it('should not navigate away without confirmation if networth is 0', () => {
    const client = new Client('testClient', 'clientInfo@controller.test'),
      controller = new ClientInfoController();

    controller.activateView(client);
    window.location.hash = '#/stub';

    document.getElementById('navigation').shadowRoot.querySelector('nav ul li:first-child a span').click();

    expect(window.location.hash).to.not.equal('');
    const modal = document.getElementById('modal');
    expect(modal.hasAttribute('opened')).to.be.true;
    modal.querySelector('.options input[value="No"]').click();
    expect(window.location.hash).to.equal('#/stub');
    expect(modal.hasAttribute('opened')).to.be.false;

    document.getElementById('navigation').shadowRoot.querySelector('nav ul li:first-child a span').click();
    modal.querySelector('.options input[value="Yes"]').click();
    expect(window.location.hash).to.equal('');
    expect(modal.hasAttribute('opened')).to.be.false;
  });

  // SINON not running assertions :(
  // it('should disable path towards client on invalid fields', () => {
  //   const navigation = document.getElementById('navigation'),
  //     clientNavNode = document.createElement('ata-navigation-client'),
  //     client = new Client('ClientInfoCtrl', 'client@infoController.test'),
  //     ctrl = new ClientInfoController(),
  //
  //     disableSpy = sinon.spy(navigation, 'disablePath'),
  //     enableSpy = sinon.spy(navigation, 'enablePath');
  //
  //   clientNavNode.clientID = client.id;
  //   navigation.appendChild(clientNavNode);
  //
  //   ctrl.activateView(client);
  //   const dateOfBirthField = document.getElementById('dateOfBirth');
  //   dateOfBirthField.value = 'July 14, 1989';
  //   // dateOfBirthField.dispatchEvent(new Event('input'));
  //
  //   assert(enableSpy.calledOnce);
  //   assert(enableSpy.calledWith('/client/'+client.id));
  // });
});
