'use strict';

import chai from 'chai';
import chaiString from 'chai-string';
import './index';

const expect = chai.use(chaiString).expect;

describe('Profile sub-navigation test', () => {
  beforeEach(() => {
    window.location.hash = '/';
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-navigation-profile id="test-profile"></ata-navigation-profile></div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should know profile related attributes', () => {
    const profileNode = document.getElementById('test-profile'),
      profile = {id: "pro-id", name: "FirstTestProfile"};

    profileNode.setAttribute('profile-id', profile.id);
    profileNode.setAttribute('profile-name', profile.name);

    expect(profileNode.getAttribute('profile-id')).to.equal(profile.id, 'get profile ID using attribute');
    expect(profileNode.getAttribute('profile-name')).to.equal(profile.name, 'get profile name using attribute');

    expect(profileNode.profileID).to.equal(profile.id, 'get profile ID using getter');
    expect(profileNode.profileName).to.equal(profile.name, 'get profile name using getter');

    profileNode.profileName = 'changed using attribute';
    expect(profileNode.getAttribute('profile-name')).to.equal('changed using attribute');
  });

  it('should put profile name into a->span tag', () => {
    const profileNode = document.getElementById('test-profile'),
        profile = {id: "pro-id", name: "InvestingIntoUnitTests"};

    profileNode.profileName = profile.name;
    const profileNameNode = profileNode.shadowRoot.querySelector('.nav-profile-name');
    expect(profileNameNode.innerHTML).to.equal(profile.name);
  });

  it('should put profile id into href tags', () => {
    const profileNode = document.getElementById('test-profile'),
        profile = {id: "pro-id", name: "InvestingIntoUnitTests"};

    profileNode.profileID = profile.id;
    profileNode.baseUrl = '/meeting/m-id/client/OWNER_CLIENT';
    const rootLinkNode = profileNode.shadowRoot.querySelector('a'),
      expectationsNode = profileNode.shadowRoot.querySelector('ul > li:nth-child(1) > a'),
      objectivesNode = profileNode.shadowRoot.querySelector('ul > li:nth-child(2) > a'),
      resultsNode = profileNode.shadowRoot.querySelector('ul > li:nth-child(3) > a');

    expect(rootLinkNode.href).endsWith('/meeting/m-id/client/OWNER_CLIENT/profile/'+profile.id+'/expectations');
    expect(expectationsNode.href).endsWith('/meeting/m-id/client/OWNER_CLIENT/profile/'+profile.id+'/expectations');
    expect(objectivesNode.href).endsWith('/meeting/m-id/client/OWNER_CLIENT/profile/'+profile.id+'/objectives');
    expect(resultsNode.href).endsWith('/meeting/m-id/client/OWNER_CLIENT/profile/'+profile.id+'/results');
  });

  it('should add active class to first node on objectives route', () => {
    const profileNode = document.getElementById('test-profile');
    profileNode.profileId = 'selectiontest';

    profileNode.activate(['objectives']);
    const infoNode = profileNode.shadowRoot.querySelector('ul > li:nth-child(1) > a');
    expect(infoNode.classList.contains('active')).to.be.true;
  });

  it('should add active class to first node on expectations route', () => {
    const profileNode = document.getElementById('test-profile');
    profileNode.profileId = 'selectiontest';

    profileNode.activate(['expectations']);
    const infoNode = profileNode.shadowRoot.querySelector('ul > li:nth-child(2) > a');
    expect(infoNode.classList.contains('active')).to.be.true;
  });

  it('should add active class to first node on results route', () => {
    const profileNode = document.getElementById('test-profile');
    profileNode.profileId = 'selectiontest';

    profileNode.activate(['results']);
    const infoNode = profileNode.shadowRoot.querySelector('ul > li:nth-child(3) > a');
    expect(infoNode.classList.contains('active')).to.be.true;
  });

  it('should stop click events when disabled', () => {
    const profileNode = document.getElementById('test-profile');
    profileNode.profileID = 'clickTest';
    profileNode.disabled = true;


    profileNode.shadowRoot.querySelector('a').click();

    expect(window.location.hash).to.not.endsWith('#');
    expect(window.location.hash).to.not.containIgnoreCase('profile');
  });

  it('should disable subsequent profile steps on disabled', () => {
    const profile = document.createElement('ata-navigation-profile');


    expect(profile.shadowRoot.querySelector('ul > li[data-name="objectives"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="expectations"] > a').hasAttribute('data-disabled')).to.be.true;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="results"] > a').hasAttribute('data-disabled')).to.be.true;

    profile.enablePath(['objectives']);
    expect(profile.shadowRoot.querySelector('ul > li[data-name="objectives"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="expectations"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="results"] > a').hasAttribute('data-disabled')).to.be.true;
    profile.enablePath(['expectations']);
    expect(profile.shadowRoot.querySelector('ul > li[data-name="objectives"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="expectations"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="results"] > a').hasAttribute('data-disabled')).to.be.false;
    profile.disablePath(['expectations']);
    expect(profile.shadowRoot.querySelector('ul > li[data-name="objectives"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="expectations"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="results"] > a').hasAttribute('data-disabled')).to.be.true;
    profile.enablePath(['expectations']);
    expect(profile.shadowRoot.querySelector('ul > li[data-name="objectives"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="expectations"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="results"] > a').hasAttribute('data-disabled')).to.be.false;
    profile.disablePath(['objectives']);
    expect(profile.shadowRoot.querySelector('ul > li[data-name="objectives"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="expectations"] > a').hasAttribute('data-disabled')).to.be.true;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="results"] > a').hasAttribute('data-disabled')).to.be.true;
    profile.enablePath(['objectives']);
    expect(profile.shadowRoot.querySelector('ul > li[data-name="objectives"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="expectations"] > a').hasAttribute('data-disabled')).to.be.false;
    expect(profile.shadowRoot.querySelector('ul > li[data-name="results"] > a').hasAttribute('data-disabled')).to.be.false;
  });

  it('should not change url on disabled link', () => {
    const profileNode = document.createElement('ata-navigation-profile');
    profileNode.profileID = 'profile-1';

    profileNode.shadowRoot.querySelector('ul > li[data-name="expectations"] > a').click();
    expect(window.location.hash).to.not.endsWith('/expectations');
  });
});
