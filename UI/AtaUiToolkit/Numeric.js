'use strict';
const dollar = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 320 512"><path d="M113.411 169.375c0-23.337 21.536-38.417 54.865-38.417 26.726 0 54.116 12.263 76.461 28.333 5.88 4.229 14.13 2.354 17.575-4.017l23.552-43.549c2.649-4.898 1.596-10.991-2.575-14.68-24.281-21.477-59.135-34.09-91.289-37.806V12c0-6.627-5.373-12-12-12h-40c-6.627 0-12 5.373-12 12v49.832c-58.627 13.29-97.299 55.917-97.299 108.639 0 123.533 184.765 110.81 184.765 169.414 0 19.823-16.311 41.158-52.124 41.158-30.751 0-62.932-15.88-87.848-35.887-5.31-4.264-13.082-3.315-17.159 2.14l-30.389 40.667c-3.627 4.854-3.075 11.657 1.302 15.847 24.049 23.02 59.249 41.255 98.751 47.973V500c0 6.627 5.373 12 12 12h40c6.627 0 12-5.373 12-12v-47.438c65.72-10.215 106.176-59.186 106.176-116.516.001-119.688-184.764-103.707-184.764-166.671z"/></svg>';
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
    return ['data-id', 'name', 'readonly', 'type', 'disabled', 'value', 'invalid'];
  }

  constructor() {
    super();
    
    this._initialized = false;
  }
  
  init() {
    this.appendChild(inputTpl.content.cloneNode(true));
    this.input = this.querySelector('input');
    this._initialized = true;
  }

  connectedCallback() {
    if (!this._initialized) this.init();
  
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
    if (attribute !== 'value') {
      if (oldValue === newValue) return;
    }
    
    if (!this._initialized) this.init();

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
      case 'invalid':
        if(newValue === null) {
          this.input.classList.remove('is-invalid')
        } else {
          this.input.classList.add('is-invalid');
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

        break;
      case 'value':
        let parsed = parseInt(newValue);
        if (isNaN(parsed)) { parsed = 0; this.setAttribute('value', 0) }
        this.value = parsed;
        
        break;
    }
  }

  set value(v) {
    this.input.value = v;
    this.input.dispatchEvent(new Event('input', {bubbles: false, cancelable: true}));
    this.input.value = parseInt(v).toLocaleString('en-US', {minimumFractionDigits:0});
  }

  get value() {
    return this.input.value;
  }
}

if(!customElements.get('ata-numeric')) {
  customElements.define('ata-numeric', Numeric);
}
