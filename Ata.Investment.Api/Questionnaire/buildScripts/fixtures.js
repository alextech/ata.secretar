import Client from '../src/investment/clients/Client'
import Profile from '../src/investment/profiles/Profile';
import moment from "moment";

export function createProfileFixture() {
  const client = new Client('Karma Fixture', 'fixture@karmarunner.local');
  client.dateOfBirth = moment('20100101', 'YYYYMMDD');
  client.knowledge = 0;

  const profile = new Profile('results test fixture', [], client);
  profile.timeHorizon = 0;
  profile.decline = 0;
  profile.annualReturn = 0;
  profile.currentInvestment = 0;

  return profile;
}
