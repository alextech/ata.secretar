import {expect, assert} from 'chai';
import sinon from 'sinon';
import html from 'raw-loader!../../index.html';
import {ClientStore} from "../../investment/clients/index";
import {MeetingController} from "./MeetingController";
import Meeting from "../../investment/Meeting";
import Client from "../../investment/clients/Client";
import '../Modal';
import ProfileStore from "../../investment/profiles/ProfileStore";
import Profile from "../../investment/profiles/Profile";

function setupFixture() {
  let templateNode = document.createElement('div');
  templateNode.innerHTML = html;
  let meetingNode = templateNode.querySelector('#meetingView');

  document.body.insertAdjacentHTML(
    'afterBegin',
    '<div id="fixture"><ata-modal id="modal"></ata-modal><ata-navigation id="navigation"></ata-navigation>'+meetingNode.outerHTML+'</div>'
  );
}

function destroyFixture() {
  document.body.removeChild(document.getElementById('fixture'));
}

describe('MeetingController name fill out client creation', () => {
  beforeEach(() => setupFixture());

  afterEach(() => destroyFixture());

  it('should create primary client on name input entry', () => {
    const clientStore = new ClientStore(),
      ctrl = new MeetingController(clientStore),
      nameField = document.getElementById('primaryClientName'),
      primaryNameChangeEvent = new Event('input');

    nameField.focus();
    nameField.value = 'NewPrimaryClient';
    nameField.dispatchEvent(primaryNameChangeEvent);

    let newClient = clientStore.primaryClient;
    assert.isDefined(newClient);

    expect(newClient).to.not.be.a('null');
    expect(newClient.name).to.be.equal('NewPrimaryClient');
  });

  it('If client already added, deleting all characters still updates the model, but makes disables navigation', () => {
    const clientStore = new ClientStore(),
      ctrl = new MeetingController(clientStore),
      nameField = document.getElementById('primaryClientName'),
      primaryNameChangeEvent = new Event('input');

    nameField.focus();
    nameField.value = 'NewTypoedClient';
    nameField.dispatchEvent(primaryNameChangeEvent);

    let newClient = clientStore.primaryClient;
    expect(newClient).to.not.be.a('null');

    nameField.value = '';
    nameField.dispatchEvent(primaryNameChangeEvent);

    expect(newClient.name).to.be.equal('');

    //@TODO make navigation invalid
  });

  it('should enable email and initiator field when client name is valid', () => {
    const clientStore = new ClientStore(),
      ctrl = new MeetingController(clientStore),
      nameField = document.getElementById('primaryClientName'),
      keyupEvent = new Event('input'),
      emailField = document.getElementById('primaryClientEmail');

    expect(emailField.disabled).to.be.true;

    nameField.focus();
    nameField.value = 'NewTypoedClient';

    nameField.dispatchEvent(keyupEvent);
    expect(emailField.disabled).to.be.false;

    emailField.focus();
    emailField.value = 'email@test.local';
    emailField.dispatchEvent(keyupEvent);
    expect(clientStore.primaryClient.email).to.equal('email@test.local');
  });

  it('should disable root path in menu on invalid fields', () => {
    const meeting = new Meeting(),
      clientStore = new ClientStore(),
      navigation = document.getElementById('navigation'),
      ctrl = new MeetingController(clientStore),
      nameField = document.getElementById('primaryClientName'),
      emailField = document.getElementById('primaryClientEmail'),
      keyupEvent = new Event('input'),

      disableSpy = sinon.spy(navigation, 'disablePath'),
      enableSpy = sinon.spy(navigation, 'enablePath');

    ctrl.activateView(meeting);

    nameField.value = 'client1';
    nameField.dispatchEvent(keyupEvent);
    emailField.value = 'client1@test.local';
    emailField.dispatchEvent(keyupEvent);

    assert(enableSpy.calledOnce);
    assert(enableSpy.calledWith('/meeting'));

    emailField.value = 'client1@test'; //bad email
    emailField.dispatchEvent(keyupEvent);
    assert(disableSpy.calledOnce);
    assert(disableSpy.calledWith('/meeting'));
  });

  it('should make person object as initiator' ,() => {
    const meeting = new Meeting(),
      clientStore = new ClientStore(),
      ctrl = new MeetingController(clientStore),
      nameField = document.getElementById('primaryClientName'),
      jointNameField = document.getElementById('jointClientName'),
      inputEvent = new Event('input');

    ctrl.activateView(meeting);

    nameField.value = 'primary';
    nameField.dispatchEvent(inputEvent);

    jointNameField.value = 'joint';
    jointNameField.dispatchEvent(inputEvent);

    document.querySelector('input[name="isInitiator"][value="primary"]').click();
    expect(clientStore.primaryClient.isInitiator).to.be.true;

    document.querySelector('input[name="isInitiator"][value="joint"]').click();
    expect(clientStore.primaryClient.isInitiator).to.be.false;
    expect(clientStore.jointClient.isInitiator).to.be.true;

    document.querySelector('input[name="isInitiator"][value="advisor"]').click();
    expect(clientStore.jointClient.isInitiator).to.be.false;
    expect(meeting.advisor.isInitiator).to.be.true;


  });

  it('should handle other attendees', () => {
    const meeting = new Meeting(),
      clientStore = new ClientStore(),
      ctrl = new MeetingController(clientStore),
      other_1 = document.getElementById('other_0'),
      jointNameField = document.getElementById('jointClientName'),
      inputEvent = new Event('input');

    ctrl.activateView(meeting);

    other_1.value = 'oth_1';
    other_1.dispatchEvent(inputEvent);
    other_1.value = 'other_1';
    other_1.dispatchEvent(inputEvent);

    expect(meeting.otherAttendees[0]).name = 'other_1';


  });
});

describe('Joint handling', () => {
  beforeEach(() => setupFixture());
  afterEach(() => destroyFixture());

  it('should enable joint fields on selected, disable otherwise', () => {
    const jointClientName = document.getElementById('jointClientName'),
      jointClientEmail = document.getElementById('jointClientEmail'),
      jointIsInitiator = document.getElementById('jointIsInitiator'),
      meeting = new Meeting(),

      clientStore = new ClientStore(),
      ctrl = new MeetingController(clientStore);

    ctrl.activateView(meeting);


    expect(jointClientName.disabled).to.be.true;
    expect(jointClientEmail.disabled).to.be.true;
    expect(jointIsInitiator.disabled).to.be.true;

    document.querySelector('input[name="isJoint"][value="true"]').click();

    expect(jointClientName.disabled).to.be.false;
    expect(jointClientEmail.disabled).to.be.true;
    expect(jointIsInitiator.disabled).to.be.true;

    document.querySelector('input[name="isJoint"][value="false"]').click();

    expect(jointClientName.disabled).to.be.true;
    expect(jointClientEmail.disabled).to.be.true;
    expect(jointIsInitiator.disabled).to.be.true;

  });

  it('should add profiles in correct order', () => {
    const primaryClientName = document.getElementById('primaryClientName'),
      jointClientName = document.getElementById('jointClientName'),
      meeting = new Meeting(),

      clientStore = new ClientStore(),
      ctrl = new MeetingController(clientStore);

    ctrl.activateView(meeting);

    document.querySelector('input[name="isJoint"][value="true"]').click();

    jointClientName.value = 'first_joint';
    jointClientName.dispatchEvent(new Event('input'));

    expect(clientStore.primaryClient).to.be.undefined;

    primaryClientName.value = 'second_primary';
    primaryClientName.dispatchEvent(new Event('input'));

    expect(clientStore.primaryClient).to.be.instanceOf(Client).that.has.property('name').that.equals('second_primary');
    expect(clientStore.jointClient).to.be.instanceOf(Client).that.has.property('name').that.equals('first_joint');
  });

  it('should prompt about switch to individual if joint entity was created in store', () => {
    const primaryClientName = document.getElementById('primaryClientName'),
      primaryEmailField = document.getElementById('primaryClientEmail'),
      jointClientName = document.getElementById('jointClientName'),
      jointClientEmail = document.getElementById('jointClientEmail'),
      jointIsInitiator = document.getElementById('jointIsInitiator'),

      keyEvent = new Event('input'),

      meeting = new Meeting(),
      clientStore = new ClientStore(),
      profileStore = new ProfileStore(),
      ctrl = new MeetingController(clientStore, profileStore);

    ctrl.activateView(meeting);

    document.querySelector('input[name="isJoint"][value="true"]').click();

    primaryClientName.value = 'first primary';
    primaryEmailField.value = 'test@testrunner.local';
    primaryClientName.dispatchEvent(keyEvent);

    jointClientName.value = 'I will be deleted';
    jointClientName.dispatchEvent(new Event('input'));

    expect(clientStore.primaryClient).to.be.instanceOf(Client).that.has.property('name').that.equals('first primary');
    expect(clientStore.jointClient).to.be.instanceOf(Client).that.has.property('name').that.equals('I will be deleted');


    // ============ now disable join ============================
    // ============== decide not to go back to individual ==============
    document.querySelector('input[name="isJoint"][value="false"]').click();
    expect(document.querySelector('ata-modal').hasAttribute('opened')).to.be.true;

    document.querySelector('#modal input[value="No"]').click();
    expect(jointClientName.disabled).to.be.false;
    expect(jointClientName.value).to.not.equal('');
    expect(clientStore.jointClient).to.not.be.null;


    profileStore.addProfile(new Profile('joint', [], clientStore.primaryClient, clientStore.jointClient));
    expect(profileStore.profiles[0].name).to.equal('joint');

    // ========== switch back to individual =========
    document.querySelector('#modal input[value="Yes"]').click();
    expect(jointClientName.disabled).to.be.true;
    expect(jointClientName.value).to.equal('');
    expect(jointClientEmail.value).to.equal('');
    expect(clientStore.jointClient).to.be.undefined;
    expect(profileStore.profiles).to.be.empty;
  });
});
