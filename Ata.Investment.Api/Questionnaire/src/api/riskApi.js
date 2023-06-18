const URL = (window.settings) ? window.settings.baseUrl : 'http://localhost/';

export function fetchRiskForPortfolio(fundCode, initial, monthly) {
  let url = URL+'portfolio/'+fundCode+'/risk?initial='+initial;
  if (monthly > 0) {
    url += '&'+'monthly='+monthly;
  }

  const riskRequest = new Request(url, {
    method: 'GET',
    credentials: 'include'
  });

  return fetch(riskRequest)
    .then(response => {
      if (response.status !== 200) {
        throw new Error('Problem fetching risk for portfolio'+fundCode);
      }

      return response.json();
    })
}
