'use strict';

import ValidationModel from '../ValidationModel';
import Profile, {parseAccounts} from "../../investment/profiles/Profile";

let validation,
  modal;

const back = function() {
  history.back();
};

export default class NewProfileController {
  constructor(profileStore, clientStore) {
    this.profileStore = profileStore;
    this.clientStore = clientStore;

  }

  activateView(e) {
    modal = document.getElementById('modal');

    const newProfileDialogue = document.importNode(
      document.getElementById('newProfileTemplate').content, true
    );
    modal.appendChild(newProfileDialogue);
    const addBtn = modal.querySelector('#addProfileBtn');

    document.forms['newProfile'].elements['primary-client-id'].value = e.addForClients[0];
    document.forms['newProfile'].elements['joint-client-id'].value = e.addForClients[1] || '';


    validation = new ValidationModel({
      onValid: function () {
        addBtn.removeAttribute('disabled');
      },

      onInvalid: function () {
        addBtn.setAttribute('disabled', '');
      }
    }, ['profileName', 'accounts']);

    const modalContent = modal.querySelector('[slot="body"]');
    modalContent.addEventListener('input', function(e) {
      validation[e.target.name] = (e.target.value.length >= 1);
    });

    modalContent.addEventListener('click', function(e) {
      if(e.target.name !== 'accounts[]') return;

      const accounts = document.forms['newProfile'].elements['accounts[]'];
      let hasChecked = false;
      for(let account of accounts) {
        if(account.checked) {
          hasChecked = true;
          break;
        }
      }
      validation['accounts'] = hasChecked;
    });

    addBtn.addEventListener('click', this.createProfile.bind(this));

    modal.dataset.title = 'Add '+(e.isJoint ? 'joint' : '') +' profile';
    modal.opened = true;
    modal.addEventListener('modalClosed', back);
  }

  createProfile() {
    const form = document.forms['newProfile'];
    let joint = document.forms['newProfile'].elements['joint-client-id'].value;
    joint = (joint !== '') ? this.clientStore.fetch(joint) : null;

    const profile = new Profile(
      form.elements['profileName'].value,
      parseAccounts(form.elements['accounts[]']),
      this.clientStore.fetch(document.forms['newProfile'].elements['primary-client-id'].value),
      joint
    );
    profile.initialInvestment = parseInt(form.elements['initialInvestment'].value.replace(/,/g, ''));
    profile.monthlyCommitment = parseInt(form.elements['monthlyCommitment'].value.replace(/,/g, ''));
    this.profileStore.addProfile(profile);

    window.location.hash = profile.url+'/objectives';
  }

  deactivateView() {
    modal.removeEventListener('modalClosed', back);
    modal.opened = false;
  }
  _openNewProfileDialogue() {
    // document.querySelector('#newProfile').opened = true;
  }
}
