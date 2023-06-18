'use strict';

import menuStyle from '!raw-loader!sass-loader!./ProfileNode.scss';

import dollar from '@fortawesome/fontawesome-free/svgs/solid/dollar-sign.svg';
import bullseye from '@fortawesome/fontawesome-free/svgs/solid/bullseye.svg';
import linechart from '@fortawesome/fontawesome-free/svgs/solid/chart-line.svg';
import barchart from '@fortawesome/fontawesome-free/svgs/solid/chart-bar.svg';
import jQuery from "jquery";

const profileTpl = document.createElement('template');
profileTpl.innerHTML=
`<style>${menuStyle}</style>
<a href="#" class="nav-profile-a">${dollar} <span class="nav-profile-name"></span>
  <span class="pull-right-container">
    <i class="fa fa-angle-left pull-right"></i>
  </span>
</a>
<ul class="nav-profile" style="display: none;">
  <li data-name="expectations"><a href="#">${linechart} Expectations & Experience</a></li>
  <li data-name="results"><a href="#" data-disabled data-disabled-by="expectations">${barchart} Results</a></li>
</ul>`;

class Profile extends HTMLElement {

  static get observedAttributes() {
    return ['base-url', 'profile-id', 'profile-name', 'opened', 'disabled'];
  }

  constructor() {
    super();

    this.attachShadow({mode: 'open'});
    this.shadowRoot.appendChild(profileTpl.content.cloneNode(true));
    this.linkNode = this.shadowRoot.querySelector('a');

    this.profileNameTxtNode = document.createTextNode('');

    this.addEventListener('click', e => {
      if(this.hasAttribute('disabled')) {
        e.preventDefault();
      }
    });
    this.shadowRoot.querySelector('.nav-profile').addEventListener('click', (e) => {
      if(e.target.hasAttribute('data-disabled')) {
        e.preventDefault();
      }
    });
  }

  connectedCallback() {
    this.shadowRoot.querySelector('.nav-profile-name').appendChild(this.profileNameTxtNode);
  }

  attributeChangedCallback(attribute, oldValue, newValue) {
    if(oldValue === newValue) return;

    switch (attribute) {
      case 'profile-id':
         this._applyLinks();

        break;
      case 'profile-name':
        if(newValue.length === 0) {
          newValue = '\u00A0';
        }
        this.profileNameTxtNode.nodeValue = newValue;

        break;
      case 'base-url':
        this._applyLinks();

        break;
      case 'opened':
        if(newValue === null) {
          jQuery(this.shadowRoot.lastElementChild).slideUp();
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
    }
  }

  set profileID(id) {
    this.setAttribute('profile-id', id);
  }

  get profileID() {
    return this.getAttribute('profile-id');
  }


  set profileName(name) {
    this.setAttribute('profile-name', name);
  }

  get profileName() {
    return this.getAttribute('profile-name');
  }

  set baseUrl(id) {
    this.setAttribute('base-url', id);
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

  activate(pathStack) {
    const topLevel = pathStack.pop(),
      id = pathStack.pop();

    if(this.deactivateFunction) {
      this.deactivateFunction(topLevel, id);
    }

    switch (topLevel) {
      case 'expectations':
        const expectationsNode = this.shadowRoot.querySelector('ul > li:nth-child(1) > a');
        expectationsNode.classList.add('active');
        this.deactivateFunction = function(topLevel) {
          if(topLevel === 'expectations') return;

          expectationsNode.classList.remove('active');
        };

        break;
      case 'results':
        const resultsNode = this.shadowRoot.querySelector('ul > li:nth-child(2) > a');
        resultsNode.classList.add('active');
        this.deactivateFunction = function(topLevel) {
          if(topLevel === 'results') return;

          resultsNode.classList.remove('active');
        };

    }
  }

  disablePath(pathStack) {
    const topLevel = pathStack.pop();
    if(! topLevel) return;

    const firstDisable = this.shadowRoot.querySelector(`ul > li[data-name="${topLevel}`);
    let profileStep = firstDisable.nextElementSibling;
    while(profileStep) {
      if(profileStep.firstElementChild.hasAttribute('data-disabled')) {
        break;
      }

      profileStep.firstElementChild.setAttribute('data-disabled', "true");
      profileStep.firstElementChild.setAttribute('data-disabled-by', topLevel);

      profileStep = profileStep.nextElementSibling;
    }
  }

  enablePath(pathStack) {
    const topLevel = pathStack.pop();
    if(! topLevel) return;

    const firstEnable = this.shadowRoot.querySelector(`ul > li[data-name="${topLevel}`);
    let profileStep = firstEnable.nextElementSibling;
    while(profileStep) {
      if(profileStep.firstElementChild.getAttribute('data-disabled-by') !== topLevel) {
        break;
      }

      profileStep.firstElementChild.removeAttribute('data-disabled');
      profileStep.firstElementChild.removeAttribute('data-disabled-by');

      profileStep = profileStep.nextElementSibling;
    }
  }

  _applyLinks() {
    const base = this.baseUrl + '/profile/' + this.profileID;

    this.linkNode.href = base+'/expectations';
    this.shadowRoot.querySelector('ul > li:nth-child(1) > a').href = base+'/expectations';
    this.shadowRoot.querySelector('ul > li:nth-child(2) > a').href = base+'/results';
  }
}

if(!customElements.get('ata-navigation-profile')) {
  customElements.define('ata-navigation-profile', Profile);
}
