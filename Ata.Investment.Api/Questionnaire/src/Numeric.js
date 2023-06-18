'use strict';

import dollar from '../src/icons/solid/dollar-sign.svg';

const inputTpl = document.createElement('template');

inputTpl.innerHTML =
`<div class="input-group">
  <div class="input-group-prepend" style="display: none;">
    <span class="input-group-text"></span>
  </div>
  <input class="form-control" type="text" min="0" step="1" value="0" placeholder="0" autocomplete="off">
  <div class="input-group-append" style="display: none;">
    <span class="input-group-text">%</span>
  </div>
</div>`;

class Numeric extends HTMLElement {
  static get observedAttributes() {
    return ['data-id', 'name', 'readonly', 'type', 'disabled'];
  }

  constructor() {
    super();

    this.appendChild(inputTpl.content.cloneNode(true));
    this.input = this.querySelector('input');
  }

  connectedCallback() {
    this.input.addEventListener('blur', e => {
      if(e.target.value === '') {
        e.target.value = 0;
      }
    });
    this.input.addEventListener('focus', e => {
      if(e.target.hasAttribute('readonly')) {
        return false;
      }
      if(e.target.value === "0") {
        e.target.value = "";
      }
      e.target.setAttribute('placeholder', '');
    });
  }

  attributeChangedCallback(attribute, oldValue, newValue) {
    if(oldValue === newValue) return;

    switch (attribute) {
      case 'data-id':
        this.input.id = newValue;

        break;
      case 'name':
        this.input.name = newValue;

        break;
      case 'readonly':
        if(newValue === null) {
          this.input.removeAttribute('readonly');
        } else {
          this.input.setAttribute('readonly', '');
        }

        break;
      case 'disabled':
        if(newValue === null) {
          this.input.removeAttribute('disabled');
        } else {
          this.input.setAttribute('disabled', '');
        }

        break;
      case 'type':
        switch(newValue) {
          case 'currency':
            this.querySelector('.input-group-prepend').style.display = 'flex';
            this.querySelector('.input-group-text').innerHTML = dollar;
            this.input.addEventListener('blur', (e) => {
              if(e.target.readOnly) return;
              e.target.setAttribute('type', 'text');

              if(e.target.value.length < 1) return;
              e.target.value = parseInt(e.target.value).toLocaleString('en-US', {minimumFractionDigits:0});
            });
            this.input.addEventListener('focus', (e) => {
              if(e.target.readOnly) {
                e.preventDefault();
                return;
              }
              e.target.value = e.target.value.replace(/,/g, '');
              e.target.setAttribute('type', 'number');
            });

            break;

          case 'percentage':
            this.querySelector('.input-group-append').style.display = 'flex';

            break;
        }
    }
  }
}

if(!customElements.get('ata-numeric')) {
  customElements.define('ata-numeric', Numeric);
}
