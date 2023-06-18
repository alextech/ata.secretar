'use strict';
import {expect} from 'chai';
import Meeting from './Meeting';

describe('Client Entity test', () => {
  it('should update only name of advisor object if receiving a string', () => {
    const meeting = new Meeting();
    meeting.advisor = 'String Value';
    expect(meeting.advisor).to.be.an('object').that.deep.equal({name: 'String Value', isInitiator: true});
  });
});
