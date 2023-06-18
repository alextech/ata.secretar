'use strict';

import {applyToNavigation, createFromNavigation} from "./NavigationDraftVM";

import {expect} from 'chai';

describe('VM from navigation', () => {
  before(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-navigation id="test-navigation"></ata-navigation></div>'
    );
  });

  after(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should be aware of clients and effect of enabling meeting', () => {
    const navigation = document.getElementById('test-navigation'),
      client_1_navnode = document.createElement('ata-navigation-client'),
      client_2_navnode = document.createElement('ata-navigation-client');

    client_1_navnode.setAttribute('client-id', '1');
    client_2_navnode.setAttribute('client-id', '2');

    client_1_navnode.setAttribute('disabled', '');

    navigation.appendChild(client_1_navnode);
    let vm = createFromNavigation(navigation);

    expect(vm).to.have.deep.members([
      { // client level
        disabled: true,
        'data-invalid': true,
        children: []
      },
    ]);

    navigation.appendChild(client_2_navnode);

    vm = createFromNavigation(navigation);
    expect(vm).to.have.deep.members([
      { // client level
        disabled: true,
        'data-invalid': true,
        children: []
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ]);

    // emulate meeting form becoming valid
    navigation.enablePath('/meeting');

    vm = createFromNavigation(navigation);
    expect(vm).to.have.deep.members([
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ]);

  });

  it('should be aware of profiles under clients and effect of disabling /info', () => {
    const navigation = document.getElementById('test-navigation'),
      client_1_navnode = navigation.firstElementChild,
      profile_1_1_navnode = document.createElement('ata-navigation-profile');

    profile_1_1_navnode.setAttribute('profile-id', '1');
    client_1_navnode.appendChild(profile_1_1_navnode);

    let vm = createFromNavigation(navigation);

    expect(vm).to.have.deep.members([
      { // client level
        disabled: false,
        'data-invalid': true,
        children: [
          {
            disabled: false, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'objectives'},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ]);

    // invalid /info => disabled siblings
    navigation.disablePath('/client/'+client_1_navnode.getAttribute('client-id')+'/info');
    vm = createFromNavigation(navigation);

    expect(vm).to.have.deep.members([
      { // client level
        disabled: false,
        'data-invalid': true,
        children: [
          {
            disabled: true, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'objectives'},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ]);
  });

  it('should know availability of links in profile shadow dom', () => {
    const navigation = document.getElementById('test-navigation'),
      client_1_navnode = navigation.firstElementChild,
      profile_1_1_navnode = client_1_navnode.firstElementChild;

    navigation.enablePath('/client/'+client_1_navnode.getAttribute('client-id')+'/info');

    navigation.enablePath(
      '/client/'+client_1_navnode.getAttribute('client-id')+
      '/profile/'+profile_1_1_navnode.getAttribute('profile-id')+'/expectations'
    );
    let vm = createFromNavigation(navigation);

    expect(vm).to.have.deep.members([
      { // client level
        disabled: false,
        'data-invalid': false,
        children: [
          {
            disabled: false, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ]);
  });

  it('should be aware of profiles at menu level', () => {
    const navigation = document.getElementById('test-navigation'),
      profile_joint = document.createElement('ata-navigation-profile');

    profile_joint.setAttribute('profile-id', '2');
    navigation.appendChild(profile_joint);

    let vm = createFromNavigation(navigation);

    expect(vm).to.have.deep.members([
      { // client level
        disabled: false,
        'data-invalid': false,
        children: [
          {
            disabled: false, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
      {
        disabled: false,
        steps: [
          {'data-disabled': false, 'data-disabled-by': null},
          {'data-disabled': true,  'data-disabled-by': 'objectives'},
          {'data-disabled': true,  'data-disabled-by': 'expectations'}
        ]
      }
    ]);
  });
});

describe('Navigation from VM', () => {
  before(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-navigation id="test-navigation"></ata-navigation></div>'
    );
  });

  after(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should enable client based on meeting', () => {
    const navigation = document.getElementById('test-navigation'),
      client_1_navnode = document.createElement('ata-navigation-client'),
      client_2_navnode = document.createElement('ata-navigation-client');

    client_1_navnode.setAttribute('client-id', '1');
    client_2_navnode.setAttribute('client-id', '2');

    navigation.appendChild(client_1_navnode);

    let vm = [
      { // client level
        disabled: true,
        'data-invalid': true,
        children: []
      },
    ];

    applyToNavigation(vm, navigation);

    expect(client_1_navnode.hasAttribute('disabled')).to.be.true;

    navigation.appendChild(client_2_navnode);

    vm = [
      { // client level
        disabled: true,
        'data-invalid': true,
        children: []
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ];

    applyToNavigation(vm, navigation);

    expect(client_2_navnode.hasAttribute('disabled')).to.be.false;

    vm = [
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ];

    applyToNavigation(vm, navigation);

    expect(client_1_navnode.hasAttribute('disabled')).to.be.false;
  });

  it('should disable profiles based on /info', () => {
    const navigation = document.getElementById('test-navigation'),
      client_1_navnode = navigation.firstElementChild,
      profile_1_1_navnode = document.createElement('ata-navigation-profile');

    profile_1_1_navnode.setAttribute('profile-id', '1');
    client_1_navnode.appendChild(profile_1_1_navnode);

    // result of running disablePath('/client/:id/info')
    let vm = [
      { // client level
        disabled: false,
        'data-invalid': true,
        children: [
          {
            disabled: true, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'objectives'},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ];

    applyToNavigation(vm, navigation);
    expect(profile_1_1_navnode.hasAttribute('disabled')).to.be.true;

    navigation.enablePath('/client/'+client_1_navnode.getAttribute('client-id')+'/info');

    vm = [
      { // client level
        disabled: false,
        'data-invalid': true,
        children: [
          {
            disabled: false, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'objectives'},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ];

    applyToNavigation(vm, navigation);
    expect(profile_1_1_navnode.hasAttribute('disabled')).to.be.false;
  });

  it('should apply availability of links in profile shadow dom', () => {
    const navigation = document.getElementById('test-navigation'),
      client_1_navnode = navigation.firstElementChild,
      profile_1_1_navnode = client_1_navnode.firstElementChild;

    let vm = [
      { // client level
        disabled: false,
        'data-invalid': true,
        children: [
          {
            disabled: false, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
    ];

    applyToNavigation(vm ,navigation);

    const steps = profile_1_1_navnode.shadowRoot.querySelectorAll('ul > li > a');
    expect(steps.item(0).hasAttribute('data-disabled')).to.be.false;
    expect(steps.item(0).hasAttribute('data-disabled-by')).to.be.false;
    expect(steps.item(1).hasAttribute('data-disabled')).to.be.false;
    expect(steps.item(1).hasAttribute('data-disabled-by')).to.be.false;
    expect(steps.item(2).hasAttribute('data-disabled')).to.be.true;
    expect(steps.item(2).getAttribute('data-disabled-by')).to.equal('expectations');
  });

  it('should apply availability of profile steps under menu', () => {
    const navigation = document.getElementById('test-navigation'),
      profile_joint = document.createElement('ata-navigation-profile');

    profile_joint.setAttribute('profile-id', '2');
    navigation.appendChild(profile_joint);

    let vm = [
      { // client level
        disabled: false,
        'data-invalid': true,
        children: [
          {
            disabled: false, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': true,
        children: []
      },
      {
        disabled: false,
        steps: [
          {'data-disabled': false, 'data-disabled-by': null},
          {'data-disabled': true,  'data-disabled-by': 'objectives'},
          {'data-disabled': true,  'data-disabled-by': 'objectives'}
        ]
      }
    ];

    applyToNavigation(vm, navigation);

    const jointProfileNode = navigation.childNodes.item(2).shadowRoot.querySelectorAll('ul > li > a');
    expect(jointProfileNode.item(1).getAttribute('data-disabled-by')).to.equal('objectives');
    expect(jointProfileNode.item(2).getAttribute('data-disabled-by')).to.equal('objectives');
  });

  it('should enable joint subnav if both clients are enabled', () => {
    const navigation = document.getElementById('test-navigation');

    navigation.firstElementChild.setAttribute('data-invalid', 'true');

    let vm = [
      { // client level
        disabled: false,
        'data-invalid': false,
        children: [
          {
            disabled: false, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': false,
        children: []
      },
      {
        disabled: false,
        steps: [
          {'data-disabled': false, 'data-disabled-by': null},
          {'data-disabled': true,  'data-disabled-by': 'objectives'},
          {'data-disabled': true,  'data-disabled-by': 'objectives'}
        ]
      }
    ];

    applyToNavigation(vm, navigation);
    expect(navigation.firstElementChild.hasAttribute('data-invalid')).to.be.false;

    expect(navigation.shadowRoot.querySelector('li[id="nav-joint-profiles"] > a').hasAttribute('data-disabled')).to.be.false;
  });

  it('should enable Add Profile when client is valid', () => {
    const navigation = document.getElementById('test-navigation'),
      client_1 = navigation.firstElementChild;
    client_1.addProfileLink.setAttribute('data-disabled', '');

    let vm = [
      { // client level
        disabled: false,
        'data-invalid': false,
        children: [
          {
            disabled: false, // cannot add profile unless /info is valid, so new profile will also be enabled
            steps: [
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': false, 'data-disabled-by': null},
              {'data-disabled': true,  'data-disabled-by': 'expectations'}
            ]
          }
        ]
      },
      { // client level
        disabled: false,
        'data-invalid': false,
        children: []
      },
      {
        disabled: false,
        steps: [
          {'data-disabled': false, 'data-disabled-by': null},
          {'data-disabled': true,  'data-disabled-by': 'objectives'},
          {'data-disabled': true,  'data-disabled-by': 'objectives'}
        ]
      }
    ];

    applyToNavigation(vm, navigation);

    expect(client_1.addProfileLink.hasAttribute('data-disabled')).to.be.false;
  })
});
