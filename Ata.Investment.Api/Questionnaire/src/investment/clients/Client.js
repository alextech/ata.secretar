'use strict';
import moment from 'moment';
import {createObservableProxy, Observable} from "../Observable";

import {create as createClient, update as updateClient} from "../../api/clientApi";

let lastInternalID = 0;

class NetWorth extends Observable {
  constructor() {
    super();
    this.liquidAssets = 0;
    this.fixedAssets = 0;
    this.liabilities = 0;
  }

  get total() {
    return this.liquidAssets +
      this.fixedAssets -
      this.liabilities;
  }
}


class Client extends Observable {
  constructor(name, email) {
    super();
    this.id = ++lastInternalID;

    this.name = name;
    this.email = email;
    this.isInitiator = false;

    this.dateOfBirth = undefined;
    this._annualIncome = 0;

    this.networth = new (createObservableProxy(NetWorth));
    this.knowledge = undefined;

    this.notes = {
      personal_notes: '',
      networth_notes: '',
      knowledge_notes: ''
    }
  }

  set jointWith(jointClient) {
    if(jointClient === this._jointClient) return;

    this._jointClient = jointClient;
    jointClient.networth = this.networth;
    jointClient.jointWith = this;
  }

  removeJoint() {
    this._jointClient = null;
  }

  get age() {
    return moment().diff(this.dateOfBirth, 'years');
  }

  set annualIncome(income) {
    this._annualIncome = income;

    let e = {
      target: this,
      type: 'householdIncomeChanged',
      details: {property: 'householdIncome', value: this.householdIncome}
    };
    super.dispatchEvent(e);
  }

  get annualIncome() {
    return this._annualIncome;
  }

  get householdIncome() {
    if(this._jointClient) {
      return this.annualIncome + this._jointClient.annualIncome;
    }
    return this.annualIncome ;
  }

  get weight() {
    const

      age = this.age,
      knowledge = this.knowledge,

      totalNetWorth = this.networth.total;

    let investableAssets,
      agePoints
    ;

    switch (true) {
      case (age > 65):
        agePoints = 0;
        break;
      case (age >= 51):
        agePoints = 2;
        break;
      case (age >= 41):
        agePoints = 4;
        break;
      case (age >= 31):
        agePoints = 6;
        break;
      default:
        agePoints = 8;
    }

    switch (true) {
      case(totalNetWorth <= 25000):
        investableAssets = 0;
        break;
      case(totalNetWorth <= 75000):
        investableAssets = 4;
        break;
      case(totalNetWorth <= 150000):
        investableAssets = 9;
        break;
      case(totalNetWorth <= 300000):
        investableAssets = 14;
        break;
      case(totalNetWorth > 300000):
        investableAssets = 17;
    }

    return agePoints + investableAssets + knowledge;
  }

  save() {
    console.log('client save start');
    if(! this.isCreated) {
      return createClient(this);
    } else {
      return updateClient(this);
    }
  }
}

export default createObservableProxy(Client);

export function createClientFromDraft(clientDraft) {
  const client = new (createObservableProxy(Client))();
  clientDraft.dateOfBirth = moment.unix(clientDraft.dateOfBirth);
  for(let prop in clientDraft.networth) {
    if(clientDraft.networth.hasOwnProperty(prop)){
      client.networth[prop] = clientDraft.networth[prop];
    }
  }

  delete clientDraft.networth;

  for(let prop in clientDraft) {
    if(clientDraft.hasOwnProperty(prop)){
      client[prop] = clientDraft[prop];
    }
  }

  return client;
}
