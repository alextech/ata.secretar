import '../../navigation/Navigation';
import '../../navigation/Client';
import html from 'raw-loader!../../index.html';
import NewProfileController from "./NewProfileController";
import {ProfileStore} from "../../investment/profiles";
import {ClientStore} from "../../investment/clients";
import Client from "../../investment/clients/Client";
import chai from "chai";
import chaiString from "chai-string";
import {ProfileAddedEvent} from "../../investment/profiles/ProfileStore";

const expect = chai.use(chaiString).expect;

"../../investment/profiles/ProfileStore";

function fillForm(form) {
  const addBtn = document.querySelector('#addProfileBtn');

  form.elements['profileName'].value = 'New Test Profile';
  form.elements['profileName'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
  form.elements['initialInvestment'].value = 5;
  form.elements['initialInvestment'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
  form.elements['monthlyCommitment'].value = 10;
  form.elements['monthlyCommitment'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));

  form.elements['accounts[]'][3].click();
  addBtn.click();
}

describe('New Profile dialogue', () => {

  beforeEach(() => {
    let templateNode = document.createElement('div');
    templateNode.innerHTML = html;
    let newProfileNode = templateNode.querySelector('#newProfileTemplate');
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-modal title="Add Joint Profile" class="modal-content" id="modal"></ata-modal>' +
      '<ata-navigation id="navigation"><ata-navigation-client client-id="new-profile-fixture-1"></ata-navigation-client></ata-navigation>'+newProfileNode.outerHTML+'</div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should enable add button on valid form', () => {
    const profileStore = new ProfileStore(),
      clientStore = new ClientStore(),
      clientMock = {
        id: 'new-profile-fixture-1'
      };
    clientStore.addClient(clientMock);
    const newProfileController = new NewProfileController(profileStore, clientStore);
    newProfileController.activateView({
      addForClients: [clientStore.primaryClient.id],
      isJoint: clientStore.jointClient !== null
    });

    const newProfileLink = document.querySelector('ata-navigation-client[client-id="new-profile-fixture-1"]').shadowRoot
      .querySelector('ul').lastElementChild.firstElementChild;
    newProfileLink.removeAttribute('data-disabled');
    newProfileLink.click();

    const form = document.forms['newProfile'],
      addBtn = document.querySelector('#addProfileBtn');

    expect(addBtn.hasAttribute('disabled'), 'initially invalid').to.be.true;
    form.elements['profileName'].value = 'Name Filled';
    form.elements['profileName'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
    expect(addBtn.hasAttribute('disabled')).to.be.true;
    form.elements['initialInvestment'].value = 5;
    form.elements['initialInvestment'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
    expect(addBtn.hasAttribute('disabled')).to.be.true;
    form.elements['monthlyCommitment'].value = 10;
    form.elements['monthlyCommitment'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
    expect(addBtn.hasAttribute('disabled')).to.be.true;

    // select one account
    form.elements['accounts[]'][1].click();
    expect(addBtn.hasAttribute('disabled'), 'at least one account selected').to.be.false;
    // select another account
    form.elements['accounts[]'][3].click();
    // unselect one of 2 accounts. At least one is still selected so should be valid
    form.elements['accounts[]'][1].click();
    expect(addBtn.hasAttribute('disabled'), 'one of accounts unselected, but still at least one').to.be.false;
    // unselect other account. Should become invalid
    form.elements['accounts[]'][3].click();
    expect(addBtn.hasAttribute('disabled'), 'all accounts unselected').to.be.true;
  });

  it('should add profile to profile store on add click', () => {
    const profileStore = new ProfileStore(),
      clientStore = new ClientStore(),
      newProfileController = new NewProfileController(profileStore, clientStore);

    const clientMock = {
      id: 'new-profile-fixture-1'
    };
    clientStore.addClient(clientMock);

    newProfileController.activateView({
      addForClients: [clientStore.primaryClient.id],
      isJoint: clientStore.jointClient !== null
    });

    const newProfileLink = document.querySelector('ata-navigation-client[client-id="new-profile-fixture-1"]').shadowRoot
      .querySelector('ul').lastElementChild.firstElementChild;
    newProfileLink.removeAttribute('data-disabled');
    newProfileLink.click();

    // newProfileController.initTemplate(false);
    const form = document.forms['newProfile'],
      addBtn = document.querySelector('#addProfileBtn');

    form.elements['profileName'].value = 'New Test Profile';
    form.elements['profileName'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
    form.elements['initialInvestment'].value = 5;
    form.elements['initialInvestment'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
    form.elements['monthlyCommitment'].value = 10;
    form.elements['monthlyCommitment'].dispatchEvent(new Event('input', {bubbles: true, composed: true}));
    form.elements['accounts[]'][3].click();

    let profileID = undefined;
    profileStore.addEventListener('profileAdded', (e) => {
      expect(e).to.have.property('details').that.has.property('profile');
      const profile = e.details.profile;
      expect(profile.name).to.equal('New Test Profile');

      profileID = profile.id;
      expect(profile.primaryClient.id).to.equal('new-profile-fixture-1');
    });

    addBtn.click();

    expect(profileID).to.not.be.undefined;

    const profile = document.querySelector(
      `ata-navigation-client[client-id="new-profile-fixture-1"] ata-navigation-profile[profile-id="${profileID}"]`
    );
    expect(profile).to.not.be.undefined;
  });

  it('should open profile on creation', () => {
    const profileStore = new ProfileStore(),
      clientStore = new ClientStore(),
      newProfileController = new NewProfileController(profileStore, clientStore);


    let lastAddedProfileID = 0;
    profileStore.addEventListener(ProfileAddedEvent, e => lastAddedProfileID = e.details.profile.id);

    clientStore.addClient({id: 'new-profile-fixture-1'});
    clientStore.addClient({id: 'new-profile-fixture-2'});
    newProfileController.activateView({
      addForClients: [clientStore.primaryClient.id],
      isJoint: false
    });

    fillForm(document.forms['newProfile']);
    expect(window.location.hash).to.endsWith(`#/client/new-profile-fixture-1/profile/${lastAddedProfileID}/objectives`);

    newProfileController.activateView({
      addForClients: [clientStore.primaryClient.id, clientStore.jointClient.id],
      isJoint: true
    });
    fillForm(document.forms['newProfile']);
    expect(window.location.hash).to.endsWith(`#/profile/${lastAddedProfileID}/objectives`);
  });
});
