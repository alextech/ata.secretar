const URL = (window.settings) ? window.settings.baseUrl : '';

export function dto(profile) {
  return {
    id: profile.id+'',
    isCreated: profile.isCreated,
    primaryClientId: profile.primaryClient.id+'',
    jointClientId: (profile.jointClient) ? profile.jointClient.id+'' : null,
    meetingId: profile.meetingId,
    icon: 'fa-dollar',
    name: profile.name,
    accounts: profile.accounts,
    initialInvestment: profile.initialInvestment,
    monthlyCommitment: profile.monthlyCommitment,

    objectives: {
      aggressiveGrowth: profile.objectives.aggressiveGrowth,
      growth: profile.objectives.growth,
      balanced: profile.objectives.balanced,
      income: profile.objectives.income,
      cashReserve: profile.objectives.cashReserve,
    },

    riskTolerance: {
      high: profile.riskTolerance.high,
      mediumHigh: profile.riskTolerance.mediumHigh,
      medium: profile.riskTolerance.medium,
      lowMedium: profile.riskTolerance.lowMedium,
      low: profile.riskTolerance.low,
    },


    timeHorizon: profile.timeHorizon,

    decline: profile.decline,
    annualReturn: profile.annualReturn,
    currentInvestment: profile.currentInvestment,

    useClassF: profile.useClassF,
    useClassP: profile.useClassP,

    score: profile.score,
    resultPortfolio: profile.resultPortfolio,
    breakdown: profile.breakdown,

    alternativeComposition: profile.alternativeComposition,

    notes: profile.notes,
    exceptions: profile.exceptions
  };
}

function sendRequest(profileRequest) {
  let expectedStatus;
  switch (profileRequest.method) {
    case 'POST':
      expectedStatus = 201;
      break;
    case 'PUT':
      expectedStatus = 202;
  }

  return fetch(profileRequest)
    .then(response => {
      if(response.status === expectedStatus) {
        return response.json();
      } else {
        throw new Error('Problem saving profile.');
      }
    });
}

export function update(profile) {
  const profileRequest = new Request(URL+'profiles/'+profile.id, {
    method: 'PUT',
    credentials: 'include',
    body: JSON.stringify(dto(profile))
  });

  return sendRequest(profileRequest);
}

export function create(profile) {
  const profileRequest = new Request(URL+'profiles', {
    method: 'POST',
    credentials: 'include',
    body: JSON.stringify(dto(profile))
  });

  return sendRequest(profileRequest, profile)
    .then(function (response) {
      profile.oldID = profile.id;
      profile.id = response.id;
      profile.isCreated = true;
    });
}
