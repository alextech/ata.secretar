'use strict';

import Chart from 'chart.js';

const chartSelectionTpl = document.createElement('template');
chartSelectionTpl.innerHTML =
` <style>.points { display: none; margin: 0 0.3em; } label { display: inline-flex; }</style>
  <div slot="body" style="display: flex; flex-direction: row;">
  <div class="selectionColumn" style="display: flex; flex-direction: column; justify-content: center; margin-left: 1em">
    <label><input type="radio" name="riskType" data-series="0" value="10"><span class="points"> (10) </span><span>Portfolio A</span></label>
    <label><input type="radio" name="riskType" data-series="1" value="6"><span class="points"> (6) </span><span>Portfolio B</span></label>
    <label><input type="radio" name="riskType" data-series="2" value="4"><span class="points"> (4) </span><span>Portfolio C</span></label>
    <label><input type="radio" name="riskType" data-series="3" value="0"><span class="points"> (0) </span><span>Portfolio D</span></label>
  </div>
  <div>
    <canvas height="320" width="641"></canvas>
  </div>
</div>`;

const riskCategoryConfig = {
  type: 'line',
  data: {
    labels: [0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,
    ],
    datasets: [
      {
        borderColor: 'rgb(212, 56, 37)',
        backgroundColor: 'rgba(212, 56, 37, 0.5)',
        fill: false,
        data: [0.2,0.50,1.75,2.75,3.75,5.00,5.00,4.00,3.00,2.00,3.00,3.00,4.00,5.00,5.50,5.00,-0.50,-7.00,0.00,4.25,5.50,6.00,7.00,8.00,8.00,8.00,5.00,2.00,-3.00,-4.00,-11.00,-11.00,-4.00,-3.00,1.00,2.00,3.00,5.00,6.00,5.00,5.00,6.00,6.00,7.00,8.00,9.00,10.00,12.00,11.00,12.00,12.00,11.00]
      },
      {
        borderColor: 'rgb(241, 181, 28)',
        backgroundColor: 'rgba(241, 181, 28, 0.5)',
        fill: false,
        data: [0.2,1.00,1.50,2.00,3.00,3.00,3.25,3.00,4.00,4.50,4.25,2.50,4.00,5.00,3.00,2.50,3.00,-3.00,3.50,5.00,5.25,5.50,5.50,5.75,6.00,5.50,5.00,3.00,1.75,-1.00,-2.00,-2.00,-1.50,-1.00,0.00,1.00,2.00,3.00,3.00,2.00,3.00,4.00,5.00,6.00,8.00,8.00,8.50,9.00,9.00,8.50,8.80,8.40]
      },
      {
        borderColor: 'rgb(0, 159, 200)',
        backgroundColor: 'rgba(0, 159, 200, 0.5)',
        fill: false,
        data: [0.2,0.75,1.25,1.30,1.40,1.50,2.00,1.70,2.00,2.00,2.20,2.20,2.30,2.20,2.10,2.20,1.00,1.00,1.00,1.75,3.00,3.30,3.40,3.50,3.60,3.70,3.80,3.00,1.50,1.00,0.00,-0.50,-0.50,0.00,1.00,1.00,1.50,2.50,4.00,4.50,5.00,5.00,4.50,4.00,4.50,4.60,4.70,4.90,5.00,4.70,4.80,5.00]
      },
      {
        borderColor: 'rgb(153, 176, 53)',
        backgroundColor: 'rgba(153, 176, 53, 0.5)',
        fill: false,
        data: [0.2,0.03921,0.07842,0.11763,0.15684,0.19605,0.23526,0.27447,0.31368,0.35289,0.3921,0.43131,0.47052,0.50973,0.54894,0.58815,0.62736,0.66657,0.70578,0.74499,0.7842,0.82341,0.86262,0.90183,0.94104,0.98025,1.01946,1.05867,1.09788,1.13709,1.1763,1.21551,1.25472,1.29393,1.33314,1.37235,1.41156,1.45077,1.48998,1.52919,1.5684,1.60761,1.64682,1.68603,1.72524,1.76445,1.80366,1.84287,1.88208,1.92129,1.9605,2]
      }
    ]
  },
  options: {
    animation: {
      duration: 0
    },
    events: [],
    scales: {
      yAxes: [{
        display: true,
        ticks: {
          step: 2,
          max: 15,
          min: -15,
          callback: function (value, index, values) {
            return value + '%';
          }
        }
      }],
      xAxes: [{
        display: false,
        ticks: {
          callback: function (value, index, values) {
            return '';
          }
        }
      }]
    },
    elements: {
      point: {
        radius: 0
      },
      line: {
        tension: 0
      }
    },
    legend: {
      display: false
    }
  }
};

export default class HypotheticalChart extends HTMLElement {

    #graph;
    #current;

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
      let highlighter = function(e) {
        let radio;
        if (e.target.nodeName == "LABEL") {
          radio = e.target.firstElementChild;
        } else if (e.target.nodeName != "INPUT") {
          radio = e.target.parentElement.firstElementChild;
        } else {
          radio = e.target;
        }

        const current = parseInt(radio.dataset.series);
        this.#highlightArea(current);

      }.bind(this);

      var that = this;

      this.shadowRoot.querySelectorAll('input[type="radio"], label').forEach((checkbox) => checkbox.addEventListener('mouseover', highlighter));
      this.shadowRoot.querySelector('.selectionColumn').addEventListener('mouseleave', this.#highlightChosen.bind(this));
      this.shadowRoot.querySelectorAll('input[type="radio"]').forEach((checkbox) => checkbox.addEventListener('change', (e) => {
        that.dispatchEvent(new Event('change'));
      }));

      this.#highlightChosen()
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

    #highlightArea(idx) {
        if (this.#current != null) {
            riskCategoryConfig.data.datasets[this.#current].fill = false;
        }

        this.#current = idx;
        riskCategoryConfig.data.datasets[idx].fill = 'origin';
        this.#graph.update();
    }

    #highlightChosen() {
        const idx = this.#selectedIndex;
        if (idx == null) return;

        this.#highlightArea(idx);
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

                this.#highlightArea(radio.dataset.series);

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

if (!customElements.get('ata-hypothetical-profiles')) {
    customElements.define('ata-hypothetical-profiles', HypotheticalChart);
}
