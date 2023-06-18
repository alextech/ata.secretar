'use strict';

import Panel from '../Panel';
import breakdownStyle from '!raw-loader!sass-loader!./breakdown.scss';

const breakdownTpl = document.createElement('template');

breakdownTpl.innerHTML =
`<style>${breakdownStyle}</style>
<form> 
  <slot name="label"></slot>
  <div><hr></div>
  <div><label>TOTAL:</label></div>
  
  <slot name="input"></slot>
  <div><hr></div>
  <ata-numeric type="percentage" name="total" disabled></ata-numeric>
</form>
`;

class Breakdown extends Panel {
  constructor() {
    super();

    this.titleNode.innerHTML = 'Account Investment Objective';

    this.shadowRoot.querySelector('.panelContent').appendChild(breakdownTpl.content.cloneNode(true));
    this.totalField = this.shadowRoot.querySelector('ata-numeric[name="total"]');

    this.fields = [];
  }

  connectedCallback() {
    super.connectedCallback();

    this.validate();
    this.addEventListener('input', this.validate.bind(this));
    this.fields = Array.from(this.querySelectorAll('ata-numeric'));

    const attributeObserver = new MutationObserver(function(mutationsList) {
      for(let mutation of mutationsList) {
        if(mutation.type !== 'attributes' || mutation.attributeName !== 'disabled') {
          continue;
        }
        if(! mutation.target.hasAttribute('disabled')) {
          continue;
        }
      }
    }.bind(this));

    attributeObserver.observe(this, {attributes: true, subtree: true});

    const contentObserver = new MutationObserver(function (mutationsList) {
      for (let mutation of mutationsList) {
        for (let node of mutation.addedNodes) {
          if (node.nodeName.toLocaleLowerCase() === 'ata-numeric') {
            this.fields.push(node);
          }
        }
      }

      this.validate();
    }.bind(this));

    contentObserver.observe(this, {childList: true});
  }

  validate() {
    let total = 0;
    for(let i = 0, numFields = this.fields.length; i < numFields; i++) {
      let value = this.fields[i].value;
      if(value === '') {
        value = 0;
      }
      total += parseInt(value);
    }

    this.totalField.setAttribute('value', ""+total);

    if(total !== 100) {
      this.totalField.setAttribute('invalid', 'true');
      if(this._isValid) {
        this._isValid = false;
        this.dispatchEvent(new CustomEvent('validityChanged', {detail: {valid: false}, bubbles: true, composed: true}))
      }
      return false;
    } else {
      if(this._isValid) {
        return true;
      }
      this.totalField.removeAttribute('invalid');
      this._isValid = true;
      this.dispatchEvent(new CustomEvent('validityChanged', {detail: {valid: true}, bubbles: true, composed: true}));

      return true;
    }
  }

  get isValid() {
    return this.validate();
  }

  disconnectedCallback() {
    this.removeEventListener('keyup', this.validate);
  }
}

if(!customElements.get('ata-percentage-breakdown')) {
  customElements.define('ata-percentage-breakdown', Breakdown);
}
