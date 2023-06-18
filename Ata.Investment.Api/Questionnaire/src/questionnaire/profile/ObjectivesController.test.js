import html from 'raw-loader!../../index.html';
import '../percentage/Breakdown';
import '../../navigation/Navigation';
import '../../navigation/Client';
import {ObjectivesController} from "./ObjectivesController";
import chai from "chai";
import chaiString from "chai-string";
import Profile from "../../investment/profiles/Profile";

const expect = chai.use(chaiString).expect;

describe('Objectives Controller Test', () => {

  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture">' +
      '' + html + '</div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should disable growth fields when time horizon is less than 3 years', () => {
    const objController = new ObjectivesController();
    objController.activateView(new Profile('objectives test mock', [], {}));
    document.getElementById('time_under_3').click();

    expect(document.forms['profile_objectives'].elements['aggGrowth'].hasAttribute('disabled')).to.be.true;
    expect(document.forms['profile_objectives'].elements['growth'].hasAttribute('disabled')).to.be.true;
    expect(document.getElementById('modal').hasAttribute('opened')).to.be.false;
  });

  it('should confirm < 3 year selection if growth has a value', () => {
    const objController = new ObjectivesController(),
      form = document.forms['profile_objectives'];

    objController.activateView(new Profile('objectives test mock', [], {}));


    // =============== pick yes
    form.elements['aggGrowth'].value = 1;
    document.getElementById('time_under_3').click();

    expect(document.getElementById('modal').hasAttribute('opened')).to.be.true;
    document.querySelector('#modal input[value="Yes"]').click();

    expect(document.getElementById('modal').hasAttribute('opened')).to.be.false;
    expect(parseInt(form.elements['aggGrowth'].value)).to.equal(0);
    expect(document.getElementById('time_under_3').checked).to.be.true;

    document.getElementById('time_3_to_7').click();
    document.getElementById('time_3_to_7').checked = false;
    // ============== pick no with no existing radio
    form.elements['growth'].value = 1;
    document.getElementById('time_under_3').click();
    expect(document.getElementById('modal').hasAttribute('opened')).to.be.true;
    document.querySelector('#modal input[value="No"]').click();
    expect(document.forms['profile_objectives'].elements['aggGrowth'].hasAttribute('disabled')).to.be.false;
    expect(document.forms['profile_objectives'].elements['growth'].hasAttribute('disabled')).to.be.false;
    expect(document.getElementById('time_under_3').checked).to.be.false;
    expect(document.getElementById('modal').hasAttribute('opened')).to.be.false;

    document.getElementById('time_3_to_7').click();
    // ============= pick with existing radio
    form.elements['growth'].value = 1;
    document.getElementById('time_under_3').click();
    expect(document.getElementById('modal').hasAttribute('opened')).to.be.true;
    document.querySelector('#modal input[value="No"]').click();
    expect(document.forms['profile_objectives'].elements['aggGrowth'].hasAttribute('disabled')).to.be.false;
    expect(document.forms['profile_objectives'].elements['growth'].hasAttribute('disabled')).to.be.false;
    expect(document.getElementById('time_3_to_7').checked).to.be.true;
    expect(document.getElementById('time_under_3').checked).to.be.false;
    expect(document.getElementById('modal').hasAttribute('opened')).to.be.false;
  });

  // unit test not working. Breakdown cant query elements for some reason at this point.
  // it('should fully reset time', () => {
  //   const objController = new ObjectivesController();
  //
  //   objController.activateView(new Profile('objectives test mock', [], {}));
  //   document.forms['profile_objectives'].elements['aggGrowth'].value = 100;
  //   document.forms['profile_objectives'].elements['aggGrowth'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
  //   document.forms['profile_objectives'].elements['high'].value = 100;
  //   document.forms['profile_objectives'].elements['high'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
  //   document.getElementById('time_3_to_7').click();
  //
  //   expect(document.querySelector('form[name="profile_objectives"] a.next').classList.contains('disabled')).to.be.false;
  //
  // });

});
