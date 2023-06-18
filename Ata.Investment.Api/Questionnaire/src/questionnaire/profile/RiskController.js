'use strict';

import {fetchRiskForPortfolio} from "../../api/riskApi";
import Chart from 'chart.js';

const graphNodeMap = new WeakMap();

function addMonthlyCommitmentListener(profile, panelNode) {
  panelNode.querySelector('input[name="monthlyCommitment"]').addEventListener('click', () => loadYearlyRiskGraphFor(
    profile,
    panelNode
  ))
}

Chart.defaults.NegativeTransparentLine = Chart.helpers.clone(Chart.defaults.line);
Chart.controllers.NegativeTransparentLine = Chart.controllers.line.extend({
  update: function() {
    // get the min and max values
    const min = Math.min.apply(null, this.chart.data.datasets[0].data),
      max = Math.max.apply(null, this.chart.data.datasets[0].data),
      yScale = this.getScaleForId(this.getDataset().yAxisID),

    // figure out the pixels for these and the value 0
      top = yScale.getPixelForValue(max),
      zero = yScale.getPixelForValue(0),
      bottom = yScale.getPixelForValue(min),

    // build a gradient that switches color at the 0 point
      ctx = this.chart.chart.ctx,
      gradient = ctx.createLinearGradient(0, top, 0, bottom),
      ratio = Math.min((zero - top) / (bottom - top), 1);

    gradient.addColorStop(0, '#007bff');
    gradient.addColorStop(ratio, '#007bff');
    gradient.addColorStop(ratio, '#dc3545');
    gradient.addColorStop(1, '#dc3545');
    this.chart.data.datasets[0].backgroundColor = gradient;

    return Chart.controllers.line.prototype.update.apply(this, arguments);
  }
});

const riskCategoryConfig = {
  type: 'line',
  data: {
    labels: [0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,
    ],
    datasets: [
      {
        borderColor: 'rgba(218, 68, 83)',
        backgroundColor: 'rgba(218, 68, 83, 0.5)',
        fill: false,
        data: [1,5,6,7,8,8,9,10,9,8,7.5,8,7.5,7,8,7,4,-0.5,2,4.25,5.5,6.5,8,9,10,9,5,2,-3,-4,-5,-5,-4,-3,1,2,3,5,6,5,5,6,6,7,8,9,10,12,11,12,12,11]
      },
      {
        borderColor: 'rgba(177, 153, 231)',
        backgroundColor: 'rgba(177, 153, 231, 0.5)',
        fill: false,
        data: [1,4,4,6,7,8,8,7,7,6.5,6,6,5,4,3,2.5,3,1,3.5,5,6,6,6,8,8.5,7,5,3,1.75,-1,-2,-2,-1.5,-1,0,1,2,3,3,2,3,4,5,6,8,8,8.5,9,9.5,8.5,9,9.5,]
      },
      {
        borderColor: 'rgb(54, 162, 235)',
        backgroundColor: 'rgb(54, 162, 235, 0.5)',
        fill: false,
        data: [1,2,3,4,5,5.5,5.5,6,5.5,5,4,3.5,3,2,2,1.5,1,1,1,1.75,3,4,5,5.5,6,6,4,3,1.5,1,0,-0.5,-0.5,0,1,1,1.5,2.5,4,4.5,5,5,4.5,4,4.5,5,6,7,7.5,7.5,8,8.5]
      },
      {
        borderColor: 'rgb(255, 159, 64)',
        backgroundColor: 'rgb(255, 159, 64, 0.5)',
        fill: false,
        data: [1,1.5,2,1.5,2,2.25,2.5,2.25,2,1.75,1.5,2,2,2.5,3,2.5,2.25,2.25,2.25,2.5,3,3.25,3.25,3,2.5,2.6,2.7,3,3.25,2.7,2.5,2.5,2.7,2.9,3,3.5,3.7,3.7,3.7,3,3.5,4,3.7,3.5,4,4.5,4.5,4.25,4,4.5,4.6,5]
      },
      {
        borderColor: 'rgb(75, 192, 192)',
        backgroundColor: 'rgb(75, 192, 192, 0.5)',
        fill: false,
        data: [0,0.03921,0.07842,0.11763,0.15684,0.19605,0.23526,0.27447,0.31368,0.35289,0.3921,0.43131,0.47052,0.50973,0.54894,0.58815,0.62736,0.66657,0.70578,0.74499,0.7842,0.82341,0.86262,0.90183,0.94104,0.98025,1.01946,1.05867,1.09788,1.13709,1.1763,1.21551,1.25472,1.29393,1.33314,1.37235,1.41156,1.45077,1.48998,1.52919,1.5684,1.60761,1.64682,1.68603,1.72524,1.76445,1.80366,1.84287,1.88208,1.92129,1.9605,2]
      }
    ]
  },
  options: {
    animation: {
      duration: 0
    },
    scales: {
      yAxes: [{
        display: true,
        ticks: {
          step: 2,
          max: 14,
          min: -6,
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

export function loadYearlyRiskGraphFor(profile, graphPanel) {
  const initial = profile.initialInvestment,
    includeMonthly = graphPanel.querySelector('input[name="monthlyCommitment"').checked,
    classProperty = graphPanel.dataset.class,
    monthly = (includeMonthly ? profile.monthlyCommitment : 0),
    yearlyPercentNode = graphPanel.querySelector('.yearlyPercent'),
    yearlyNode = graphPanel.querySelector('.yearlyReturns'),
    compoundNode = graphPanel.querySelector('.compoundReturns');

  graphPanel.dataset.title = profile[classProperty] + ' ' + graphPanel.dataset.baseTitle;


  if (! graphNodeMap.has(yearlyNode)) {
    const graph = new Chart(yearlyNode.getContext('2d'), {
      type: 'bar',
      data: {datasets:[{offset: 0}]},
      options: {
        scales: {
          yAxes: [{
            ticks: {
              callback: function (value, index, values) {
                return '$' + (value + this.chart.data.datasets[0].offset).toLocaleString('en-US', {minimumFractionDigits:0});
              }
            }
          }]
        },
        legend: {
          display: false
        },
        tooltips: {
          callbacks: {
            label: function (tooltipItem, data) {
              let deltaThatYear, deltaFromInception;
              const dataset = data.datasets[0];

              if (tooltipItem.index === 0) {
                deltaThatYear = dataset.data[0];
                deltaFromInception = deltaThatYear;
              } else {
                deltaThatYear = dataset.data[tooltipItem.index] - dataset.data[tooltipItem.index - 1];
                deltaFromInception = dataset.data[tooltipItem.index] - dataset.data[0];
              }

              const totalAsOfYear = dataset.data[tooltipItem.index] + dataset.offset;

              return [
                'Total as of year: $' + totalAsOfYear.toLocaleString('en-US', {minimumFractionDigits:0}),
                'Delta from previous year: $' + deltaThatYear.toLocaleString('en-US', {minimumFractionDigits:0}),
                'Delta from inception: $' + deltaFromInception.toLocaleString('en-US', {minimumFractionDigits:0})
              ];
            }
          }
        }
        // responsive: true
      }
    });
    graphNodeMap.set(yearlyNode, graph);
  }

  if (! graphNodeMap.has(compoundNode)) {
    const graph = new Chart(compoundNode.getContext('2d'), {
      type: 'line',
      data: {datasets:[{}]},
      options: {
        scales: {
          yAxes: [{
            ticks: {
              callback: function (value, index, values) {
                return '$' + parseInt(value).toLocaleString('en-US', {minimumFractionDigits:0});
              }
            }
          }]
        },
        elements: {
          line: {
            tension: 0
          }
        },
        legend: {
          display: false
        },
        tooltips: {
          callbacks: {
            label: function (tooltipItem, data) {
              let deltaTo, totalAsOfPeriod;

              const dataset = data.datasets[0];
              if (tooltipItem.index === 0) {
                deltaTo = dataset.data[0];
              } else {
                deltaTo = dataset.data[tooltipItem.index] - profile.initialInvestment;
              }

              totalAsOfPeriod = dataset.data[tooltipItem.index];

              return [
                'Total as of period: $' + totalAsOfPeriod.toLocaleString('en-US', {minimumFractionDigits:0}),
                'Delta from today to this period: $' + deltaTo.toLocaleString('en-US', {minimumFractionDigits:0})
              ];
            }
          }
        }
      }
    });
    graphNodeMap.set(compoundNode, graph);
  }

  if (! graphNodeMap.has(yearlyPercentNode)) {
    const graph = new Chart(yearlyPercentNode.getContext('2d'), {
      type: 'bar',
      data: {datasets:[{offset: 0}]},
      options: {
        scales: {
          yAxes: [{
            ticks: {
              callback: function (value, index, values) {
                return value + '%';
              }
            }
          }]
        },
        legend: {
          display: false
        },
        // responsive: true
      }
    });

    graphNodeMap.set(yearlyPercentNode, graph);
    addMonthlyCommitmentListener(profile, graphPanel);
  }

  // TODO class types will not remain static
  // TODO ask if will always have two options
  let fundCode = profile[classProperty].replace(/\s+/g, '');

  let graph;
  graph = graphNodeMap.get(yearlyPercentNode);
  graph.data.datasets[0].data = [];
  graph.update();
  graph = graphNodeMap.get(yearlyNode);
  graph.data.datasets[0].data = [];
  graph.update();
  graph = graphNodeMap.get(compoundNode);
  graph.data.datasets[0].data = [];
  graph.update();

  graphPanel.querySelector('h1 img').style.display = 'inline';

  fetchRiskForPortfolio(fundCode, initial, monthly)
    .then(response => {
      graphPanel.querySelector('h1 img').style.display = 'none';

      let graph;
      graph = graphNodeMap.get(yearlyPercentNode);
      graph.data.labels = response.yearlyPercentReturns.labels;
      graph.data.datasets = response.yearlyPercentReturns.datasets;
      graph.update();

      graph = graphNodeMap.get(yearlyNode);
      graph.data.labels = response.returns.labels;
      graph.data.datasets = response.returns.datasets;
      graph.update();

      graph = graphNodeMap.get(compoundNode);
      graph.data.labels = response.compoundReturns.labels;
      graph.data.datasets = response.compoundReturns.datasets;
      graph.update();
    })
    .catch(e => {
      throw "Problem fetching historic risks: "+e;
    });
}

const categoryTpl = document.createElement('template');
categoryTpl.innerHTML =
`<div slot="body" style="display: flex; flex-direction: row;">
  <div>
    <p>Hover over portfolio names to highlight in the chart. Select to apply.</p>
    <canvas></canvas>
  </div>
  <div style="display: flex; flex-direction: column; justify-content: center; margin-left: 1em">
    <label><input type="radio" name="riskType" data-series="0" value="high">Portfolio A</label>
    <label><input type="radio" name="riskType" data-series="1" value="mediumHigh">Portfolio B</label>
    <label><input type="radio" name="riskType" data-series="2" value="medium">Portfolio C</label>
    <label><input type="radio" name="riskType" data-series="3" value="lowMedium">Portfolio D</label>
    <label><input type="radio" name="riskType" data-series="4" value="low">Portfolio E</label>
  </div>
</div>
<div slot="options">
  <div class="options"><input type="button" value="OK" class="btn btn-success" disabled/></div>
</div>`;

export function loadRiskCategories() {
  const modal = document.getElementById('modal');
  modal.dataset.title = 'Risk selection';

  modal.appendChild(categoryTpl.content.cloneNode(true));
  modal.shadowRoot.querySelector('.modal-content').style.width = '43em';
  modal.opened = true;

  const graph = new Chart(modal.querySelector('canvas').getContext('2d'), riskCategoryConfig);

  let current = 0;
  let highlighter = function(e) {
    let radio;
    if (e.target.firstElementChild) {
      radio = e.target.firstElementChild;
    } else {
      radio = e.target;
    }
    riskCategoryConfig.data.datasets[current].fill = false;
    current = parseInt(radio.dataset.series);
    riskCategoryConfig.data.datasets[current].fill = 'origin';
    graph.update();
  };
  modal.querySelectorAll('input[type="radio"], label').forEach((checkbox) => checkbox.addEventListener('mouseover', highlighter));

  const radios = modal.querySelectorAll('input[type="radio"]'),
    okBtn = modal.querySelector('input[value="OK"]');

  radios.forEach(radio => radio.addEventListener('click', () => okBtn.disabled = false));
  radios.forEach(radio => radio.addEventListener('click', highlighter));

  okBtn.addEventListener('click', () => {
    let categoryName;
    for (let radio of radios) {
      if (radio.checked) {
        categoryName = radio.value;
      }
    }

    const inputEvent = new Event('input'),
      keyUpEvent = new KeyboardEvent('keyup', {bubbles: true, composed: true});
    document.querySelectorAll('ata-percentage-breakdown[name="profile_risk"] input').forEach(input => {
      input.value = 0;
      input.dispatchEvent(inputEvent);
      input.dispatchEvent(keyUpEvent);
    });

    const selectedCategoryField = document.forms['profile_objectives'].elements[categoryName];
    selectedCategoryField.value = 100;
    selectedCategoryField.dispatchEvent(inputEvent);
    selectedCategoryField.dispatchEvent(keyUpEvent);

    document.querySelector('ata-percentage-breakdown[name="profile_risk"]').validate();

    modal.opened = false;
  });
}
