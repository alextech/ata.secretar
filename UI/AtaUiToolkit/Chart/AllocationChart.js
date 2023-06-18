import Chart from 'chart.js';
import allocationStyle from '!raw-loader!sass-loader!./AllocationChart.scss';

function getCanvasOptions(data) {
  return {
    type: 'pie',
    data: {
      datasets: [{
        data: data.data,
        backgroundColor: data.colors
      }],
      labels: data.labels
    },
    options: {
      animation: {
        duration: 0
      },
      maintainAspectRatio: false,
      tooltips: {
        enabled: false
      },
      legend: {
        display: false,
        // position: 'bottom',
      }
    }
  }
}


let colors = [], names = [], percentages = [];

const chartTpl = document.createElement('template');
// legend is originally not displayed to give a chance to populate
chartTpl.innerHTML =
`
<style>
${allocationStyle}
</style>
<div class="btn-group-toggle">
  <label class="btn btn-secondary">
    <input type="checkbox" name="composition" autocomplete="off"><span></span>
  </label>
</div>
<div style="height: 10em; width: 10em; display: block; margin: auto;">
  <canvas></canvas>
</div>
<div class="legend" style="display: none">
  <span style=""></span><span>XXX ----</span><span>:</span><span>--%</span>
</div>
`;

export default class AllocationChart extends HTMLElement {
  static get observedAttributes() {
    return ['selected', 'name', 'disabled'];
  }

  #chartNode;
  #buttonNode;
  #nameNode;
  #activeControlTag;
  #optionId;

  constructor() {
    super();

    this.attachShadow({mode: "open"});
    this.shadowRoot.appendChild(chartTpl.content.cloneNode(true));

    this.#chartNode = this.shadowRoot.querySelector("canvas");
    this.#buttonNode = this.shadowRoot.querySelector("label");
    this.#nameNode = this.shadowRoot.querySelector('label span');
    this.#activeControlTag = this.shadowRoot.querySelector('label');
  }

   connectedCallback() {
    this.#activeControlTag.firstElementChild.addEventListener('click', (e) => {
        if (this.hasAttribute('selected') || this.hasAttribute('disabled')) {
            return;
        }
        this.setAttribute('selected', '');
        this.dispatchEvent(new CustomEvent('optionChartSelected', {bubbles: true, composed: true}));
    });
  }

  attributeChangedCallback(attribute, oldValue, newValue) {
    if (oldValue === newValue) return;

    switch (attribute) {
      case 'selected':
        if (newValue === null) {
          this.#activeControlTag.classList.remove('active');
        } else {
          this.#activeControlTag.classList.add('active');
        }

        break;
      case 'disabled': {
        if (newValue === null) {
          this.#buttonNode.classList.remove('disabled');
          this.#buttonNode.firstElementChild.disabled = false;
        } else {
          this.#buttonNode.classList.add('disabled');
          this.#buttonNode.firstElementChild.disabled = true;
        }
      }
    }
  }

  set selected(value) {
    if (value) {
      this.setAttribute('selected', '');
    } else {
      this.removeAttribute('selected');
    }
  }

  get selected() {
    this.getAttribute('selected');
  }

  set disabled(value) {
    if (value) {
      this.setAttribute('disabled', '');
    } else {
      this.removeAttribute('disabled');
    }
  }

  get selected() {
    this.getAttribute('disabled');
  }

  set option(option) {
    this.#nameNode.innerText = option.name;
    this.#optionId = option.OptionId;

    for (const part of option.composition) {
      percentages.push(part.percent);
      names.push(part.portfolio);
      colors.push(part.color);
    }

    this.#fillChart();
    this.#fillLegend();

    colors = [];
    names = [];
    percentages = [];
  }

  #fillChart() {
    new Chart(this.#chartNode.getContext('2d'), getCanvasOptions({
      data: percentages,
      colors: colors,
    }));
  }

  #fillLegend() {
    const legend = this.shadowRoot.querySelector('.legend');
    let node, newNode;

    node = legend.firstElementChild;
    node.style.backgroundColor = colors[0];
    node = node.nextElementSibling;
    node.innerText = names[0];
    node = node.nextElementSibling.nextElementSibling;
    node.innerText = percentages[0]+'%';

    for (let i = 1; i < names.length; i++) {
      node = legend.firstElementChild;
      newNode = node.cloneNode();
      newNode.style.backgroundColor = colors[i];
      legend.appendChild(newNode);

      node = node.nextElementSibling;
      newNode = node.cloneNode();
      newNode.innerText = names[i];
      legend.appendChild(newNode);

      node = node.nextElementSibling;
      legend.appendChild(node.cloneNode(true));

      node = node.nextElementSibling;
      newNode = node.cloneNode();
      newNode.innerText = percentages[i]+'%';
      legend.appendChild(newNode);
    }

    legend.style.display = 'grid';
  }
}

if (!customElements.get('ata-allocation-chart')) {
  customElements.define('ata-allocation-chart', AllocationChart);
}
