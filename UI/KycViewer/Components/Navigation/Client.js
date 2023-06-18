'use strict';

import menuStyle from '!raw-loader!sass-loader!./clientNode.scss';
import user from '@fortawesome/fontawesome-free/svgs/solid/user.svg';
import file from '@fortawesome/fontawesome-free/svgs/regular/file-alt.svg';
import plus from '@fortawesome/fontawesome-free/svgs/solid/plus.svg';

import jQuery from 'jquery';


const clientTpl = document.createElement('template');

clientTpl.innerHTML =
`<style>${menuStyle}</style>
<a href="#" class="nav-client-a" data-disabled>${user} <span class="nav-client-name"></span>
  <span class="pull-right-container">
    <i class="fa fa-angle-left pull-right"></i>
  </span>
</a>
<ul class="nav-client" style="display: none">
  <li><a href="#">${file} Client Information</a></li>
  
  <li><a href="#" data-disabled>${plus} Add Profile</a> </li>
</ul>`;

class Client extends HTMLElement {
  static get observedAttributes() {
    return ['base-url', 'client-id', 'client-name', 'opened', 'disabled', 'locked'];
  }

  constructor() {
    super();
    this.slotCounter = 0;
    this.deactivateFunction = undefined;

    this.attachShadow({mode: 'open'});
    this.shadowRoot.appendChild(clientTpl.content.cloneNode(true));

    this.clientNameTxtNode = document.createTextNode('');
    this.linkNode = this.shadowRoot.querySelector('a');
    this.infoNode = this.shadowRoot.querySelector('ul > li > a');
    this.addProfileNode = this.shadowRoot.querySelector('ul > li:last-child');

    this.addProfileLink = this.shadowRoot.querySelector('ul').lastElementChild.firstElementChild;

    this.addEventListener('click', e => {
      if(this.hasAttribute('disabled')) {
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
      }
    });
  }

  // noinspection JSUnusedGlobalSymbols
  connectedCallback() {
    this.shadowRoot.querySelector('.nav-client-name').appendChild(this.clientNameTxtNode);

    this.addProfileLink.addEventListener('click', (e) => {
      if(this.hasAttribute('disabled') || e.target.hasAttribute('data-disabled')) {
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
      }
    });

    this.observer = new MutationObserver(this.#clientNavMutationObserver.bind(this));
    this.observer.observe(this, {childList: true});
  }

  disconnectedCallback() {
    this.observer.disconnect();
  }

  #clientNavMutationObserver(mutationsList, observer) {
    for (let mutation of mutationsList) {
      if (mutation.addedNodes[0]?.nodeName.toLocaleLowerCase() === 'ata-navigation-profile') {
        mutation.addedNodes[0].setAttribute('base-url',this.baseUrl + '/client/'+this.clientID);
        this.#appendProfile(mutation.addedNodes[0]);
      }
    }
  }

  #appendProfile(newChild) {
    const tagName = newChild.nodeName.toLocaleLowerCase();

    let slotName = 'profile-' + this.slotCounter,
      newListItem = document.createElement('li'),
      navList = this.shadowRoot.querySelector('ul');

    newChild.setAttribute('slot', 'profile-' + this.slotCounter);
    newListItem.innerHTML = `<slot name="${slotName}"></slot>`;

    navList.insertBefore(newListItem, navList.lastElementChild);

    this.slotCounter++;

    newChild.setAttribute('client-id', this.getAttribute('client-id'));
  }

  // noinspection JSUnusedGlobalSymbols
  attributeChangedCallback(attribute, oldValue, newValue) {
    if(oldValue === newValue) return;

    switch (attribute) {
      case 'base-url':
        this.linkNode.href = newValue + '/client/' + this.clientID + '/info';
        this.infoNode.href = newValue + '/client/' + this.clientID + '/info';
        this.addProfileLink.href = newValue + '/client/' + this.clientID + '/profile/add';
        this.#forwardBaseUrl();

        break;
      case 'client-id':
        this.linkNode.href = this.baseUrl + '/client/'+newValue+'/info';
        this.infoNode.href = this.baseUrl + '/client/'+newValue+'/info';
        this.addProfileLink.href = this.baseUrl + '/client/'+newValue+'/profile/add';
        this.#forwardBaseUrl();

        break;
      case 'client-name':
        if(newValue.length === 0) {
          newValue = '\u00A0';
        }
        this.clientNameTxtNode.nodeValue = newValue;

        break;
      case 'opened':
        if(newValue === null) {
          jQuery(this.shadowRoot.lastElementChild).slideUp();
          if(this.deactivateFunction) {
            this.deactivateFunction();
          }
        } else {
          jQuery(this.shadowRoot.lastElementChild).slideDown();
        }

        break;
      case 'disabled':
        if(newValue === null) {
          this.linkNode.removeAttribute('data-disabled');
        } else {
          this.linkNode.setAttribute('data-disabled', '');
        }

        break;
      case 'locked':
        if(newValue === null) {
          this.addProfileNode.style.display = '';
        } else {
          this.addProfileNode.style.display = 'none';
        }

        break;
      default:
        return;
    }
  }

  // noinspection JSUnusedGlobalSymbols
  set clientId(id) {
    this.clientID = id;
  }

  set clientID(id) {
    this.setAttribute('client-id', id);
  }

  // noinspection JSUnusedGlobalSymbols
  get clientId() {
    return this.clientID;
  }

  // noinspection JSUnusedGlobalSymbols
  get clientID() {
    return this.getAttribute('client-id');
  }

  // noinspection JSUnusedGlobalSymbols
  set clientName(name) {
    this.setAttribute('client-name', name);
  }

  // noinspection JSUnusedGlobalSymbols
  get clientName() {
    return this.getAttribute('client-name');
  }

  set baseUrl(url) {
    this.setAttribute('base-url', url);
  }

  get baseUrl() {
    return this.getAttribute('base-url');
  }

  set opened(value) {
    if(value) {
      this.setAttribute('opened', '');
    } else {
      this.removeAttribute('opened');
    }
  }

  get opened() {
    return this.hasAttribute('opened');
  }

  set disabled(value) {
    if(value) {
      this.setAttribute('disabled', '');
    } else {
      this.removeAttribute('disabled');
    }
  }

  get disabled() {
    return this.hasAttribute('disabled');
  }

  set locked(value) {
    if(value) {
      this.setAttribute('locked', '');
    } else {
      this.removeAttribute('locked');
    }
  }

  get disabled() {
    return this.hasAttribute('locked');
  }

  get path() {
    let path = 'client/'+this.getAttribute('client-id'),
      parentElement = this.parentElement;
    while(parentElement.path) {
      path = parentElement.path + path;
      parentElement = parentElement.parentElement;
    }

    return path;
  }

  #forwardBaseUrl() {
    for (let i = 0; i < this.children.length; i++) {
      this.children[0].setAttribute('base-url', this.baseUrl + '/client/'+this.clientID);
    }
  }

  activate(pathStack) {
    const topLevel = pathStack.pop(),
      id = pathStack.pop();

    if(this.deactivateFunction) {
      this.deactivateFunction(topLevel, id);
    }

    switch (topLevel) {
      case 'info':
        this.infoNode.classList.add('active');
        this.deactivateFunction = function(toplevel) {
          if(toplevel === 'info') return;

          this.infoNode.classList.remove('active');
        }.bind(this);

        break;
      case 'profile':
        this._activateProfile(id, pathStack);

        break;
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

  disablePath(pathStack) {
    const topLevel = pathStack.pop(),
      id = pathStack.pop();

    switch (topLevel) {
      case 'info':
        for(let i = 0, numProfiles = this.children.length; i < numProfiles; i++) {
          this.children[i].setAttribute('disabled', "true");
        }
        this.addProfileLink.setAttribute('data-disabled', true);

        break;
      case 'profile':
        const profileNode = this.querySelector(`ata-navigation-profile[profile-id="${id}"]`);
        if (!profileNode) {
          throw `Specified profile ${id} is not in navigation tree`;
        }

        profileNode.disablePath(pathStack);
    }
  }

  enablePath(pathStack) {
    const topLevel = pathStack.pop(),
      id = pathStack.pop();

    switch (topLevel) {
      case 'info':
        for(let i = 0, numProfiles = this.children.length; i < numProfiles; i++) {
          this.children[i].removeAttribute('disabled');
        }
        this.addProfileLink.removeAttribute('data-disabled');

        break;
      case 'profile':
        const profileNode = this.querySelector(`ata-navigation-profile[profile-id="${id}"]`);
        if (!profileNode) {
          throw `Specified profile ${id} is not in navigation tree`;
        }

        profileNode.enablePath(pathStack);
    }
  }

}

if(!customElements.get('ata-navigation-client')) {
  customElements.define('ata-navigation-client', Client);
}
