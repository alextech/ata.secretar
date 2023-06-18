'use strict';

import {destructRunningTotal, initRunningTotal} from "./ProfileUtils";
import {loadYearlyRiskGraphFor} from "./RiskController";

let graphPanel;

let meeting;

function assignColors(options) {
  const availableColors = ['#28a745', '#ea9439', '#007bff', '#17a2b8'];
  const assignedColors = {};

  for (const option of options) {
    for (const composition of option.compositionParts) {
      let color;
      if (! assignedColors.hasOwnProperty(composition.portfolio)) {
        color = assignedColors[composition.portfolio] = availableColors.pop();
      } else {
        color = assignedColors[composition.portfolio];
      }

      composition.color = color;
    }
  }
}

//TODO move to profile utils.
function setInfoPanelTitle(profile) {
  graphPanel.dataset.title =
    `${profile.primaryClient.name} ${(profile.isJoint) ? `and ${profile.jointClient.name}` : ''}: ${profile.name}` + graphPanel.getAttribute('data-base-title');
}

export class ResultsController {
  constructor(profileStore, m) {
    meeting = m;
    this.profileStore = profileStore;
    this.view = document.getElementById('resultsView');
    graphPanel = document.getElementById('graphPanel');
    this.graphImg = graphPanel.querySelector('img');
    this.exceptions = this.view.querySelector('#exceptions');
    this.exceptionsList = this.view.querySelector('#exceptionsList');

    document.getElementById('deleteProfile_2').addEventListener('click', () => {
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

    document.getElementById('submitProfile').addEventListener('click', () => {
      const modal = document.getElementById('modal');
      const confirmMessage = document.createElement('template');
      confirmMessage.innerHTML = `
<div slot="body">
  Confirm sending "${this.profile.name}" to ${this.profile.primaryClient.email} ${this.profile.isJoint ? "and "+this.profile.jointClient.email : ""}?
</div>
<div slot="options">
  <div class="options"><input type="button" value="Yes" class="btn btn-success"/> <input type="button" value="No" class="btn btn-danger"></div>
</div>`;

      modal.dataset.title = 'Confirm sending profile';
      modal.appendChild(
        document.importNode(confirmMessage.content, true)
      );

      modal.querySelector('.options input[value="Yes"]').addEventListener('click', (e) => {
        e.target.disabled = true;
        modal.querySelector('.options input[value="No"]').disabled = true;
        modal.querySelector('[slot="body"]').innerHTML = 'Saving...';
        this._submit(modal);
      });
      modal.querySelector('.options input[value="No"]').addEventListener('click', () => {
        modal.opened = false;
      });

      modal.opened = true;
    });
  }

  async activateView(profile) {
    this.profile = profile;
    setInfoPanelTitle(profile);
    await profile.calculatePlan();
    this.graphImg.src = `/img/${profile.resultPortfolio.name}.png`;

    // ======== Running total
    initRunningTotal(profile);

    //  ======= exceptions
    while(this.exceptionsList.firstChild) {
      this.exceptionsList.removeChild(this.exceptionsList.firstChild);
    }
    profile.exceptions.forEach(function (message) {
      let li = document.createElement('li');
      li.innerText = message;
      this.exceptionsList.appendChild(li);
    }.bind(this));

    // this._showRiskGraphs(profile);

    this.exceptions.style.display = (profile.exceptions.length) > 0 ? 'block' : 'none';

    this.view.style.display = 'flex';

    await this._showAllocationOptions();
    return new Promise((resolve) =>{resolve()});
  }

  deactivateView() {
    destructRunningTotal();
    this.view.style.display = 'none';
    this._saveClassSelection();
  }

  _saveClassSelection(e) {

    if (e) { // if caused by click event, need to update. Otherwise it is navigation signal.
      this._showRiskGraphs(this.profile);
    }
  }

  _showCompleted(modal, success) {
    const confirmMessage = document.createElement('template');
    confirmMessage.innerHTML = `
<div slot="body">
  ${(success) ? 'Upload complete. Please check your email.' : 'Problem saving profile.'}
</div>
<div slot="options">
    <div class="options"><input type="button" value="OK" class="btn btn-success"></div>
</div>`;

    modal.dataset.title = (success) ? 'Profile uploaded.' : 'Upload failed.';
    modal.appendChild(
      document.importNode(confirmMessage.content, true)
    );
    modal.querySelector('.options input[value="OK"]').addEventListener('click', () => {
      modal.opened = false;
      if(success) {
        window.location.hash = this.profile.url + '/results';
      }
    });
  }

  _showRiskGraphs(profile) {
    Array.from(this.view.querySelectorAll('.resultRiskGraph'))
      .filter(graphPanel => graphPanel.style.display !== 'none')
      .forEach(graphPanel => loadYearlyRiskGraphFor(profile, graphPanel));
  }

  async _showAllocationOptions() {
    const compositionNode = this.view.querySelector('.compositionSelection');
    while (compositionNode.firstChild) {
      compositionNode.removeChild(compositionNode.firstChild);
    }

    const allocation = this.profile.resultPortfolio;
    assignColors(allocation.options);

    for (const option of allocation.options) {
      const optionNode = document.createElement('ata-allocation-chart');
      compositionNode.appendChild(optionNode);

      optionNode.option = option
    }
  }

  _submit(modal) {
    this._saveClassSelection();

    console.log('start save');
    meeting.save()
      .then(() => {
        //TODO cheating
        this.profile.meetingId = meeting.id;
        this.profile.primaryClient.meetingId = meeting.id;
        if(this.profile.isJoint) {
          this.profile.jointClient.meetingId = meeting.id;
        }
      })
      .then(() => this.profile.save())
      .then(() => {
        const URL = (window.settings) ? window.settings.baseUrl : '';
        const emailerRequest = new Request(URL+'profile-emailer/', {
          method: 'POST',
          credentials: 'include',
          body: JSON.stringify({id: this.profile.id})
        });
        return fetch(emailerRequest)
          .then(response => {
            if(response.status === 201) {
              return response.json();
            } else {
              throw new Error('Problem sending email.');
            }
          });
      })
      .then(() => {
        this._showCompleted(modal, true);
      })
      .catch((e) => {
        console.error(e);
        this._showCompleted(modal, false);
      });
  }
}
