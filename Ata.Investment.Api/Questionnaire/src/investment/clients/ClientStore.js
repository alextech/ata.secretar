'use strict';
import {Observable} from "../Observable";

export const ClientAddedEvent = 'clientAdded';
export const ClientRemovedEvent = 'clientDeleted';

export default class ClientStore extends Observable {
  addClient(client) {
    if(this._primary === undefined) {
      this._primary = client;
    } else {
      this._primary.jointWith = client;
      client.jointWith = this._primary;
      this._joint = client;
    }

    super.dispatchEvent({
      target: this,
      type: ClientAddedEvent,
      details: {client: client}
    });
  }

  get primaryClient() {
    return this._primary;
  }

  get jointClient() {
    return this._joint;
  }

  removeJoint() {
    const tmpClient = this._joint;
    this._joint = undefined;
    this._primary.removeJoint();

    super.dispatchEvent({
      target: this,
      type: ClientRemovedEvent,
      details: {client: tmpClient}
    });
  }

  fetch(clientId) {
    // toString because initial numeric ID will eventually become UUID
    if((this._primary && this._primary.id.toString() === clientId) ||
      (this._primary.oldId && this._primary.oldID.toString() === clientId)
    ) {
      return this._primary;
    }

    if((this._joint && this._joint.id.toString() === clientId) ||
      (this._joint.oldID && this._joint.oldID.toString() === clientId)) {
      return this._joint;
    }

    throw "Requested non-existent client "+clientId;
  }
}
