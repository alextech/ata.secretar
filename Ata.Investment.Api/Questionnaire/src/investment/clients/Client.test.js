'use strict';
import {expect} from 'chai';
import Client from "./Client";
import {ClientStore} from "./index";
import moment from 'moment';
import MockDate from 'mockdate';

describe('Client Entity test', () => {
  it('should generate incremental internal ID on new instance creation', () => {
    const client1 = new Client('TestClient1');
    const client2 = new Client('TestClient2');
    expect(client1.id).to.equal(1);
    expect(client2.id).to.equal(2);
  });

  it('should use new ID given to it by server when saved', () => {

  });

  it('should notify observer on name change', () => {
    const client1 = new Client('ObservableClient1');
    let nameChangeCalled = false;

    const observer = function (e) {
      nameChangeCalled = true;

      expect(e).to.have.property('target');
      expect(e.target).to.equal(client1);
      expect(e).to.have.property('details');
      expect(e.details.property).to.equal('name');
      expect(e.details.value).to.equal(e.details.value);
    };

    client1.addEventListener('nameChanged', observer);

    client1.name = 'TestClient1';
    expect(nameChangeCalled).to.be.true;

    nameChangeCalled = false;
    client1.removeEventListener('nameChanged', observer);
    client1.name = 'Unnoticed Change';
    expect(nameChangeCalled).to.be.false;
  });

  it('should notify observers on id change when saved', () => {
    // others should not be able to arbitrarily change ID.
    // change would come from act of being saved and internal ID switched to one
    // supplied by server
    // client1.save();
  });

  it('should update networth total on network change', () => {
    const client = new Client('NetWorth Test', 'networth@client.test'),
      networth = client.networth;

    networth.liquidAssets = 100;
    expect(networth.total).to.equal(100);
    networth.fixedAssets = 200;
    expect(networth.total).to.equal(300);
    networth.liabilities = 50;
    expect(networth.total).to.equal(250);
  })
});

describe('Client business logic', () => {
  it('should calculate age', () => {
    MockDate.set('11/05/2018');

    const client = new Client('TestClient');
    client.dateOfBirth = moment('April 14, 1989', ['MMMM DD, YYYY'], true);
    expect(client.age).to.equal(29);

    MockDate.reset();
  });
});
