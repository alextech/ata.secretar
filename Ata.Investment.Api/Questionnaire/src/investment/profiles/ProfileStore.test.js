'use strict';

import Client from '../clients/Client';
import Profile from './Profile';
import ProfileStore from './ProfileStore';

describe('Profile Store test', () => {
  it('should find profiles for client', () => {
    const profileStore = new ProfileStore();
    let c1 = new Client(),
    c2 = new Client(),
    c3 = new Client();


    const p1 = new Profile('p1', [], c1),
      p2 = new Profile('p2', [], c1),
      p3 = new Profile('p3', [], c3, c1);

    profileStore.addProfile(p1); // single
    profileStore.addProfile(p2); // single

    profileStore.addProfile(p3); // joint

    let result = profileStore.findForClient(c1);
    expect(result).to.be.an('array').that.includes.members([p1, p2, p3]);
    result = profileStore.findForClient(c2);
    expect(result).to.be.an('array').that.is.empty;
    result = profileStore.findForClient(c3);
    expect(result).to.be.an('array').that.includes(p3);
  });
});
