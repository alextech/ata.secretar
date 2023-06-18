'use strict';
import {Observable} from "../Observable";

export const ProfileAddedEvent = 'profileAdded';
export const ProfileRemoveEvent = 'profileRemoved';

export default class ProfileStore extends Observable {
  constructor() {
    super();
    this.profiles = [];
  }

  addProfile(profile) {
    this.profiles.push(profile);

    super.dispatchEvent({
      target: this,
      type: ProfileAddedEvent,
      details: {profile: profile}
    });
  }

  removeProfile(profile) {
    const index = this.profiles.indexOf(profile);
    this.profiles.splice(index, 1);

    super.dispatchEvent({
      target: this,
      type: ProfileRemoveEvent,
      details: {profile: profile}
    });
  }

  fetch(profileId) {
    const profile = this.profiles.find(p => p.id.toString() === profileId || (p.oldID && p.oldID.toString() === profileId));
    if(!profile) {
      throw "Requested non-existent profile" + profileId;
    }

    return profile;
  }

  findForClient(client) {
    return this.profiles.filter(p => p.primaryClient.id === client.id || (p.jointClient && p.jointClient.id === client.id));
  }
}
