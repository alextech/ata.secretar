import {expect, assert} from 'chai';
import sinon from 'sinon';

import ValidationModel from './ValidationModel';


describe('ValidationModel proxying to navigation', () => {
  beforeEach(() => {
    document.body.insertAdjacentHTML(
      'afterBegin',
      '<div id="fixture"><ata-navigation id="navigation"></ata-navigation></div>'
    );
  });

  afterEach(() => {
    document.body.removeChild(document.getElementById('fixture'));
  });

  it('should call disable/enable path on navigation with path supplied to validation', () => {
    const navigation = document.getElementById('navigation'),
      disableSpy = sinon.spy(navigation, 'disablePath'),
      enableSpy = sinon.spy(navigation, 'enablePath');

    const path = 'test/path';
    const validation = new ValidationModel(path, [
      'test_one',
      'test_two'
    ]);

    validation.test_one = true;
    validation.test_two = true;
    validation.test_one = false;


    assert(enableSpy.calledOnce);
    assert(enableSpy.calledWith(path));
    assert(disableSpy.calledOnce);
    assert(disableSpy.calledWith(path));
  });

  it('should optionally accept initial fields as arrays to designate initial validation state', () => {
    const navigation = document.getElementById('navigation'),
      enableSpy = sinon.spy(navigation, 'enablePath');

    const path = 'test/path';
    const validation = new ValidationModel(path, [
      'test_one',
      ['test_two', true]
    ]);

    // test_two is already true, so doing test_one=true should be enough to trigger enablePath
    validation.test_one = true;

    assert(enableSpy.calledOnce);
    assert(enableSpy.calledWith(path));
  });

  it('should optionally accept target with callbacks if need to run functions somewhere other than on navigation', () => {
    const navigation = document.getElementById('navigation'),
      disableSpy = sinon.spy(navigation, 'disablePath'),
      enableSpy = sinon.spy(navigation, 'enablePath');

    let onValidCalled = false;
    let onInvalidCalled = false;

    const validation = new ValidationModel({
      onValid: function() {
        onValidCalled = true;
      },

      onInvalid: function() {
        onInvalidCalled = true;
      }
    }, [['field1', true], 'field2']);

    validation.field2 = true;
    expect(onValidCalled).to.be.true;

    validation.field1 = false;
    expect(onInvalidCalled).to.be.true;

    // assert(enableSpy.notCalled());
    // assert(disableSpy.notCalled());
  });

  it('should call optional callbacks when need to control both menu and internal functions', () => {
    const navigation = document.getElementById('navigation');

    let onValidCalled = false;
    let onInvalidCalled = false;
    const path = 'test/path';
    const validation = new ValidationModel(path, [
      'test_one',
      'test_two'
    ], {
      onValid: function() {
        onValidCalled = true;
      },
      onInvalid: function() {
        onInvalidCalled = true;
      }
    });

    validation.test_one = true;
    validation.test_two = true;
    validation.test_one = false;

    expect(onValidCalled).to.be.true;
    expect(onInvalidCalled).to.be.true;
  });

  // not only when values change
  it('should take validation state at construction of the fields given', () => {
    const navigation = document.getElementById('navigation');
    let onValidCalled = false;
    const path = 'test/path';
    const validation = new ValidationModel(path, [
      ['test_one', true],
      ['test_two', true]
    ], {
      onValid: function() {
        onValidCalled = true;
      },
      onInvalid: function() {}
    });

    expect(onValidCalled).to.be.true;
  });
});
