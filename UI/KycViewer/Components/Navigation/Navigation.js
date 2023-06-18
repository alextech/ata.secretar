'use strict';

import menuStyle from '!raw-loader!sass-loader!./navigation.scss';
import handshake from '@fortawesome/fontawesome-free/svgs/regular/handshake.svg';
import users from '@fortawesome/fontawesome-free/svgs/solid/users.svg';
import plus from '@fortawesome/fontawesome-free/svgs/solid/plus.svg';

const navTpl = document.createElement('template');

function createStack(path) {
  if(Array.isArray(path)) {
    return path;
  }

  const pathStack = path.split('/').reverse();
  pathStack.pop();
  return pathStack;
}

navTpl.innerHTML =
`<style>${menuStyle}</style>
<nav>
  <ul class="sidebar-menu" data-widget="tree">
    <li class="active"><a href="#">${handshake} <span>Meeting Details</span></a></li>
    <li id="nav-joint-profiles" class="treeview" style="display: none">
      <a href="#" data-disabled>${users} <span>Joint Profiles</span>
        <span class="pull-right-container">
          <i class="fa fa-angle-left pull-right"></i>
        </span>
      </a>
      <ul class="joint-submenu">
        <li><a href="#/profile/add" data-disabled>${plus} Add Joint Profile</a></li>
      </ul>
    </li>
  </ul>
</nav>`;

class Navigation extends HTMLElement {

  static get observedAttributes() {
    return ['base-url', 'locked'];
  }

  constructor() {
    super();
    this.slotCounter = 0;
    this.deactivateFunction = undefined;
    this.locked = false;

    // if slot within UL wouldn't crash....
    this.attachShadow({mode: 'open'});
    this.shadowRoot.appendChild(navTpl.content.cloneNode(true));

    // by default first active element is known to be meeting node
    this.meetingNode = this.shadowRoot.querySelector('.active');

    this.jointProfilesNode = this.shadowRoot.querySelector('#nav-joint-profiles');
    this.jointListNode = this.jointProfilesNode.querySelector('.joint-submenu');
    this.addJointProfileLink = this.jointListNode.lastElementChild.firstElementChild;
    this.addJointProfileLink.addEventListener('click', e => {
      if(this.hasAttribute('disabled') || e.target.hasAttribute('data-disabled')) {
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
      }
    });
  }

  connectedCallback() {
    this.jointProfilesNode.firstElementChild.addEventListener('click', (e) => {
      e.preventDefault();
    });

    this.observer = new MutationObserver(this.#navigationMutationObserver.bind(this));
    this.observer.observe(this, {childList: true});
  }

  disconnectedCallback() {
    this.observer.disconnect()
  }

  #navigationMutationObserver(mutationsList, observer) {
    for (let mutation of mutationsList) {
      switch(mutation.addedNodes[0]?.nodeName.toLocaleLowerCase()) {
        case 'ata-navigation-client':
          mutation.addedNodes[0].setAttribute('base-url', this.getAttribute('base-url'));
          this._appendClient(mutation.addedNodes[0]);

          break;
        case 'ata-navigation-profile':
          mutation.addedNodes[0].setAttribute('base-url', this.getAttribute('base-url'));
          this._appendProfile(mutation.addedNodes[0]);
      }

      switch(mutation.removedNodes[0]?.nodeName.toLocaleLowerCase()) {
        case 'ata-navigation-client':
          this._removeClient(mutation.removedNodes)
      }
    }
  }


  _appendClient(newChild) {
    let slotName = 'client-' + this.slotCounter,
      newListItem = document.createElement('li'),
      navList = this.shadowRoot.querySelector('nav ul');

    newChild.setAttribute('data-invalid', 'true');
    if (this.locked) {
        newChild.setAttribute('locked', "")
    }
    newChild.setAttribute('slot', slotName);
    newListItem.innerHTML = `<slot name="${slotName}"></slot>`;

    navList.insertBefore(newListItem, navList.lastElementChild);

    this.slotCounter++;

    this._enableJointIfMultipleClients();
  }

  _appendProfile(newChild) {
    let slotName = 'profile-' + this.slotCounter,
      newListItem = document.createElement('li');

    newChild.removeAttribute('client-id');

    newChild.setAttribute('slot', slotName);
    newListItem.innerHTML = `<slot name="${slotName}"></slot>`;

    this.jointListNode.insertBefore(newListItem, this.jointListNode.lastElementChild);

    this.slotCounter++;
  }

  _removeClient(oldChild) {
      this._enableJointIfMultipleClients();
  }

  _enableJointIfMultipleClients() {
    const hasJointClients = this.querySelectorAll('ata-navigation-client').length > 1;
    this.jointProfilesNode.style.display = hasJointClients ? '' : 'none';
  }

  attributeChangedCallback(attribute, oldValue, newValue) {
    if(oldValue === newValue) return;

    switch (attribute) {
      case 'base-url':
        this.meetingNode.firstElementChild.href = newValue;
        for (let i = 0; this.children.length; i++) {
          this.children[0].setAttribute('base-url', newValue);
        }
        this.addJointProfileLink.href = newValue + '/profile/add';

        break;
      case 'locked':
        if(newValue === null) {
          this.locked = false;
          this.addJointProfileLink.style.display = '';
        } else {
          this.locked = true;
          this.addJointProfileLink.style.display = 'none';
        }

        break;
      default:
        return;
    }
  }

  set baseUrl(baseUrl) {
    this.setAttribute('base-url', baseUrl);
  }

  get baseUrl() {
    this.getAttribute('base-url');
  }

  get path() {
    return '/';
  }

  activate(path) {
    const pathStack = createStack(path),
      topLevel = pathStack.pop(),
      id = pathStack.pop();

    if(this.deactivateFunction) {
      this.deactivateFunction(topLevel, id);
    }

    switch(topLevel) {
      case 'meeting':
        this._activateMeeting();

        break;
      case 'client':
        this._activateClient(id, pathStack);

        break;
      case 'profile':
        this._activateProfile(id, pathStack);

        break;
      default:
        this._activateMeeting();
    }
  }

  disablePath(path) {
    const pathStack = createStack(path),
      topLevel = pathStack.pop(),
      id = pathStack.pop();

    switch(topLevel) {
      case 'meeting':
        this._toggleClients(false);

        break;
      case 'client':
        this._invalidateClient(id, pathStack, path);

        break;
      case 'profile':
        this.querySelector(`ata-navigation-profile[profile-id="${id}"]`).disablePath(pathStack);

        break;
    }
  }

  enablePath(path) {
    const pathStack = createStack(path),
      topLevel = pathStack.pop(),
      id = pathStack.pop();

    switch(topLevel) {
      case 'meeting':
        this._toggleClients(true);

        break;
      case 'client':

        this._validateClient(id, pathStack, path);
        break;
      case 'profile':
        this.querySelector(`ata-navigation-profile[profile-id="${id}"]`).enablePath(pathStack);

        break;
    }
  }

  _activateMeeting() {
    this.deactivateFunction = function(toplevel) {
      if(toplevel === '' || toplevel === 'meeting') {
        return;
      }

      this.meetingNode.classList.remove('active');
    };

    this.meetingNode.classList.add('active');
  }

  _toggleClients(toggle) {
    const clients = this.children;
    for(let i = 0, numClients = clients.length; i < numClients; i++) {
      if(toggle){
        clients[i].removeAttribute('disabled');
      } else {
        clients[i].setAttribute('disabled', '');
      }
    }
  }

  _activateClient(id, pathStack) {
    if(id === undefined) {
      throw `Did not specify which client to deactivate`;
    }
    const clientNode = this.querySelector(`ata-navigation-client[client-id="${id}"]`);
    if (!clientNode) {
      throw `Specified ${id} is not in navigation tree`;
    }

    clientNode.opened = true;
    clientNode.activate(pathStack);

    const deactivationHash = 'client'+id;

    // set up inverse of activation ahead of time
    this.deactivateFunction = function(toplevel, id) {
      if((toplevel + id) === deactivationHash) {
        return;
      }
      clientNode.opened = false;
    }
  }

  _activateProfile(id, pathStack) {
    if(id === undefined) {
      throw 'Did not specify which profile to deactivate';
    }
    const profileNode = this.querySelector(`ata-navigation-profile[profile-id="${id}"]`);
    if(!profileNode) {
      throw `Specified ${id} is not in navigation tree`;
    }

    profileNode.opened = true;
    profileNode.activate(pathStack);

    const deactivationHash = 'profile'+id;

    this.deactivateFunction = function(toplevel, id) {
      if((toplevel + id) === deactivationHash) {
        return;
      }

      profileNode.opened = false;
    }
  }

  _invalidateClient(id, pathStack, path) {
    if(id === undefined) {
      throw `Did not specify which client to deactivate`;
    }
    const clientNode = this.querySelector(`ata-navigation-client[client-id="${id}"]`);
    if (!clientNode) {
      throw `Specified client ${id} is not in navigation tree`;
    }

    if(pathStack[pathStack.length - 1] !== 'info') {
      clientNode.disablePath(pathStack);
      return;
    } else {
      clientNode.disablePath(pathStack);
    }

    clientNode.setAttribute('data-invalid', '');
    this._disableJointProfiles();
  }

  _validateClient(id, pathStack) {
    if(id === undefined) {
      throw `Did not specify which client to deactivate`;
    }
    let clientNode = this.querySelector(`ata-navigation-client[client-id="${id}"]`);
    if (!clientNode) {
      throw `Specified client ${id} is not in navigation tree`;
    }

    // if subpath deep under client BECAME valid (emphasis on became, not IS),
    // then client too has to be valid, or else would not be possible to navigate in sub-menu.
    // Discovered while working on #208 reload saved draft.
    clientNode.enablePath(pathStack);

    clientNode.removeAttribute('data-invalid');

    clientNode = this.firstElementChild;
    if(clientNode.hasAttribute('data-invalid') ||
        (clientNode.nextElementSibling &&
          clientNode.nextElementSibling.hasAttribute('data-invalid')
        )
      ) {
      this._disableJointProfiles();
    } else {
      this._enableJointProfiles()
    }
  }

  _disableJointProfiles() {
    this.jointProfilesNode.firstElementChild.setAttribute('data-disabled', '');

    this.addJointProfileLink.setAttribute('data-disabled', 'true');
    const jointProfiles = this.querySelector('ata-navigation-profile');
    if(!jointProfiles) return;
    for(let i = 0, numProfiles = jointProfiles.length; i < numProfiles; i++) {
      jointProfiles.setAttribute('disabled', 'true');
    }
  }

  _enableJointProfiles() {
    this.jointProfilesNode.firstElementChild.removeAttribute('data-disabled');

    this.addJointProfileLink.removeAttribute('data-disabled');
    const jointProfiles = this.querySelector('ata-navigation-profile');
    if(!jointProfiles) return;
    for(let i = 0, numProfiles = jointProfiles.length; i < numProfiles; i++) {
      jointProfiles.removeAttribute('disabled');
    }
  }

  get optionsLink() {
    return this.shadowRoot.querySelector('a.options');
  }
}

if(!customElements.get('ata-navigation')) {
  customElements.define('ata-navigation'  , Navigation);
}
