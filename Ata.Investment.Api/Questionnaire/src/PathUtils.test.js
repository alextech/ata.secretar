import {expect} from 'chai';
import {createPathFromEvent, getElementFromPath} from './PathUtils';

class stubElement extends HTMLElement {
  constructor() {
    super();

    this.attachShadow({mode: 'open'});
    this.shadowRoot.innerHTML = `<div><input type="text" id="stub-1" /><slot name="sub-stub"></slot></div>`;
  }
}

customElements.define('ata-stub-element', stubElement);

describe('PathUtils test', () => {

  beforeEach(() => {

    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><div id="parent"><input type="text" id="child" /><ata-stub-element><a slot="sub-stub"></a></ata-stub-element></div><svg height="100" width="100">' +
      '  <circle cx="50" cy="50" r="40" stroke="black" stroke-width="3" fill="red" />' +
      '</svg> </div>'
    );

  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should have path of IDs towards keyup target', () => {
    const keyupEvent = new KeyboardEvent('keyup', {bubbles: true, composed: true}),
      testNode = document.querySelector('ata-stub-element');

    testNode.focus();
    testNode.value = 'testValue';

    let wasCalled = false;
    const testKeyupTest = function(e) {
      wasCalled = true;
      const path = createPathFromEvent(e);

      expect(path).to.equal('DIV:nth-child(1) > DIV:nth-child(1) > TSG-STUB-ELEMENT:nth-child(2)');
    };

    document.body.addEventListener('keyup', testKeyupTest);

    testNode.dispatchEvent(keyupEvent);
    expect(wasCalled).to.be.true;
    document.body.removeEventListener('keyup', testKeyupTest);
  });

  // needed because ::shadow and /deep/ were deprecated and removed in Chrome 63, while Shadow Parts spec is just being written
  it('should have slash as separator for shadow entry of IDs towards keyup target', () => {
    const keyupEvent = new KeyboardEvent('keyup', {bubbles: true, composed: true}),
      testNode = document.querySelector('ata-stub-element').shadowRoot.getElementById('stub-1');
      // testNode = document.querySelector('ata-stub-element').shadowRoot.querySelector('slot[name="sub-stub"]');

    testNode.focus();
    testNode.value = 'testValue';


    let wasCalled = false;
    const testKeyupTest = function(e) {
      wasCalled = true;
      const path = createPathFromEvent(e);

      expect(path).to.equal('DIV:nth-child(1) > DIV:nth-child(1) > TSG-STUB-ELEMENT:nth-child(2) / DIV:nth-child(1) > INPUT:nth-child(1)');
    };
    document.body.addEventListener('keyup', testKeyupTest);

    testNode.dispatchEvent(keyupEvent);
    expect(wasCalled).to.be.true;
    document.body.removeEventListener('keyup', testKeyupTest);
  });

  it('should find target element from path', () => {
    let path = '#fixture #parent #child',
      testElement = document.getElementById('child'),
      element = getElementFromPath(path);

    expect(element.id).to.equal(testElement.id);

    path = 'DIV:nth-child(1) > DIV:nth-child(1) > TSG-STUB-ELEMENT:nth-child(2) / DIV:nth-child(1) > INPUT:nth-child(1)';
    element = getElementFromPath(path);
    testElement = document.querySelector('ata-stub-element').shadowRoot.querySelector('#stub-1');
    expect(element.id).to.equal(testElement.id);
  });

  it('should handle events from slotted light DOM', () => {
    const link = document.querySelector('ata-stub-element a'),
      mouseEvent = new MouseEvent('click', {bubbles: true, composed: true});

    let wasCalled = false;
    const clickTest = function(e) {
      wasCalled = true;
      const path = createPathFromEvent(e);

      expect(path).to.equal('DIV:nth-child(1) > DIV:nth-child(1) > TSG-STUB-ELEMENT:nth-child(2) > A:nth-child(1)');
    };
    document.body.addEventListener('click', clickTest);

    link.dispatchEvent(mouseEvent);
    expect(wasCalled).to.be.true;
    document.body.removeEventListener('click', clickTest);
  });

  it('should discard path under SVG', () => {
    const svg = document.querySelector('svg circle'),
      mouseEvent = new MouseEvent('click', {bubbles: true, composed: true});

    let wasCalled = false;
    const clickTest = function(e) {
      wasCalled = true;
      const path = createPathFromEvent(e);

      expect(path).to.equal('DIV:nth-child(1) > svg:nth-child(2)');
    };
    document.body.addEventListener('click', clickTest);

    svg.dispatchEvent(mouseEvent);
    expect(wasCalled).to.be.true;
    document.body.removeEventListener('click', clickTest);
  });
});
