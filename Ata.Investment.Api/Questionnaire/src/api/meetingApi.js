const URL = (window.settings) ? window.settings.baseUrl : '';

export function dto(meeting) {
  return {
    date: meeting.date.unix(),
    advisor: meeting.advisor,
    isJoint: meeting.isJoint,
    otherAttendees: meeting.otherAttendees,
    purpose: meeting.purpose
  };
}

function sendRequest(request) {
  let expectedStatus;
  switch (request.method) {
    case 'POST':
      expectedStatus = 201;
      break;
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

export function update(meeting) {
  const meetingRequest = new Request(URL+'meeting/'+meeting.id, {
    method: 'PUT',
    credentials: 'include',
    body: JSON.stringify(dto(meeting)),
  });

  return sendRequest(meetingRequest);
}
