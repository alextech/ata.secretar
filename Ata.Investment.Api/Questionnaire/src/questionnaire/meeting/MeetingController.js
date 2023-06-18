'use strict';

import {Client} from "../../investment/clients/index";
import validator from 'validator';
import ValidationModel from '../ValidationModel';

import moment from 'moment';

let validation;

const validTimeFormats = [
  'MMM D, YYYY h:mm a',
  'MMMM D, YYYY h:mm a',
  'MMM D, YYYY h:mma',
  'MMMM D, YYYY h:mma',
];
let currentInitiator;

function syncEmail(emailField, client) {
  const value = validator.trim(emailField.value),
    isValid = validator.isEmail(value);

  validation[emailField.id] = isValid;
  if(isValid) {
    emailField.classList.remove('is-invalid');
    client.email = value;
  } else {
    emailField.classList.add('is-invalid');
  }
}

export class MeetingController {
  constructor(clientStore, profileStore) {
    this.isFirstLoad = true;
    this.clientStore = clientStore;
    this.profileStore = profileStore;

    this.primaryClient = undefined;
    this.jointClient = undefined;

    this.view = document.getElementById('meetingView');


    this.primaryNameField = document.getElementById('primaryClientName');
    this.jointNameField = document.getElementById('jointClientName');
    this.primaryClientEmail = document.getElementById('primaryClientEmail');
    this.jointClientEmail = document.getElementById('jointClientEmail');

    this.primaryNameField.addEventListener('input', this._syncPrimaryClient.bind(this));
    this.jointNameField.addEventListener('input', this._syncJointClient.bind(this));

    this.primaryClientEmail.addEventListener('input', (e) => {syncEmail(e.target, this.primaryClient)});
    this.jointClientEmail.addEventListener('input', (e) => {syncEmail(e.target, this.jointClient)});

    const form = document.forms['meeting'];
    this.form = form;
    form.elements['isJoint'].forEach(jointRadio => jointRadio.addEventListener('click', this._toggleJoint.bind(this)));

    form.elements['date'].addEventListener('input', this._validateDate.bind(this));
    form.elements['time'].addEventListener('input', this._validateDate.bind(this));
    form.elements['date'].addEventListener('blur', e => {
      e.target.value = this.meeting.date.format('MMMM D, YYYY');
    });
    form.elements['time'].addEventListener('blur', e=> {
      e.target.value = this.meeting.date.format('h:mm a');
    });
    form.elements['purpose'].addEventListener('input', e => this.meeting.purpose = e.target.value);
    form.elements['advisor'].addEventListener('change', e => this.meeting.advisor = e.target.value);
    form.elements['isInitiator'].forEach(initiator => initiator.addEventListener('click', this._assignInitiator.bind(this)));
    form.elements['other'].addEventListener('input', this._addOtherRow.bind(this));
    form.elements['other'].addEventListener('blur', this._checkIfOtherNeedsRemoved.bind(this));
    form.elements['other'].addEventListener('blur', this._resetInitiator.bind(this));

    this._toggleInitiatorForOther = function(e) {
      this.view.querySelector(
        `input[type="radio"][data-other-index="${e.target.getAttribute('data-other-index')}"]`
      ).disabled = (e.target.value.trim() === '')
    };
    form.elements['other'].addEventListener('input', this._toggleInitiatorForOther.bind(this));

    validation = new ValidationModel('/meeting', [
      'primaryClientName',
      'primaryClientEmail',
      ['jointClientName', true],
      ['jointClientEmail', true],
      ['date', true],
      ['time', true]
    ]);
  }

  _syncClientName(nameField, client, addProxy) {
    const value = validator.trim(nameField.value),
      isValid = !validator.isEmpty(value);

    validation[nameField.id] = isValid;
    if(isValid) {
      nameField.classList.remove('is-invalid');
    } else {
      nameField.classList.add('is-invalid');
    }

    nameField.parentNode.querySelectorAll(`#${nameField.id} ~ input, #${nameField.id} ~ label input`)
      .forEach(n => n.disabled = !isValid);

    if(!isValid && client === undefined) {
      return false;
    }

    if(client === undefined) {
      client = new Client();
      addProxy(client);
    }

    client.name = value;

    return client;
  }

  _syncPrimaryClient(e) {
    this.primaryClient = this._syncClientName(e.target, this.primaryClient, function(client) {
      this.clientStore.addClient(client);
      if(this.jointClient) {
        this.clientStore.addClient(this.jointClient);
      }
    }.bind(this))
  }

  _syncJointClient(e) {
    this.jointClient = this._syncClientName(e.target, this.jointClient, function(client) {
      if(this.primaryClient) {
        this.clientStore.addClient(client);
      }
    }.bind(this));
  }

  _toggleJoint(e) {
    const isJoint = (e.target.value === 'true');

    if(!isJoint && this.clientStore.jointClient) {
      e.preventDefault();
      this._confirmSingleFromJoint();
      return;
    }

    this.jointNameField.disabled = !isJoint;
    this.meeting.isJoint = !isJoint;
  }

  _validateDate(e) {
    const dateField = this.form.elements['date'],
      timeField = this.form.elements['time'];

    const value = moment(dateField.value+' '+timeField.value, validTimeFormats, true),
      isValid = validation['date'] = value.isValid();

    if(isValid) {
      this.meeting.date = value;
      e.target.classList.remove('is-invalid');
    } else {
      e.target.classList.add('is-invalid');
    }
  }

  _confirmSingleFromJoint() {
    const modal = document.getElementById('modal');
    const confirmMessage = document.createElement('template');
    confirmMessage.innerHTML = `
<div slot="body">
  This will also delete all associated profiles and joint profiles.
</div>
<div slot="options">
  <div class="options"><input type="button" value="Yes" class="btn btn-success"/> <input type="button" value="No" class="btn btn-danger"></div>
</div>
`;

    modal.dataset.title = 'Delete Joint Account';
    modal.appendChild(
      document.importNode(confirmMessage.content, true)
    );

    modal.querySelector('.options input[value="Yes"]').addEventListener('click', () => {
      this.clientStore.removeJoint();
      this.jointNameField.value = '';
      this.jointNameField.disabled = true;
      this.jointNameField.classList.remove('is-invalid');
      this.jointClientEmail.value = '';
      this.jointClientEmail.disabled = true;
      this.jointClientEmail.classList.remove('is-invalid');

      this.view.querySelector('input[name="isJoint"][value="false"]').checked = true;

      validation['jointClientName'] = true;
      validation['jointClientEmail'] = true;
      this.profileStore.findForClient(this.jointClient).forEach(p => this.profileStore.removeProfile(p));

      this.jointClient = undefined;
      modal.opened = false;
    });
    modal.querySelector('.options input[value="No"]').addEventListener('click', () => {
      modal.opened = false;
      this.view.querySelector('input[name="isJoint"][value="true"]').checked = true;
    });
    modal.opened = true;
  }

  _assignInitiator(e) {
    if(currentInitiator) {
      currentInitiator.isInitiator = false;
    }
    switch(e.target.value) {
      case 'primary':
        currentInitiator = this.clientStore.primaryClient;

        break;
      case 'joint':
        currentInitiator = this.clientStore.jointClient;

        break;
      case 'advisor':
        currentInitiator = this.meeting.advisor;

        break;
      case 'other':
        currentInitiator = this.meeting.otherAttendees[e.target.getAttribute('data-other-index')];
    }

    currentInitiator.isInitiator = true;
  }

  //TODO refactor for addRow to just add HTML row, not figure out values. Needed to run addRow when loading meeting
  _addOtherRow(e) {
    const index = parseInt(e.target.dataset.otherIndex);
    let otherObj = this.meeting.otherAttendees[index];

    e.target.value = e.target.value.trimLeft();

    if(e.target.value === '') {
      if(otherObj) {
        this.meeting.otherAttendees.splice(index, 1);
      }

      const nextSiblingText = e.target.parentElement.nextElementSibling.querySelector('input[type="text"]');
      if(nextSiblingText && nextSiblingText.value === '') {
        this._resetInitiator(e);
        e.target.parentElement.parentElement.removeChild(e.target.parentElement.nextElementSibling);
        return;
      }
    }

    if(e.target.value === '') {
      return;
    }

    if(otherObj === undefined) {
      otherObj = {
        name: e.target.value,
        isInitiator: false
      };
      this.meeting.otherAttendees.push(otherObj);
    }

    otherObj.name = e.target.value;

    if(e.target.parentElement.nextElementSibling.className !== 'other_row') {
      const rowTemplate = document.getElementById('otherTemplate');
      const newRow = document.importNode(rowTemplate.content, true);

      const otherText = newRow.querySelector('input[type="text"]');
      otherText.setAttribute('data-other-index', index+1);
      otherText.addEventListener('input', this._addOtherRow.bind(this));
      otherText.addEventListener('input', this._toggleInitiatorForOther.bind(this));
      otherText.addEventListener('blur', this._checkIfOtherNeedsRemoved.bind(this));
      // otherText.addEventListener('blur', this._resetInitiator.bind(this));
      const otherInitiator = newRow.querySelector('input[type="radio"]');
      otherInitiator.setAttribute('data-other-index', index+1);
      otherInitiator.addEventListener('click', this._assignInitiator.bind(this));
      e.target.parentElement.parentElement.insertBefore(
        newRow, rowTemplate
      );
    }
  }

  _checkIfOtherNeedsRemoved(e) {
    if(e.target.value === '') {
      const otherSibling = e.target.parentElement.nextElementSibling.querySelector('input[type="text"]');
      if(otherSibling && otherSibling.value === '') {
        return;
      }

      let index = e.target.dataset.otherIndex;

      const isLast = e.target.parentElement.nextElementSibling.tagName.toLocaleLowerCase() === 'template';
      if(e.target.parentElement.hasAttribute('data-first-other') && !isLast) {
        e.target.parentElement.nextElementSibling.querySelector('label').innerText = 'Other attendees:';
        e.target.parentElement.nextElementSibling.setAttribute('data-first-other', 'true');
      }

      this._resetInitiator(e);
      if(!isLast) {
        e.target.parentElement.parentElement.removeChild(e.target.parentElement);
      } else {
        return;
      }

      let sibling = this.view.querySelector('div[data-first-other="true"]');
      index = 0;
      while(sibling) {
        sibling.querySelectorAll('input').forEach(input => input.setAttribute('data-other-index', index));

        index++;
        sibling = sibling.nextElementSibling;
      }
    }
  }

  _resetInitiator(e) {
    if(e.target.parentElement.querySelector('input[type="radio"]').checked) {
      currentInitiator = this.meeting.advisor;
      currentInitiator.isInitiator = true;
      this.view.querySelector('input[type="radio"][value="advisor"]').checked = true;
    }
  }

  // needed for draft display
  _firstLoad(form) {
    // ====================== FILL IN OTHER NAMES ================
    // at first there will only be one other field so will not be array yet. Force array.
    let otherFields = [form.elements['other']];
    const inputEvent = new KeyboardEvent('input', {bubbles: true, composed: true});
    for (let i = 0, numOther = this.meeting.otherAttendees.length; i < numOther; i++) {
      otherFields[i].value = this.meeting.otherAttendees[i].name;
      otherFields[i].dispatchEvent(inputEvent);
      otherFields = form.elements['other'];
    }

    // ====================== SELECT INITIATOR ======================
    if(currentInitiator) {
      currentInitiator.isInitiator = false;
    }
    switch(true) {
      case (this.clientStore.primaryClient && this.clientStore.primaryClient.isInitiator):
          currentInitiator = this.clientStore.primaryClient;
          this.view.querySelector('input[type="radio"][value="primary"]').checked = true;

        break;
      case (this.clientStore.jointClient && this.clientStore.jointClient.isInitiator):
          currentInitiator = this.clientStore.jointClient;
          this.view.querySelector('input[type="radio"][value="joint"]').checked = true;

          break;
      case this.meeting.advisor.isInitiator:
          currentInitiator = this.meeting.advisor;
          this.view.querySelector('input[type="radio"][value="advisor"]').checked = true;

          break;
      default:
        const otherIndex = this.meeting.otherAttendees.findIndex(attendee => attendee.isInitiator);
        if(otherIndex < 0) break;
        currentInitiator = this.meeting.otherAttendees[otherIndex];
        this.view.querySelector(`input[type="radio"][data-other-index="${otherIndex}"]`).checked = true;
    }
  }

  activateView(meeting) {
    this.meeting = meeting;
    const form = document.forms['meeting'];
    form.elements['date'].value = meeting.date.format('MMMM D, YYYY');
    form.elements['time'].value = meeting.date.format('h:mm a');

    this.primaryClient = this.clientStore.primaryClient;
    this.jointClient = this.clientStore.jointClient;

    if(this.primaryClient) {
      form.elements['primaryClientName'].value = this.primaryClient.name;
      form.elements['primaryClientEmail'].value = this.primaryClient.email;
      form.elements['primaryClientEmail'].disabled = false;
      this.view.querySelector('input[name="isInitiator"][value="primary"]').disabled = false;

      this._syncPrimaryClient({target: form.elements['primaryClientName']});
      syncEmail(form.elements['primaryClientEmail'], this.primaryClient);
    }

    if(this.jointClient) {
      form.elements['isJoint'][1].click();
      form.elements['jointClientName'].value = this.jointClient.name;
      form.elements['jointClientEmail'].value = this.jointClient.email;
      form.elements['jointClientEmail'].disabled = false;
      this.view.querySelector('input[name="isInitiator"][value="joint"]').disabled = false;

      this._syncJointClient({target: form.elements['jointClientName']});
      syncEmail(form.elements['jointClientEmail'], this.jointClient);
    }

    form.elements['purpose'].value = meeting.purpose;

    // HTML comes from server, so it has priority over JS object
    meeting.advisor.name = form.elements['advisor'].value;

    if(this.isFirstLoad) {
      this._firstLoad(form);
      this.isFirstLoad = false;
    }

    this.view.style.display = 'grid';
  }

  deactivateView() {
    this.view.style.display = 'none';
  }
}
