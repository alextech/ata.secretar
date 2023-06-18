'use strict';

import moment from 'moment';
import ValidationModel from '../ValidationModel';

let validation;
let initialized = false;

export class ClientInfoController {

  constructor() {
    this.view = document.getElementById('clientInfoView');

    this.personalInfoPanel = this.view.querySelector('#personalInfo');
    this.dateOfBirth = this.view.querySelector('#dateOfBirth');
    this.annualIncome = this.view.querySelector('#annualIncome');
    this.householdIncome = this.view.querySelector('#householdIncome');

    this.networthPanel = this.view.querySelector('#netWorth');
    this.networthTotal = this.view.querySelector('#networthTotal');

    this.householdChangeHandler = function(e) {
      this.householdIncome.value = e.details.value.toLocaleString('en-US', {minimumFractionDigits:0});
    }.bind(this);

    this.dateOfBirth.addEventListener('input', e => {
      const value = moment(e.target.value, [
        'MMM D, YYYY',
        'MMMM D, YYYY',
        'DD/MM/YYYY',
        'DD/M/YYYY',
        'D/MM/YYYY',
        'D/M/YYYY',
      ], true),
        isValid = value.isValid() && value.isBefore(moment(), 'day');

      validation['dateOfBirth'] = isValid;
      if(isValid) {
        this.activeClient.dateOfBirth = value;
        e.target.value = value.format('MMMM D, YYYY');
        e.target.classList.remove('is-invalid');
      } else {
        e.target.classList.add('is-invalid');
      }
    });

    this.annualIncome.addEventListener('input', e => {
      let value = e.target.value.replace(/,/g, '');
      value = (value === '') ? 0 : parseInt(value);

      this.activeClient.annualIncome = value;
    });

    this.networthPanel.addEventListener('input', e => {
      let value = e.target.value.replace(/,/g, '');
      value = (value === '') ? 0 : parseInt(value);

      this.activeClient.networth[e.target.id] = value;
    });
    this.networthChangeHandler = function(e) {
      this.networthTotal.value = e.target.total.toLocaleString('en-US', {minimumFractionDigits:0});
    }.bind(this);



    this.__confirmNetWorth = this._confirmNetWorth.bind(this);
  }

  _knowledgeCheck(e) {
    const val = parseInt(e.target.value);

    if(val === 0) {
      const modal = document.getElementById('modal');
      modal.appendChild(
        document.importNode(
          document.getElementById('knowledgeErrorTemplate').content, true
        )
      );
      modal.dataset.title = 'Inadequate knowledge';
      modal.opened = true;
      modal.querySelector('.options input[value="OK"]').addEventListener('click', () => {
        validation['knowledge'] = false;
        modal.opened = false;
      });
    } else {
      validation['knowledge'] = true;
    }

    this.activeClient.knowledge = val;
  }

  _confirmNetWorth(e) {
    if(this.activeClient.networth.total > 0) return true;

    e.preventDefault();

    const modal = document.getElementById('modal');
    modal.appendChild(
      document.importNode(
        document.getElementById('confirmNetworthTotal').content, true
      )
    );

    let yesHandler, noHandler;
    yesHandler = function () {
      modal.opened = false;
      modal.querySelector('.options input[value="Yes"]').removeEventListener('click', yesHandler);
      window.location.hash = e.path.find(el => el.nodeName.toLocaleLowerCase() === 'a').hash;
    };
    noHandler = function() {
      modal.opened = false;
      modal.querySelector('.options input[value="No"]').removeEventListener('click', noHandler);
    };
    modal.querySelector('.options input[value="Yes"]').addEventListener('click', yesHandler);
    modal.querySelector('.options input[value="No"]').addEventListener('click', noHandler);
    modal.dataset.title = '$0 Net Worth';
    modal.opened = true;
  }

  activateView(client) {
    this.activeClient = client;

    this.view.style.display = 'grid';

    this.personalInfoPanel.dataset.title = client.name;
    this.dateOfBirth.value = (client.dateOfBirth) ? client.dateOfBirth.format('MMMM D, YYYY') : null;
    this.annualIncome.value = client.annualIncome.toLocaleString('en-US', {minimumFractionDigits:0});
    this.householdIncome.value = client.householdIncome.toLocaleString('en-US', {minimumFractionDigits:0});

    this.networthPanel.querySelector('#liquidAssets').value = client.networth.liquidAssets.toLocaleString('en-US', {minimumFractionDigits:0});
    this.networthPanel.querySelector('#fixedAssets').value = client.networth.fixedAssets.toLocaleString('en-US', {minimumFractionDigits:0});
    this.networthPanel.querySelector('#liabilities').value = client.networth.liabilities.toLocaleString('en-US', {minimumFractionDigits:0});
    this.networthPanel.querySelector('#networthTotal').value = client.networth.total.toLocaleString('en-US', {minimumFractionDigits:0});

    client.addEventListener('householdIncomeChanged', this.householdChangeHandler);
    client.networth.addEventListener('*Changed', this.networthChangeHandler);

    validation = new ValidationModel('/client/'+client.id+'/info', [
      ['dateOfBirth', client.dateOfBirth !== undefined],
      ['knowledge', client.knowledge !== undefined]
    ]);

    const form = document.forms['client_info'];
    for(let radio of form.elements['knowledge']) {
      radio.addEventListener('click', this._knowledgeCheck.bind(this));
    }

    if(client.knowledge !== undefined) {
      this.view.querySelector(`input[name="knowledge"][value="${client.knowledge}"]`).checked = true;
    } else {
      this.view.querySelectorAll('input[name="knowledge"]').forEach(radio => [radio.checked = false]);
    }

    for (let note_name in this.activeClient.notes) {
      if(this.activeClient.notes.hasOwnProperty(note_name)) {
        form.elements[note_name].value = this.activeClient.notes[note_name];
      }
    }

    if(!initialized) {
      this.view.querySelectorAll('.notes').forEach(note_field => {
        note_field.addEventListener('richTextSaved', (e) => this.activeClient.notes[e.target.name] = e.target.value);
      });
      initialized = true;
    }
    document.getElementById('navigation').addEventListener('click', this.__confirmNetWorth);
  }

  deactivateView() {
    this.view.style.display = 'none';
    if(! this.activeClient) return;

    this.activeClient.removeEventListener('householdIncomeChanged', this.householdChangeHandler);
    this.activeClient.networth.removeEventListener('*Changed', this.householdChangeHandler);
    const form = document.forms['client_info'];
    for(let radio of form.elements['knowledge']) {
      radio.removeEventListener('click', this._knowledgeCheck);
    }
    document.getElementById('navigation').removeEventListener('click', this.__confirmNetWorth);
  }
}
