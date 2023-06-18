'use strict';
import chai, {assert} from 'chai';
import chaiString from 'chai-string';
import './index';

const expect = chai.use(chaiString).expect;

describe('Client sub-navigation test', () => {
  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-navigation-client id="test-client"></ata-navigation-client></div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should know client related attributes', async () => {
    const clientNode = document.getElementById('test-client'),
      client = {id: 22, name:'FirstTestClient', email: 'test1@test.local'};

    clientNode.setAttribute('client-id', client.id);
    clientNode.setAttribute('client-name', client.name);

    await new Promise(resolve => setTimeout(resolve, 0));
    expect(parseInt(clientNode.getAttribute('client-id'))).to.equal(client.id, 'get client ID using attribute');
    expect(clientNode.getAttribute('client-name')).to.equal(client.name, 'get client name using attribute');

    expect(parseInt(clientNode.clientID)).to.equal(client.id, 'get client ID using getter');
    expect(clientNode.clientName).to.equal(client.name, 'get client name using getter');

    clientNode.clientName = 'changed using attribute';
    expect(clientNode.getAttribute('client-name')).to.equal('changed using attribute');
  });

  it('should put client name into a->span tag', () => {
    const clientNode = document.getElementById('test-client'),
      client = {id: 'stub-client-id', name:'FirstTestClient', email: 'test1@test.local'};

    clientNode.clientName = client.name;
    const clientNameNode = clientNode.shadowRoot.querySelector('.nav-client-name');
    expect(clientNameNode.innerHTML).to.equal(client.name);
  });

  it('should put client id into top subroot link and info href with base url', () => {
    const clientNode = document.getElementById('test-client'),
      client = {id: 'stub-client-id', name:'FirstTestClient', email: 'test1@test.local'};

    clientNode.baseUrl = '/meeting/u-test';
    clientNode.clientName = client.name;
    clientNode.clientID = client.id;
    const subrootNode = clientNode.shadowRoot.querySelector('a'),
      infoNode = clientNode.shadowRoot.querySelector('ul > li > a');
    expect(subrootNode.href).to.endsWith('/meeting/u-test/client/'+client.id+'/info');
    expect(infoNode.href).to.endsWith('/meeting/u-test/client/'+client.id+'/info');
  });

  it('should set base-url to profile children, assembled from own base-url plus client-id', async () => {
    const clientNode = document.getElementById('test-client'),
      profileNode = document.createElement('ata-navigation-profile');

    clientNode.baseUrl = '/meeting/u-test';
    clientNode.clientID = 'c-id';

    clientNode.appendChild(profileNode);
    await new Promise(resolve => setTimeout(resolve, 0));
    expect(profileNode.getAttribute('base-url')).to.equal('/meeting/u-test/client/c-id');
  });

  it('should collapse/expand submenu on opened/closed attribute change', () => {

  });

  it('should add active class to first node on info route', () => {
    const clientNode = document.getElementById('test-client');
    clientNode.clientId = 'selectiontest';

    clientNode.activate(['info']);
    const infoNode = clientNode.shadowRoot.querySelector('ul > li > a');
    expect(infoNode.classList.contains('active')).to.be.true;
  });

  it('should stop click events when disabled', () => {
    const clientNode = document.getElementById('test-client');
    clientNode.clientId = 'clickTest';
    clientNode.disabled = true;


    clientNode.shadowRoot.querySelector('a').click();

    expect(window.location.hash).to.not.endsWith('#/client/clickTest');
  });

  it('should add list item for profile added', async () => {
    const client_1_navnode = document.getElementById('test-client'),
      profile_1_navnode = document.createElement('ata-navigation-profile'),
      profile_2_navnode = document.createElement('ata-navigation-profile');

    profile_1_navnode.setAttribute('profile-id', 'profile-1');
    profile_1_navnode.setAttribute('profile-name', 'first-profile');
    client_1_navnode.appendChild(profile_1_navnode);

    profile_2_navnode.setAttribute('profile-id', 'profile-2');
    profile_2_navnode.setAttribute('profile-name', 'second-profile');
    client_1_navnode.appendChild(profile_2_navnode);

    await new Promise(resolve => setTimeout(resolve, 0));
    let profileSlotNode = client_1_navnode.shadowRoot.querySelector('ul > li:nth-child(2) > slot');
    assert.isNotNull(profileSlotNode);
    expect(profileSlotNode.name).to.equal(profile_1_navnode.slot);

    profileSlotNode = client_1_navnode.shadowRoot.querySelector('ul > li:nth-child(3) > slot');
    assert.isNotNull(profileSlotNode);
    expect(profileSlotNode.name).to.equal(profile_2_navnode.slot);
  });

});
