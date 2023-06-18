import {dto as meetingDTO} from './meetingApi';
import {dto as clientDTO} from './clientApi';
import {dto as profileDTO} from './profileApi';

const URL = (window.settings) ? window.settings.baseUrl : '';

function dto(meeting, clientStore, profileStore, navigationVM) {
  const clients = [];
  // meeting will not initially have any clients
  if(clientStore.primaryClient) {
    clients.push(clientDTO(clientStore.primaryClient));
  }
  if(clientStore.jointClient) {
    clients.push(clientDTO(clientStore.jointClient));
  }

  const profiles = [];
  profileStore.profiles.forEach(p => profiles.push(profileDTO(p)));
  return {
    meeting: meetingDTO(meeting),
    clients: clients,
    profiles: profiles,
    navigation: navigationVM
  }
}

function sendRequest(request) {
  let expectedStatus;
  switch (request.method) {
    case 'PUT':
      expectedStatus = 202;
  }

  return fetch(request)
    .then(response => {
      if(response.status === expectedStatus) {
        console.log('meeting saved');
        return response.json();
      } else {
        throw new Error('Problem saving meeting.');
      }
    });
}

export function save(meeting, clientStore, profileStore, navigationVM) {
  const draftRequest = new Request(URL+'meeting/'+meeting.id+'/draft', {
    method: 'PUT',
    credentials: 'include',
    body: JSON.stringify(dto(meeting, clientStore, profileStore, navigationVM))
  });

  return sendRequest(draftRequest);
}
