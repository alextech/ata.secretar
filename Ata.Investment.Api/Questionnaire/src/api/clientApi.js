const URL = (window.settings) ? window.settings.baseUrl : '';

export function dto(client) {
  return {
    id: client.id+'',
    isCreated: client.isCreated,
    name: client.name,
    email: client.email ? client.email : '',
    dateOfBirth: (client.dateOfBirth) ? client.dateOfBirth.unix() : null,
    annualIncome: client.annualIncome,

    networth: {
      liquidAssets: client.networth.liquidAssets,
      fixedAssets: client.networth.fixedAssets,
      liabilities: client.networth.liabilities,
    },

    knowledge: client.knowledge,

    meetingId: client.meetingId,

    isInitiator: client.isInitiator,

    notes: client.notes
  };
}

function sendRequest(clientRequest) {
  let expectedStatus;
  switch (clientRequest.method) {
    case 'POST':
      expectedStatus = 201;
      break;
    case 'PUT':
      expectedStatus = 202;
  }

  return fetch(clientRequest)
    .then(response => {
      if(response.status === expectedStatus) {
        return response.json();
      } else {
        throw new Error('Problem saving client.');
      }
    });
}

export function create(client) {
  const clientRequest = new Request(URL+'clients', {
    method: 'POST',
    credentials: 'include',
    body: JSON.stringify(dto(client))
  });

  return sendRequest(clientRequest)
    .then(c => {
      client.oldID = client.id;
      client.id = c.id;
      client.isCreated = true;
    });
}

export function update(client) {
  const clientRequest = new Request(URL+'clients/'+client.id, {
    method: 'PUT',
    credentials: 'include',
    body: JSON.stringify(dto(client))
  });

  return sendRequest(clientRequest);
}
