'use strict';

import {createObservableProxy, Observable} from "../Observable";
import {create as createProfile, update as updateProfile} from '../../api/profileApi';
import {fetchAllocationRecommendation} from "../../api/allocationApi";

let lastInternalID = 0;

let Ranges;
let Version;

const presets = {
  '80i20e': {
    income: 80,
    ce:   7,
    use:  7,
    inte: 6
  },
  '70i30e': {
    income: 70,
    ce:   11,
    use:  10,
    inte: 9
  },
  '60i40e': {
    income: 60,
    ce:   14,
    use:  14,
    inte: 12
  },
  '50i50e': {
    income: 50,
    ce:   18,
    use:  17,
    inte: 15
  },
  '40i60e': {
    income: 40,
    ce:   21,
    use:  21,
    inte: 18
  },
  '30i70e': {
    income: 30,
    ce:   25,
    use:  24,
    inte: 21
  },
  '20i80e': {
    income: 20,
    ce:   28,
    use:  28,
    inte: 24
  },
  '100e': {
    income: 0,
    ce:   35,
    use:  35,
    inte: 30
  }
};

class RiskTolerance extends Observable{
  constructor() {
    super();

    this.high = 0;
    this.mediumHigh = 0;
    this.medium = 0;
    this.lowMedium = 0;
    this.low = 0;
  }
}

class Objectives extends Observable {
  constructor() {
    super();

    this.aggressiveGrowth = 0;
    this.growth = 0;
    this.balanced = 0;
    this.income = 0;
    this.cashReserve = 0;
  }
}

class Profile extends Observable {
  constructor(name, accounts, primary, joint) {
    super();
    this.id = ++lastInternalID;
    this.name = name;
    this.accounts = accounts;
    this.timeHorizon = undefined;

    if (! primary) {
      throw "Cannot have profile without at least single client owner";
    }
    this.primaryClient = primary;
    this.jointClient = joint;

    this.initialInvestment = 0;
    this.monthlyCommitment = 0;
    this.decline = undefined;
    this.annualReturn = undefined;
    this.currentInvestment = undefined;

    this.objectives = new (createObservableProxy(Objectives));
    this.riskTolerance = new (createObservableProxy(RiskTolerance));

    this.score = 0;
    this.resultPortfolio = undefined;
    this.breakdown = undefined;

    this.exceptions = [];

    this.useClassF = false;
    this.useClassP = false;

    this.alternativeComposition = 0;

    this.notes = {
      objectives_notes: '',
      risk_notes: '',
      timehorizon_notes: ''
    };
  }

  get isJoint() {
    return !!(this.jointClient);
  }

  get incomeScore() {
    let totalIncome = this.primaryClient.annualIncome;
    if(this.isJoint) {
      totalIncome = totalIncome + this.jointClient.annualIncome;
    }

    let householdIncome;

    switch (true) {
      case (totalIncome <= 40000):
        householdIncome = 0;
        break;
      case (totalIncome <= 75000):
        householdIncome = 4;
        break;
      case (totalIncome <= 125000):
        householdIncome = 9;
        break;
      case (totalIncome <= 200000):
        householdIncome = 14;
        break;
      case (totalIncome > 200000):
        householdIncome = 17;
    }

    return householdIncome;
  }

  addScore() {
    let clientWeight;

    if(this.isJoint) {
      clientWeight = Math.floor((this.primaryClient.weight + this.jointClient.weight) / 2);
    } else {
      clientWeight = this.primaryClient.weight;
    }

    let total = clientWeight +
      this.incomeScore +
      this.objectives.aggressiveGrowth * 0.17 +
      this.objectives.growth * 0.14 +
      this.objectives.balanced * 0.09 +
      this.objectives.income * 0.04 +
      // cashReserve has weight 0

      this.riskTolerance.high * 0.25 +
      this.riskTolerance.mediumHigh * 0.16 +
      this.riskTolerance.medium * 0.11 +
      this.riskTolerance.lowMedium * 0.05 +
      // low has weight 0

      (this.timeHorizon || 0) +

      (this.decline || 0) +
      (this.annualReturn || 0) +
      (this.currentInvestment || 0);

    const exceptions = [];

    // correct risk
    const isLowMedium = this.riskTolerance.lowMedium === 100,
      isLow = this.riskTolerance.low === 100;

    if(total > Ranges['80i20e'].max && isLow) {
      total = Ranges['80i20e'].max;
      exceptions.push('comply with 100% low risk tolerance.');
    } else if(total > Ranges['30i70e'].max && (isLowMedium || isLow)) {
      total = Ranges['30i70e'].max;
      exceptions.push('comply with 100% low '+(isLowMedium ? ' to medium ' : '') +'risk tolerance.');
    }

    // correct objectives
    const isBalanced = this.objectives.balanced === 100,
      isIncome = this.objectives.income === 100,
      isCashReserve = this.objectives.cashReserve === 100;
    let complyName;
    switch (true) {
      case isBalanced:
        complyName = 'balanced';
        break;
      case isIncome:
        complyName = 'income';
        break;
      case isCashReserve:
        complyName = 'cash reserve';
        break;
    }

    if(total > Ranges['100i'].max && (isIncome || isCashReserve)) {
      total = Ranges['100i'].max;
      exceptions.push('comply with 100% '+complyName+' objective.');
    } else if(total > Ranges['40i60e'].max && (isBalanced || isIncome || isCashReserve)) {
      total = Ranges['40i60e'].max;
      exceptions.push('comply with 100% '+complyName+' objective.');
    }

    this.exceptions = exceptions;
    this.score = Math.floor(total);

    return total;
  }

  async calculatePlan() {
    let result,
      score = this.addScore();

    await fetchAllocationRecommendation(score, Version)
        .then(rs => result = rs);

    this.resultPortfolio = result;
    this.breakdown = presets[result];
  }

  get url() {
    return `${(! this.isJoint)?`/client/${this.primaryClient.id}` : ''}/profile/${this.id}`;
  }

  save() {
    const profile = this;
    this.primaryClient.meetingId = this.meetingId;

    console.log('profile save start', this  );
    return this.primaryClient.save()
      .then(() => {
        if(profile.isJoint) {
          profile.jointClient.meetingId = profile.meetingId;
          return profile.jointClient.save();
        }
      })
      .then(() => {
        if(! profile.isCreated) {
          return createProfile(profile);
        } else {
          return updateProfile(profile);
        }
      });
  }
}

export function parseAccounts(fields) {
  const accounts = [];
  for(let i = 0, totalFields = fields.length; i < totalFields; i++) {
    if(fields[i].checked) {
      accounts.push(fields[i].value)
    }
  }

  return accounts;
}

export function injectAvailableAllocationRanges(allocations) {
  Ranges = allocations;
}

export function setVersion(version) {
  Version = version;
}

export default createObservableProxy(Profile);

export function createProfileFromDraft(profileDraft) {
  const profile = new (createObservableProxy(Profile))(
    profileDraft.name,
    profileDraft.accounts,
    profileDraft.primaryClient,
    profileDraft.jointClient
  );

  for (let prop in profileDraft.objectives) {
    if(profileDraft.objectives.hasOwnProperty(prop)) {
      profile.objectives[prop] = profileDraft.objectives[prop];
    }
  }

  delete profileDraft.objectives;

  for (let prop in profileDraft.riskTolerance) {
    if(profileDraft.riskTolerance.hasOwnProperty(prop)) {
      profile.riskTolerance[prop] = profileDraft.riskTolerance[prop];
    }
  }

  delete profileDraft.riskTolerance;

  delete profileDraft.primaryClientId;
  delete profileDraft.jointClientId;

  for (let prop in profileDraft) {
    if(profileDraft.hasOwnProperty(prop)) {
      profile[prop] = profileDraft[prop];
    }
  }

  return profile;
}
