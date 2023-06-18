'use strict';

import ValidationModel from '../ValidationModel';
import {parseAccounts} from "../../investment/profiles/Profile";
import {initRunningTotal, destructRunningTotal} from './ProfileUtils';
import {loadRiskCategories} from "./RiskController";

let profileInfoPanel;
let validation;
let previousTimeHorizon = undefined;
let initialized = false;

//TODO move to profile utils.
function setInfoPanelTitle(e) {
  const profile = e.target;
  profileInfoPanel.dataset.title =
    `${profile.primaryClient.name} ${(profile.isJoint) ? `and ${profile.jointClient.name}` : ''}: ${profile.name}`;
}

function getNextLink(profile) {
  return `#/${(profile.isJoint) ? '' : `client/${profile.primaryClient.id}/`}profile/${profile.id}/expectations`;
}


function validateAccounts() {
  const accounts = document.forms['profile_objectives'].elements['accounts[]'];
  let hasChecked = false;
  for(let account of accounts) {
    if(account.checked) {
      hasChecked = true;
      break;
    }
  }
  validation['accounts[]'] = hasChecked;
}

let statusEl;
let updateScoreLabel,
  validateProfile = (e) => validation['profileName'] = (e.target.value.length !== 0);

export class ObjectivesController {
  constructor(profileStore) {
    this.profileStore = profileStore;
    this.view = document.getElementById('profileObjectivesView');
    profileInfoPanel = document.getElementById('profileInfo');

    this.view.querySelectorAll('ata-percentage-breakdown').forEach(field => {
      field.addEventListener('validityChanged', (e) => {
        validation[field.getAttribute('name')] = e.detail.valid;
      });
    });
    const form = document.forms['profile_objectives'];
    this.view.querySelector('#profileInfo').addEventListener('click', (e) => {
      if(e.target.name !== 'accounts[]') return;
      validateAccounts();
      this.profile.accounts = parseAccounts(form.elements['accounts[]'])
    });
    this.timeChangeHandler = function(e) {
      const val = parseInt(e.target.value);
      if(val === previousTimeHorizon) return;

      if(val === 0) {
        this._timeGrowthConfirm(form);
      } else {
        form.elements['aggGrowth'].disabled = false;
        form.elements['growth'].disabled = false;
        previousTimeHorizon = val;
        validation['timeHorizon'] = true;
      }

    }.bind(this);
    for(let timeCheckbox of form.elements['timehorizon']) {
      timeCheckbox.addEventListener('click', this.timeChangeHandler);
    }

    document.getElementById('deleteProfile_1').addEventListener('click', () => {
      const modal = document.getElementById('modal');
      const confirmMessage = document.createElement('template');
      confirmMessage.innerHTML = `
<div slot="body">
  Confirm deleting "${this.profile.name}"?
</div>
<div slot="options">
  <div class="options"><input type="button" value="Yes" class="btn btn-success"/> <input type="button" value="No" class="btn btn-danger"></div>
</div>`;

      modal.dataset.title = 'Confirm deleting profile';
      modal.appendChild(
        document.importNode(confirmMessage.content, true)
      );

      modal.querySelector('.options input[value="Yes"]').addEventListener('click', () => {
        this.profileStore.removeProfile(this.profile);
        let href =window.location.hash;
        href = href.slice(0, href.indexOf('profile'));
        window.location.hash = href;

        modal.opened = false;
      });
      modal.querySelector('.options input[value="No"]').addEventListener('click', () => {
        modal.opened = false;
      });
      modal.opened = true;
    });

    this.view.querySelector('#riskPicker').addEventListener('click', loadRiskCategories);
  }

  _timeGrowthConfirm(form) {
    if(form.elements['aggGrowth'].value !== '0' || form.elements['growth'].value !== '0') {
      const modal = document.getElementById('modal');
      modal.appendChild(
        document.importNode(
          document.getElementById('timeHorizonConfirmTemplate').content, true
        )
      );

      modal.opened = true;
      modal.dataset.title = 'Set Time Horizon';
      let yesHandler, noHandler;
      const view = this.view;

      yesHandler = function () {
        form.elements['aggGrowth'].disabled = true;
        form.elements['growth'].disabled = true;
        form.elements['aggGrowth'].value = 0;
        form.elements['growth'].value = 0;

        previousTimeHorizon = 0;

        modal.opened = false;
        modal.querySelector('.options input[value="Yes"]').removeEventListener('click', yesHandler);
        modal.querySelector('.options input[value="No"]').removeEventListener('click', noHandler);
        validation['timeHorizon'] = true;
      };

      noHandler = function () {
        if(! previousTimeHorizon) {
          document.getElementById('time_under_3').checked = false;
        } else {
          view.querySelector(`input[name="timehorizon"][value="${previousTimeHorizon}"]`).checked = true;
        }
        modal.opened = false;
        modal.querySelector('.options input[value="Yes"]').removeEventListener('click', yesHandler);
        modal.querySelector('.options input[value="No"]').removeEventListener('click', noHandler);
      };

      modal.querySelector('.options input[value="Yes"]').addEventListener('click', yesHandler);
      modal.querySelector('.options input[value="No"]').addEventListener('click', noHandler);
    } else {
      form.elements['aggGrowth'].disabled = true;
      form.elements['growth'].disabled = true;

      previousTimeHorizon = 0;
    }
  }

  activateView(profile) {
    previousTimeHorizon = undefined;
    this.profile = profile;
    this.view.style.display = 'grid';

    setInfoPanelTitle({target: profile});
    profile.addEventListener('nameChanged', setInfoPanelTitle);
    this.view.querySelector('.next').setAttribute('href', getNextLink(profile));
    const form = document.forms['profile_objectives'];
    form.elements['profileName'].value = profile.name;
    form.elements['profileName'].addEventListener('input', validateProfile);

    const accounts = profile.accounts;
    form.elements['accounts[]'].forEach(field => field.checked = accounts.includes(field.value));
    form.elements['initialInvestment'].value = profile.initialInvestment.toLocaleString('en-US', {minimumFractionDigits:0});
    form.elements['monthlyCommitment'].value = profile.monthlyCommitment.toLocaleString('en-US', {minimumFractionDigits:0});

    const objectives = profile.objectives;
    form.elements['aggGrowth'].value = objectives.aggressiveGrowth;
    form.elements['growth'].value = objectives.growth;
    form.elements['balanced'].value = objectives.balanced;
    form.elements['income'].value = objectives.income;
    form.elements['cashReserve'].value = objectives.cashReserve;

    const riskTolerance = profile.riskTolerance;
    form.elements['high'].value = riskTolerance.high;
    form.elements['mediumHigh'].value = riskTolerance.mediumHigh;
    form.elements['medium'].value = riskTolerance.medium;
    form.elements['lowMedium'].value = riskTolerance.lowMedium;
    form.elements['low'].value = riskTolerance.low;

    for (let note_name in this.profile.notes) {
      if(this.profile.notes.hasOwnProperty(note_name)) {
        form.elements[note_name].value = this.profile.notes[note_name];
      }
    }

    const nextBtn = this.view.querySelector('.next');
    validation = new ValidationModel(
      `${!profile.isJoint?`/client/${profile.primaryClient.id}` : ''}/profile/${profile.id}/objectives`, [
        ['profileName', true],
        'accounts[]', // validated separately bellow
        'profile_objectives',
        'profile_risk',
        ['timeHorizon', profile.timeHorizon !== undefined]
      ], {
        onValid: function() {
          nextBtn.classList.remove('disabled');
        },

        onInvalid: function() {
          nextBtn.classList.add('disabled');
        }
      }
    );

    // synchronizes total while validating
    validation['profile_objectives'] = this.view.querySelector('ata-percentage-breakdown[name="profile_objectives"]').isValid;
    validation['profile_risk'] = this.view.querySelector('ata-percentage-breakdown[name="profile_risk"]').isValid;

    validateAccounts();

    if(profile.timeHorizon !== undefined) {
      this.view.querySelector(`input[name="timehorizon"][value="${profile.timeHorizon}"]`).click();
      previousTimeHorizon = profile.timeHorizon;
    } else {
      this.view.querySelectorAll('input[name="timehorizon"]').forEach(radio => {radio.checked = false});
    }

    if(!initialized) {
      this.view.querySelectorAll('.notes').forEach(note_field => {
        note_field.addEventListener('richTextSaved', (e) => this.profile.notes[e.target.name] = e.target.value);
      });
      initialized = true;
    }

    this._bindSetters();
  }

  _bindSetters() {
    const profile = this.profile;
    const form = document.forms['profile_objectives'];

    const objectives = profile.objectives,
      riskTolerance = profile.riskTolerance;

    statusEl = initRunningTotal(profile);
    const dataMapper = {
      profileName: (v) => profile.name = v,

      aggGrowth: (v) => objectives.aggressiveGrowth = parseInt(v) || 0,
      growth: (v) => objectives.growth = parseInt(v) || 0,
      balanced: (v) => objectives.balanced = parseInt(v) || 0,
      income: (v) => objectives.income = parseInt(v) || 0,
      cashReserve: (v) => objectives.cashReserve = parseInt(v) || 0,

      high: (v) => riskTolerance.high = parseInt(v) || 0,
      mediumHigh: (v) => riskTolerance.mediumHigh = parseInt(v) || 0,
      medium: (v) => riskTolerance.medium = parseInt(v) || 0,
      lowMedium: (v) => riskTolerance.lowMedium = parseInt(v) || 0,
      low: (v) => riskTolerance.low = parseInt(v) || 0,

      timehorizon: (v) => profile.timeHorizon = parseInt(v) || 0,

      initialInvestment: (v) => profile.initialInvestment = parseInt(v.replace(/,/g, '')) || 0,
      monthlyCommitment: (v) => profile.monthlyCommitment = parseInt(v.replace(/,/g, '')) || 0,

      set: function(prop, val) {
        if(this.hasOwnProperty(prop)) {
          this[prop](val);
        }
      }
    };

    updateScoreLabel = function(e) {
      if(e.target.classList.contains('notes')) {
        return;
      }
      dataMapper.set(e.target.name, e.target.value);
      statusEl.innerHTML = "Profile score: " + Math.round(profile.addScore());
    };

    form.addEventListener('keyup', updateScoreLabel);

    for(let i = 0, radios = form.elements['timehorizon'].length, field;
        i < radios, field = form.elements['timehorizon'][i];
        i++) {
      field.addEventListener('click', updateScoreLabel);
    }
  }

  deactivateView() {
    this.view.style.display = 'none';

    const form = document.forms['profile_objectives'];

    // const accounts = form.elements['accounts[]'];
    this.profile.removeEventListener('nameChanged', setInfoPanelTitle);
    destructRunningTotal();

    form.elements['profileName'].removeEventListener('input', validateProfile);
    form.removeEventListener('keyup', updateScoreLabel);
    for(let i = 0, radios = form.elements['timehorizon'].length, field;
        i < radios, field = form.elements['timehorizon'][i];
        i++) {
      field.removeEventListener('click', updateScoreLabel);
    }
  }
}
