import {expect} from 'chai';
import {Client, ClientStore} from './index';

describe('ClientStore event dispatching', () => {

  it('should dispatch ClientAdded when adding new client and registered', () => {
    let addedListenerCalled = false;
    let c1 = new Client();
    let clientStore = new ClientStore();

    let addedListener = function (e) {
      expect(e).to.have.property('target');
      expect(e.target).to.equal(clientStore);
      expect(e).to.have.property('details');
      expect(e.details.client).to.equal(c1);

      addedListenerCalled = true;
    };

    clientStore.addEventListener('clientAdded', addedListener);
    clientStore.addClient(c1);

    expect(addedListenerCalled).to.equal(true);
    addedListenerCalled = false;

    clientStore.removeEventListener('clientAdded', addedListener);
    clientStore.addClient(c1);

    expect(addedListenerCalled).to.equal(false);
  });

});

describe('ClientStore retrieval', () => {
  it('should return primary client when first client is added, joint when second', () => {
    let c1 = new Client();
    let c2 = new Client();
    let store = new ClientStore();

    store.addClient(c1);
    store.addClient(c2);
    expect(store.primaryClient).to.eql(c1);
    expect(store.jointClient).to.eql(c2);
    expect(store.fetch(c1.id.toString())).to.eql(c1);
    expect(store.fetch(c2.id.toString())).to.eql(c2);
  });

  it('should include annual income from both clients', () => {
    let c1 = new Client(),
      c2 = new Client(),
      store = new ClientStore();

    c1.annualIncome = 100;
    c2.annualIncome = 50;

    store.addClient(c1);
    expect(c1.householdIncome).to.equal(100);
    store.addClient(c2);
    expect(c1.householdIncome).to.equal(150);
    expect(c2.householdIncome).to.equal(150);
  });

  it('should replicate networth for both clients', () => {
    let c1 = new Client(),
      c2 = new Client(),
      store = new ClientStore();

    store.addClient(c1);
    store.addClient(c2);
    c1.networth.fixedAssets = 100;
    expect(c1.networth.fixedAssets).to.equal(100);
    expect(c2.networth.fixedAssets).to.equal(100);
    // c2.networth.fixedAssets = 20;
    // expect(c1.networth.fixedAssets).to.equal(120);
    // expect(c2.networth.fixedAssets).to.equal(120);
  });
});

describe('ClientStore validation', () => {
  it('should throw exception when adding more than two clients', () => {

  });
});
