'use strict';

import moment from 'moment';
import {update as updateMeeting} from '../api/meetingApi';

export default class Meeting {
  constructor() {
    this.id = (window.settings) ?  window.settings.meetingUUID : '';
    this.date = moment();
    this.advisor = {
      name: 'Cliff Steele',
      isInitiator: true
    };
    this.isJoint = false;
    this.otherAttendees = [];
    this.purpose = '';
  }

  save() {
    console.log('meeting save start');
    return updateMeeting(this);
  }

  get advisor() {
    return this._advisor;
  }

  set advisor(advisor) {
    if(typeof advisor === 'string' || advisor instanceof String) {
      this._advisor.name = advisor;
    } else {
      this._advisor = advisor;
    }
  }
}

export function createFromDraft(draft) {
  const meeting =  new Meeting();
  meeting.date = moment.unix(draft.date);
  delete draft.date;
  for(let prop in draft) {
    if(draft.hasOwnProperty(prop)){
      meeting[prop] = draft[prop];
    }
  }

  return meeting;
}
