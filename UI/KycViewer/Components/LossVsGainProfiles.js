'use strict';

import Chart from 'chart.js';

const chartSelectionTpl = document.createElement('template');
chartSelectionTpl.innerHTML =
` <style>.points { display: none; margin: 0 0.3em; } label { display: inline-flex; }</style>
  <div slot="body" style="display: flex; flex-direction: row;">
  <div class="selectionColumn" style="display: flex; flex-direction: column; justify-content: center; margin-left: 1em">
    <label><input type="radio" name="riskType" data-series="0" value="0"><span class="points"> (0) </span> <span>Portfolio A</span></label>
    <label><input type="radio" name="riskType" data-series="1" value="3"><span class="points"> (3) </span> <span>Portfolio B</span></label>
    <label><input type="radio" name="riskType" data-series="2" value="6"><span class="points"> (6) </span> <span>Portfolio C</span></label>
    <label><input type="radio" name="riskType" data-series="3" value="10"><span class="points"> (10) </span> <span>Portfolio D</span></label>
  </div>
  <div style="pointer-events:none;">
    <canvas height="320" width="641"></canvas>
  </div>
</div>`;

const riskCategoryConfig = {
  type: 'bar',
  data: {
    labels: ['1)', '2)', '3)', '4)'
    ],
    datasets: [
      {
        borderColor: 'rgb(153, 176, 53, 0.5)',
        backgroundColor: 'rgb(153, 176, 53, 0.5)',
        hoverBorderColor: 'rgb(153, 176, 53, 1)',
        hoverBackgroundColor: 'rgb(153, 176, 53, 1)',

        data: [200, 500, 1200, 2500]
      },
      {
        borderColor: 'rgb(212, 56, 37, 0.5)',
        backgroundColor: 'rgb(212, 56, 37, 0.5)',
        hoverBorderColor: 'rgb(212, 56, 37, 1)',
        hoverBackgroundColor: 'rgb(212, 56, 37, 1)',

        data: [0, -200, -800, -2000]
      }
    ]
  },
  options: {
    animation: {
      duration: 0
    },
    tooltips: {
      enabled: false
    },
    scales: {
      yAxes: [{
        display: true,
        stacked: true,
        ticks: {
          max: 2500,
          min: -2500,
          callback: function (value, index, values) {
            return '$' + value.toLocaleString('en-US');
          }
        }
      }],
      xAxes: [{
        display: false,
        stacked: true
      }]
    },
    legend: {
      display: false
    },
    onClick: function (e, d, f) {
        console.log(e);
        console.log(d);
        console.log(f);
    }
  }
};

export default class LossVsGain extends HTMLElement {

    #graph;

    static get observedAttributes() {
        return ['value', 'disabled', 'isadvisor'];
    }

    constructor() {
        super();

        this.attachShadow({mode: 'open'});
        this.shadowRoot.appendChild(chartSelectionTpl.content.cloneNode(true));

        this.#graph = new Chart(this.shadowRoot.querySelector('canvas').getContext('2d'), riskCategoryConfig);
    }

    connectedCallback() {
      const highlighter = function(e) {
        let radio;
        if (e.target.nodeName == "LABEL") {
            radio = e.target.firstElementChild;
        } else if (e.target.nodeName != "INPUT") {
          radio = e.target.parentElement.firstElementChild;
        } else {
          radio = e.target;
        }

        const current = parseInt(radio.dataset.series);
        this.#highlightBar(current);

      }.bind(this);

      var that = this;

      this.shadowRoot.querySelectorAll('input[type="radio"], label').forEach((checkbox) => checkbox.addEventListener('mouseover', highlighter));
      this.shadowRoot.querySelector('.selectionColumn').addEventListener('mouseleave', this.#highlightChosen.bind(this));
      this.shadowRoot.querySelectorAll('input[type="radio"]').forEach((checkbox) => checkbox.addEventListener('change', (e) => {
        that.dispatchEvent(new Event('change'));
      }));
    }

    attributeChangedCallback(attribute, oldValue, newValue) {
      if (oldValue === newValue) return;

      switch (attribute) {
        case 'value':
          this.value = newValue;

        break;
        case 'disabled':
            if(newValue != null)
            {
                this.shadowRoot.querySelectorAll('input').forEach(r => r.setAttribute('disabled', ''));
            } else {
                this.shadowRoot.querySelectorAll('input').forEach(r => r.removeAttribute('disabled'));
            }

        break;
        case 'isadvisor':
            if(newValue == null) {
                this.shadowRoot.querySelectorAll('.points').forEach(r => r.style.display = "none");
            } else  {
                this.shadowRoot.querySelectorAll('.points').forEach(r => r.style.display = "inherit");
            }

        break;
      }
    }

    #highlightBar(idx) {
        idx = parseInt(idx);
        const meta = this.#graph.getDatasetMeta(0),
        rect = this.#graph.canvas.getBoundingClientRect(),
        point = meta.data[idx].getCenterPoint();
        if (isNaN(point.x)) {
            return;
        }
        const event = new MouseEvent('mousemove', {
          clientX: rect.left + point.x,
          clientY: rect.top + point.y
        }),
        node = this.#graph.canvas;
        node.dispatchEvent(event);
    }

    #highlightChosen() {
        const idx = this.#selectedIndex;
        if (idx == null) return;

        this.#highlightBar(idx);
    }

    get #selectedIndex() {
      const radios = this.shadowRoot.querySelectorAll('input[type="radio"]');
      for (let radio of radios) {
        if (radio.checked) {
          return radio.dataset.series;
        }
      }
    }

    set value(value) {
        const radios = this.shadowRoot.querySelectorAll('input[type="radio"]');
        for (let radio of radios) {
            if (radio.value === value) {
                radio.checked = true;

                this.#highlightBar(radio.dataset.series);

                break;
            }
        }
    }

    get value() {
      const radios = this.shadowRoot.querySelectorAll('input[type="radio"]');
      for (let radio of radios) {
        if (radio.checked) {
          return radio.value;
        }
      }
    }

    set disabled(value) {
        if(value) {
            this.setAttribute('disabled', '');
        } else {
            this.removeAttribute('disabled');
        }
    }

    get disabled() {
        return this.getAttribute('disabled');
    }
}

if (!customElements.get('ata-loss-vs-gain-profiles')) {
    customElements.define('ata-loss-vs-gain-profiles', LossVsGain);
}
