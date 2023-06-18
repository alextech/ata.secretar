import fetchMock from 'fetch-mock';
import {fetchAllocationRanges} from "./allocationApi";

const mockAllocations = {
  "_links": {
    "self": {},
    "recommendations": {}
  },
  "_embedded": [
    {
      "_links": {
        "self": {
          "href": "/api/Allocations/100e/1803",
          "method": "Get"
        }
      },
      "name": "100e",
      "version": 1803,
      "scoreRange": {
        "min": 136,
        "max": 150
      },
      "options": []
    },
    {
      "_links": {
        "self": {
          "href": "/api/Allocations/100i/1803",
          "method": "Get"
        }
      },
      "name": "100i",
      "version": 1803,
      "scoreRange": {
        "min": 0,
        "max": 0
      },
      "options": []
    }
  ]
};

describe('Allocation Api Test', () => {
  beforeEach(() => {
    fetchMock.restore();

    fetchMock.get('end:/allocations/?version=1803',
        mockAllocations
    );
  });

  it('Should be able to give allocations by version as map of Name=>ScoreRange', async () => {
    const ranges = await fetchAllocationRanges(1803);

    expect(ranges.hasOwnProperty('100e')).to.be.true;
    expect(ranges.hasOwnProperty('100i')).to.be.true;

    expect(ranges['100e']).to.deep.equal({min: 136, max: 150});
    expect(ranges['100i']).to.deep.equal({min: 0, max: 0});
  });
});
