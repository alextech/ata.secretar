'use strict';
import {expect} from 'chai';

import Profile, {parseAccounts} from "./Profile";
import Client from "../clients/Client";

describe('Profile Entity test', () => {
  it('should return observable proxy for objectives with Total', () => {
    const profile = new Profile('', [], {}),
      objectives = profile.objectives;

    let wasCalled = false;

    const changeNotification = function(objectives) {
      wasCalled = true;
    };
    objectives.addEventListener('*Changed', changeNotification);
    objectives.aggressiveGrowth = 50;

    // verify change called when subscribed
    expect(wasCalled).to.be.true;

    //verify change NOT called when subscription REMOVED
    wasCalled = false;
    objectives.removeEventListener('*Changed', changeNotification);
    objectives.cashReserve = 25;
    expect(wasCalled).to.be.false;
  });

  it('should recognize it is joint or not', () => {
    const p1 = new Profile('single-test', [], {name: 'primary'});
    expect(p1.isJoint).to.be.false;

    const p1_1 = new Profile('sing-test-with-null', [], {name: 'primary'}, null);
    expect(p1_1.isJoint).to.be.false;

    const p2 = new Profile('joint-test', [], {name: 'primary'}, {name: 'joint'});
    expect(p2.isJoint).to.be.true;
  });

  it('should calculate income if PROFILE is joint, NOT client', () => {
    const c1 = new Client(),
      c2 = new Client();
    c1.annualIncome = 75000;
    c2.annualIncome = 85000;
    c1.jointWith = c2;

    const p1 = new Profile('single-test', [], c1);
    expect(p1.incomeScore).to.equal(4);
    const p2 = new Profile('joint-test', [], c1, c2);
    expect(p2.incomeScore).to.equal(14);

  });
});

describe('Profile fields parser test', () => {

  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<form id="fixture" name="fixture">' +
      '<input type="checkbox" name="accounts[]" value="acc_1" checked>' +
      '<input type="checkbox" name="accounts[]" value="acc_2">' +
      '<input type="checkbox" name="accounts[]" value="acc_3" checked>' +
      '</form>'
    )
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should return map of accounts', () => {
    const chkBoxes = document.forms['fixture'].elements['accounts[]'];
    const accounts = parseAccounts(chkBoxes);

    expect(accounts).to.be.an('array').that.includes.members([
      "acc_1",
      "acc_3"
    ]);
  });
});
