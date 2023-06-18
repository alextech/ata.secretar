'use strict';


// ==============  create VM ======================
function nodeVM(navNode) {
  let name, value;
  let hasDataDisabled = navNode.hasAttribute('data-disabled');
  if(hasDataDisabled) {
    name = 'data-disabled';
    value = true;
  } else {
    name = 'disabled';
    value = navNode.hasAttribute('disabled');
  }

  const item = {};
  item[name] = value;

  return item;
}

function profileVM(profileNode) {
  let profileVM = nodeVM(profileNode);
  profileVM.steps = [];

  const steps = profileNode.shadowRoot.querySelectorAll('ul > li > a');
  steps.forEach((step) => {
    profileVM.steps.push({
      'data-disabled': step.hasAttribute('data-disabled'),
      'data-disabled-by': step.getAttribute('data-disabled-by')
    });
  });

  return profileVM;
}

function clientVM(clientNode) {
  const clientVM =  nodeVM(clientNode);
  clientVM['data-invalid'] = clientNode.hasAttribute('data-invalid');
  clientVM.children = [];
  const profileNodes = clientNode.childNodes;
  for(let i = 0, items = profileNodes.length; i < items; i++) {
    clientVM.children.push(profileVM(profileNodes[i]));
  }

  return clientVM;
}

// ============= apply VM ================
function applyProfile(profile, profileNode) {
  const steps = profileNode.shadowRoot.querySelectorAll('ul > li > a');
  for (let i = 0, numSteps = profile.steps.length; i < numSteps; i++) {
    const step = profile.steps[i];
    const stepNode = steps.item(i);
    if(step['data-disabled']) {
      stepNode.setAttribute('data-disabled', '');
    } else {
      stepNode.removeAttribute('data-disabled');
    }

    if(step['data-disabled-by']) {
      stepNode.setAttribute('data-disabled-by', step['data-disabled-by']);
    } else {
      stepNode.removeAttribute('data-disabled-by');
    }
  }
}

function applyClient(client, clientNavNode) {
  if (client.disabled) {
    clientNavNode.setAttribute('disabled', '');
  } else {
    clientNavNode.removeAttribute('disabled');
  }

  if(client['data-invalid']) {
    clientNavNode.setAttribute('data-invalid', '');
    //@TODO consider making this part of client's watching data-invalid attribute, as similar action is performed on disabling /info path
    clientNavNode.addProfileLink.setAttribute('data-disabled', '');
  } else {
    clientNavNode.removeAttribute('data-invalid');
    clientNavNode.addProfileLink.removeAttribute('data-disabled', '');
  }

  const profileNodes = clientNavNode.childNodes;
  // traditional for-i loop, instead of for-of
  // because index of VM should match item order of child nodes
  for (let i = 0, numProfiles = client.children.length; i < numProfiles; i++) {
    if (client.children[i].disabled) {
      profileNodes.item(i).setAttribute('disabled', '');
    } else {
      profileNodes.item(i).removeAttribute('disabled');
    }

    applyProfile(client.children[i], profileNodes.item(i));
  }
}

// ================ EXPORTS ==================
export function createFromNavigation(navigation) {
  const vm = [];

  // meeting, clients, joint - level
  const meetingLevel = navigation.childNodes;
  let navNode;
  // this not an SVG element!!!
  if((navNode = meetingLevel.item(0))) {
    vm.push(clientVM(navNode));
  }
  if((navNode = meetingLevel.item(1))) {
    vm.push(clientVM(navNode));
  }
  for(let i = 2, items = meetingLevel.length; i < items; i++) {
    let item = profileVM(meetingLevel.item(i));
    vm.push(item);
  }

  return vm;
}

//TODO consider making defensive
export function applyToNavigation(vm, navigation) {
  const meetingLevel = navigation.childNodes;
  const c1 = vm[0];
  if(!c1) return;

  applyClient(c1, meetingLevel.item(0));

  const c2 = vm[1];
  if(!c2) return;

  applyClient(c2, meetingLevel.item(1));

  for(let i = 2, items = vm.length; i < items; i++) {
    applyProfile(vm[i], meetingLevel.item(i));
  }

  if(!c1['data-invalid'] && !c2['data-invalid']) {
    navigation.jointProfilesNode.firstElementChild.removeAttribute('data-disabled');
    navigation.jointProfilesNode.querySelector('ul.joint-submenu > li > a').removeAttribute('data-disabled');
  }
}
