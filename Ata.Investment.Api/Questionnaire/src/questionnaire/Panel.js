'use strict';

import Tooltip from 'tooltip.js';

import panelStyle from '!raw-loader!sass-loader!./panel.scss';
import questionMark from '../../src/icons/solid/question-circle.svg';
import closeIcon from '../../src/icons/solid/times-circle.svg';
// panel.scss is imported in index.scss

let questionIcon = undefined;

const panelTpl = document.createElement('template');
panelTpl.innerHTML =
`<style>${panelStyle}</style>
<div class="panel">
  <div class="title"><span></span> <span class="icons"> <slot name="icons"></slot></span></div>
  <div class="panelContent"><slot name="body"></slot></div>
</div>
`;

export default class Panel extends HTMLElement {
  static get observedAttributes() {
    return ['data-title'];
  }

  constructor() {
    super();

    this.attachShadow({mode: 'open'});
    this.shadowRoot.appendChild(panelTpl.content.cloneNode(true));

    this.titleNode = this.shadowRoot.querySelector('.title span');
    this.iconsNode = this.shadowRoot.querySelector('.title .icons');
  }

  connectedCallback() {
    const tooltipContent = this.querySelector('[slot="tooltip"]');
    if(tooltipContent) {
      if(!questionIcon) {
        questionIcon = document.createElement('div');
        questionIcon.innerHTML = questionMark;
      }
      const helpIcon = questionIcon.firstElementChild.cloneNode(true);
      helpIcon.slot = 'icons';
      this.insertBefore(helpIcon, tooltipContent);
      const dispatchTooltipEvent = function (tooltip) {
        this.dispatchEvent(new CustomEvent('TooltipOpened', {detail: {tooltip: tooltip}, bubbles: true, composed: true}));
      }.bind(this);

      const placement = tooltipContent.dataset.tooltipPlacement || 'right';
      const tooltip = new Tooltip(this, {
        html: true,
        trigger: 'manual',
        placement: placement,
        title: tooltipContent,
        template: `<div class="popover fade bs-popover-${placement} show tooltip tooltip_${this.id}" role="tooltip">`
        + '<div class="arrow tooltip-arrow"></div>'
        + `<h3 class="popover-header">&nbsp;${closeIcon}</h3>`
        + '<div class="popover-body tooltip-inner"></div></div>',
        popperOptions: {
          onCreate: function (popper) {
            const closeBtn = popper.instance.popper.querySelector('.popover-header svg');
            closeBtn.addEventListener('click', tooltip.hide);
            dispatchTooltipEvent(tooltip);
          },
          onUpdate: function() {
            dispatchTooltipEvent(tooltip);
          }
        }
      });

      helpIcon.addEventListener('click', function() {
        tooltip.toggle();
      });
    }
  }

  attributeChangedCallback(attribute, oldValue, newValue) {
    if(oldValue === newValue) return;

    switch (attribute) {
      case 'data-title':
        this.titleNode.innerHTML = newValue;

        break;
      default:
        return;
    }
  }

  set title(title) {
    this.setAttribute('title', title);
  }

  get title() {
    return this.getAttribute('title');
  }
}

if(!customElements.get('ata-panel')) {
  customElements.define('ata-panel', Panel);
}
