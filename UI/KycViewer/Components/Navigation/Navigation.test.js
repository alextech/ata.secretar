'use strict';
import chai, {assert} from 'chai';
import chaiString from 'chai-string';

import './index';

const expect = chai.use(chaiString).expect;

describe('Navigation tree test', () => {
  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-navigation id="test-navigation"></ata-navigation></div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should carry base-url to all children', async () => {
    const navigation = document.getElementById('test-navigation'),
        client_1_navnode = document.createElement('ata-navigation-client'),
        profile_1_navnode = document.createElement('ata-navigation-profile'),
        profile_j_navnode = document.createElement('ata-navigation-profile');

    client_1_navnode.setAttribute('client-id', 'client-1');
    profile_1_navnode.setAttribute('profile-id', 'profile-1');
    profile_j_navnode.setAttribute('profile-id', 'profile-j');

    client_1_navnode.appendChild(profile_1_navnode);

    navigation.setAttribute('base-url', '/meeting/m-id');

    navigation.appendChild(client_1_navnode);
    client_1_navnode.appendChild(profile_1_navnode);
    navigation.appendChild(profile_j_navnode);

    await new Promise(resolve => setTimeout(resolve, 0));

    expect(client_1_navnode.getAttribute('base-url')).to.equal('/meeting/m-id');
    expect(profile_1_navnode.getAttribute('base-url')).to.equal('/meeting/m-id/client/client-1');
    expect(profile_j_navnode.getAttribute('base-url')).to.equal('/meeting/m-id');
  });

  it('should have base url in /meeting level href', async () => {
    const navigation = document.getElementById('test-navigation');

    navigation.setAttribute('base-url', '/meeting/m-id');

    expect(navigation.shadowRoot.querySelector('nav > ul > li > a').href).to.endsWith('/meeting/m-id');
  });

  it('should add list item for client added', async () => {
    const navigation = document.getElementById('test-navigation'),
      client_1_navnode = document.createElement('ata-navigation-client'),
      client_2_navnode = document.createElement('ata-navigation-client'),

      client_1 = {id: 'client_id_1', name:'FirstTestClient', email: 'test1@test.local'},
      client_2 = {id: 'client_id_2', name: 'SecondTestClient', email: 'test2@test.local'};

    client_1_navnode.setAttribute('client-id', client_1.id);
    client_1_navnode.setAttribute('client-name', client_1.name);
    navigation.appendChild(client_1_navnode);

    client_2_navnode.setAttribute('client-id', client_2.id);
    client_2_navnode.setAttribute('client-name', client_2.name);
    navigation.appendChild(client_2_navnode);

    await new Promise(resolve => setTimeout(resolve, 0));

    let clientSlotNode = navigation.shadowRoot.querySelector('ul > li:nth-child(2) > slot');
    assert.isNotNull(clientSlotNode);
    expect(clientSlotNode.name).to.equal(client_1_navnode.slot);

    clientSlotNode = navigation.shadowRoot.querySelector('ul > li:nth-child(3) > slot');
    assert.isNotNull(clientSlotNode);
    expect(clientSlotNode.name).to.equal(client_2_navnode.slot);
  });

  it('should add list item for joint profile added', async () => {
    const navigation = document.getElementById('test-navigation'),
      profile_navnode = document.createElement('ata-navigation-profile');

    navigation.appendChild(profile_navnode);

    await new Promise(resolve => setTimeout(resolve, 0));

    const profileSlotNode = navigation.shadowRoot.querySelector('ul.joint-submenu li:first-child > slot');
    assert.isNotNull(profileSlotNode);
    expect(profileSlotNode.name).to.equal(profile_navnode.slot);
  });

  it('should enable joint profile subnav when multiple clients', async () => {
    const navigation = document.getElementById('test-navigation'),
      jointProfilesNode = navigation.shadowRoot.querySelector('nav').firstElementChild.lastElementChild;

    expect(jointProfilesNode.style.display).to.equal('none');

    const c1Node = document.createElement('ata-navigation-client');
    navigation.appendChild(c1Node);
    await new Promise(resolve => setTimeout(resolve, 0));
    expect(jointProfilesNode.style.display).to.equal('none');

    const c2Node = document.createElement('ata-navigation-client');
    navigation.appendChild(c2Node);
    await new Promise(resolve => setTimeout(resolve, 0));
    expect(jointProfilesNode.style.display).to.equal('');

    navigation.removeChild(c2Node);
    await new Promise(resolve => setTimeout(resolve, 0));
    expect(jointProfilesNode.style.display).to.equal('none');
  });

  it('should close/open subnavs on supplied path', () => {
    const navigation = document.getElementById('test-navigation'),
      c1Node = document.createElement('ata-navigation-client');

    c1Node.setAttribute('client-id', '1c');
    navigation.appendChild(c1Node);

    let path = [];
    path.push('1c');
    path.push('client');

    navigation.activate(path);

    const c2Node = document.createElement('ata-navigation-client');
    c2Node.setAttribute('client-id', '2c');
    navigation.appendChild(c2Node);

    path = [];
    path.push('2c');
    path.push('client');

    navigation.activate(path);

    expect(c1Node.opened).to.be.false;
    expect(c2Node.opened).to.be.true;

    // do sanity check for same activation path again
    path = [];
    path.push('2c');
    path.push('client');
    navigation.activate(path);

    expect(c1Node.opened).to.be.false;
    expect(c2Node.opened).to.be.true;

    path =[];

    navigation.activate(path);
    expect(c1Node.opened).to.be.false;
    expect(c2Node.opened).to.be.false;
  });

  //TODO revist when have joint profile paths.
  it('should handle joint subnav paths', () => {
    const navigation = document.getElementById('test-navigation'),
      c1Node = document.createElement('ata-navigation-client'),
      c2Node = document.createElement('ata-navigation-client');

    c1Node.setAttribute('client-id', '1c');
    c2Node.setAttribute('client-id', '2c');

    navigation.appendChild(c1Node);
    navigation.appendChild(c2Node);

    let path = [];
    path.push('1c');
    path.push('client');

    navigation.activate(path);
    expect(c1Node.opened).to.be.true;

    path.push('1c');
    path.push('client');
  });

  it('any one of invalid /client/[:id]/info paths deactivates joint node', () => {
    const navigation = document.getElementById('test-navigation'),
      c1Node = document.createElement('ata-navigation-client'),
      c2Node = document.createElement('ata-navigation-client'),
      p1Node = document.createElement('ata-navigation-profile');

    c1Node.setAttribute('client-id', '1c');
    c2Node.setAttribute('client-id', '2c');

    navigation.appendChild(c1Node);
    navigation.appendChild(c2Node);

    p1Node.setAttribute('profile-id', '1p');
    c2Node.appendChild(p1Node);

    let path = '/client/1c/info';

    navigation.disablePath(path);
    const jointLink = navigation.shadowRoot.querySelector('#nav-joint-profiles > a');
    const addJointLink = navigation.shadowRoot.querySelector('.joint-submenu a');
    expect(jointLink.hasAttribute('data-disabled')).to.be.true;
    expect(addJointLink.hasAttribute('data-disabled')).to.be.true;
    path = '/client/2c/info';
    navigation.disablePath(path);
    // validating other client should not enable joint because do not know if first client became valid
    navigation.enablePath(path);
    expect(jointLink.hasAttribute('data-disabled')).to.be.true;
    expect(addJointLink.hasAttribute('data-disabled')).to.be.true;
    navigation.disablePath(path);
    expect(jointLink.hasAttribute('data-disabled')).to.be.true;
    expect(addJointLink.hasAttribute('data-disabled')).to.be.true;
    path = '/client/1c/info';
    navigation.enablePath(path);
    expect(jointLink.hasAttribute('data-disabled')).to.be.true;
    expect(addJointLink.hasAttribute('data-disabled')).to.be.true;
    path = '/client/2c/info';
    navigation.enablePath(path);
    // only when both are known to be enabled
    expect(jointLink.hasAttribute('data-disabled')).to.be.false;
    expect(addJointLink.hasAttribute('data-disabled')).to.be.false;

    // invalid sub profile paths does not affect joint
    path = '/client/2c/profile/1p';
    navigation.disablePath(path);
    expect(jointLink.hasAttribute('data-disabled')).to.be.false;
    expect(addJointLink.hasAttribute('data-disabled')).to.be.false;

    //invalidating a client after it has at least once became valid
    path = '/client/2c/info';
    navigation.disablePath(path);
    expect(jointLink.hasAttribute('data-disabled')).to.be.true;
    expect(addJointLink.hasAttribute('data-disabled')).to.be.true;
  });

  it('invalid /meeting path deactivates client dropdowns and joint dropdown', () => {
    const navigation = document.getElementById('test-navigation'),
      c1Node = document.createElement('ata-navigation-client'),
      c2Node = document.createElement('ata-navigation-client');

    c1Node.setAttribute('client-id', '1c');
    c2Node.setAttribute('client-id', '2c');

    navigation.appendChild(c1Node);
    navigation.appendChild(c2Node);

    let path = '/meeting';
    navigation.disablePath(path);
    expect(c1Node.hasAttribute('disabled')).to.be.true;
    expect(c2Node.hasAttribute('disabled')).to.be.true;

    navigation.enablePath(path);
    expect(c1Node.hasAttribute('disabled')).to.be.false;
    expect(c2Node.hasAttribute('disabled')).to.be.false;
  });


  it('click handlers should not work on disabled /meeting path', () => {
    const navigation = document.getElementById('test-navigation'),
      c1Node = document.createElement('ata-navigation-client'),
      c2Node = document.createElement('ata-navigation-client');

    c1Node.setAttribute('client-id', '1c');
    c2Node.setAttribute('client-id', '2c');

    navigation.appendChild(c1Node);
    navigation.appendChild(c2Node);

    let path = '/meeting';
    navigation.disablePath(path);
    window.location = '#/meeting';

    navigation.firstElementChild.shadowRoot.querySelector('a').click();
    expect(window.location.hash).to.endsWith(`#/meeting`);
    navigation.shadowRoot.querySelector('#nav-joint-profiles a').click();
  });

  it('invalid /client/[:id]/info path deactivates sibling profiles', () => [

  ]);

  // produces false positive
  // it('should not run inverse activation function multiple times', () => {
  //   const navigation = document.getElementById('test-navigation'),
  //   c1Node = document.createElement('ata-navigation-client');
  //
  //   c1Node.setAttribute('client-id', '1c');
  //   navigation.appendChild(c1Node);
  //
  //   let numCalls = 0;
  //   navigation.addEventListener('SubnavSelected', (e) => {
  //     navigation.activate(e.detail.path);
  //     numCalls++;
  //     expect(numCalls).to.equal(1);
  //   });
  //
  //   navigation.firstElementChild.shadowRoot.querySelector('a').click();
  //
  //   expect(numCalls).to.equal(1);
  // });
});
