const URL = (window.settings) ? window.settings.baseUrl : 'http://localhost/';

let recommendationURL;

export function fetchAllocationRecommendation(score, version) {

  // TODO version may be flexible as advisor (in the future) could pick different version to view profile as
  let url = recommendationURL+'&score='+score;

  const recommendationRequest = new Request(url, {
    method: 'GET',
    mode: "cors",
    credentials: 'include'
  });

  return fetch(recommendationRequest)
    .then(response => {
      if (response.status !== 200) {
        throw new Error('Could not find allocation recommendation for score'+score);
      }

      return response.json();
    })
    .catch(error => {
      console.log('Allocation API request failed', error);
    });
}

export function fetchAllocationRanges(version) {
  const url = URL+'allocations/?version='+version;

  const rangesRequest = new Request(url, {
    method: 'GET',
    mode: "cors",
    credentials: "include"
  });

  return fetch(rangesRequest)
      .then(response => {
        if (response.status !== 200) {
          throw new Error('Could fetch recommendation ranges for version '+version);
        }

        return response.json();
      })
      .then(allocations => {
        const ranges = {};

        for(let allocation of allocations._embedded) {
          ranges[allocation.name] = allocation.scoreRange;
        }

        recommendationURL = allocations._links.recommendations.href;

        return ranges;
      })
      .catch(error => {
        console.log('Ranges API request failed', error);
      });
}
