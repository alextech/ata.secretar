const URL = (window.settings) ? window.settings.baseUrl : 'http://localhost/';

export function fetchAlternativeComposition(fundCode, version) {
  let url = URL+'portfolio/'+fundCode+'/alternative_compositions/'+version;

  const riskRequest = new Request(url, {
    method: 'GET',
    credentials: 'include'
  });

  return fetch(riskRequest)
    .then(response => {
      if (response.status !== 200) {
        throw new Error('Problem fetching alternative composition for portfolio'+fundCode);
      }

      return response.json();
    })
}
