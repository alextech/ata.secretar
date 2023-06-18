'use strict';
import chai from 'chai';
import chaiString from 'chai-string';
import './AllocationChart';

const expect = chai.use(chaiString).expect;

describe('Allocation Chart test', () => {
  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-allocation-chart id="test-chart" name="Test Chart"></ata-allocation-chart></div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should fill name, and legend with color | fund | percentage', () => {
    const allocation = document.getElementById('test-chart');

    allocation.option = {
      compositionNumber: 0,
      name: "Some Fund Option",
      compositionParts: [
        {
          "percent": 40,
          "portfolio": "СIG 1111",
          "color": "red"
        },
        {
          "percent": 60,
          "portfolio": "СIG 2222",
          "color": "blue"
        },
      ]
    };

    expect(allocation.shadowRoot.querySelector('label').innerHTML).to.have.string('Some Fund Option');

    let nodes;
    nodes = allocation.shadowRoot.querySelectorAll('.legend span:nth-child(4n+1)');
    expect(nodes.length).to.equal(2);
    expect(nodes[0].style.backgroundColor).to.equal('red');
    expect(nodes[1].style.backgroundColor).to.equal('blue');

    nodes = allocation.shadowRoot.querySelectorAll('.legend span:nth-child(4n)');
    expect(nodes.length).to.equal(2);
    expect(nodes[0].innerText).to.equal('40%');
    expect(nodes[1].innerText).to.equal('60%');

    nodes = allocation.shadowRoot.querySelectorAll('.legend span:nth-child(4n+2)');
    expect(nodes.length).to.equal(2);
    expect(nodes[0].innerText).to.equal('СIG 1111');
    expect(nodes[1].innerText).to.equal('СIG 2222');
  });

});
