'use strict';

import ValidationModel from '../ValidationModel';
import {initRunningTotal, destructRunningTotal} from "./ProfileUtils";

let profileInfoPanel;
let validation;

//TODO move to profile utils.
function setInfoPanelTitle(profile) {
  profileInfoPanel.dataset.title =
    `${profile.primaryClient.name} ${(profile.isJoint) ? `and ${profile.jointClient.name}` : ''}: ${profile.name}` + profileInfoPanel.getAttribute('data-base-title');
}

function getNextLink(profile) {
  return `#/${(profile.isJoint) ? '' : `client/${profile.primaryClient.id}/`}profile/${profile.id}/results`;
}

let statusEl;
let updateScoreLabel;

export class ExpectationsController {
  constructor() {
    this.view = document.getElementById('profileExpectationsView');
    profileInfoPanel = document.getElementById('expectations');

    profileInfoPanel.querySelectorAll('input').forEach(field => {
      field.addEventListener('click', (e) => {
        validation[e.target.getAttribute('name')] = true;
      });
    });
  }

  activateView(profile) {
    this.profile = profile;
    this.view.style.display = 'block';
    setInfoPanelTitle(profile);
    this.view.querySelector('.next').setAttribute('href', getNextLink(profile));

    const nextBtn = this.view.querySelector('.next');
    validation = new ValidationModel(
      `${!profile.isJoint?`/client/${profile.primaryClient.id}` : ''}/profile/${profile.id}/expectations`, [
        ['decline', profile.decline !== undefined],
        ['annualReturn', profile.annualReturn !== undefined],
        ['currentInvestment', profile.currentInvestment !== undefined],
      ], {
        onValid: function() {
          nextBtn.classList.remove('disabled');
        },

        onInvalid: function() {
          nextBtn.classList.add('disabled');
        }
      }
    );
    if(profile.decline !== undefined) {
      this.view.querySelector(`input[name="decline"][value="${profile.decline}"]`).click();
    } else {
      this.view.querySelectorAll('input[name="decline"]').forEach(radio => {radio.checked = false});
    }
    if(profile.annualReturn !== undefined) {
      this.view.querySelector(`input[name="annualReturn"][value="${profile.annualReturn}"]`).click();
    } else {
      this.view.querySelectorAll('input[name="annualReturn"]').forEach(radio => {radio.checked = false});
    }
    if(profile.currentInvestment !== undefined) {
      this.view.querySelector(`input[name="currentInvestment"][value="${profile.currentInvestment}"]`).click();
    } else {
      this.view.querySelectorAll('input[name="currentInvestment"]').forEach(radio => {radio.checked = false});
    }

    this._bindSetters();
  }

  _bindSetters() {
    const profile = this.profile;
    const form = document.forms['profile_expectations'];

    const dataMapper = {
      decline: (v) => profile.decline = parseInt(v) || 0,
      annualReturn: (v) => profile.annualReturn = parseInt(v) || 0,
      currentInvestment: (v) => profile.currentInvestment = parseInt(v) || 0,

      set: function(prop, val) {
        if(this.hasOwnProperty(prop)) {
          this[prop](val);
        }
      }
    };
    updateScoreLabel = function(e) {
      dataMapper.set(e.target.name, e.target.value);
      statusEl.innerHTML = "Profile score: " + Math.round(profile.addScore());
    };
    for(let i = 0, fields = form.elements.length, field;
        i < fields, field = form.elements[i];
        i++) {
        field.addEventListener('click', updateScoreLabel);
    }

    statusEl = initRunningTotal(profile);
  }

  deactivateView() {
    this.view.style.display = 'none';
    destructRunningTotal();
    const form = document.forms['profile_expectations'];
    for(let i = 0, fields = form.elements.length, field;
        i < fields, field = form.elements[i];
        i++) {
      field.addEventListener('click', updateScoreLabel);
    }
  }
}
