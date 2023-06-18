'use strict';

import Panel from "./Panel";
import modalStyle from '!raw-loader!sass-loader!./Modal.scss';

import closeIcon from '../icons/solid/times.svg';

export default class Modal extends Panel {
  static get observedAttributes() {
    const attributes = super.observedAttributes;
    attributes.push('opened');
    return attributes;
  }

  constructor() {
    super();

    const panel = this.shadowRoot.querySelector('.panel'),
      modal = document.createElement('section'),
      style = document.createElement('style');

    this.shadowRoot.querySelector('.panel').insertAdjacentHTML(
      'beforeEnd',
      '<div class="options"><slot name="options"></slot></div>'
    );

    panel.classList.add('modal-content');

    style.innerHTML = modalStyle;
    this.shadowRoot.insertBefore(style, panel);

    modal.className = 'modal';
    this.shadowRoot.insertBefore(modal, panel);
    modal.appendChild(panel);

    this.modalNode = modal;
    const tmp = document.createElement('div');
    tmp.innerHTML = closeIcon;
    this.closeIcon = tmp.firstElementChild;
    this.closeIcon.classList.add('closeIcon');
    this.closeIcon.slot = 'icons';
    this.appendChild(this.closeIcon);
  }

  connectedCallback() {
    this.closeIcon.addEventListener('click', () => this.removeAttribute('opened'));
  }

  attributeChangedCallback(attribute, oldValue, newValue) {
    super.attributeChangedCallback(attribute, oldValue, newValue);
    if(oldValue === newValue) return;

    switch (attribute) {
      case 'opened':
        this.modalNode.classList.toggle("show-modal");
        if(newValue === null) {
          this.dispatchEvent(new CustomEvent('modalClosed', {bubbles: true, composed: true}));
        }
    }
  }

  appendChild(newChild) {
    const prevBody = this.querySelectorAll('[slot="body"],[slot="options"]');
    for(let i = 0, nodes = prevBody.length; i < nodes; i++) {
      this.removeChild(prevBody[i]);
    }

    return super.appendChild(newChild);
  }

  set opened(isOpened) {
    if(isOpened) {
      this.setAttribute('opened', '');
    } else {
      this.removeAttribute('opened')
    }
  }

  get opened() {
    return this.hasAttribute('opened');
  }
}

if(!customElements.get('ata-modal')) {
  customElements.define('ata-modal', Modal);
}
