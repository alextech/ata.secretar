// import './index.less';
import './index.scss';
import Navigo from 'navigo';
import Pusher from 'pusher-js';
import './navigation';
import './questionnaire/Panel';
import './Numeric';
import './RichEditor';
import './questionnaire/percentage';
import './questionnaire/AllocationChart';
import './questionnaire/Modal';

import 'admin-lte/build/js/Tree';

import NavigationController from "./navigation/NavigationController";
import {ClientStore} from "./investment/clients";
import {ProfileStore} from "./investment/profiles";
import {MeetingController} from "./questionnaire/meeting/MeetingController";
import {createPathFromEvent, getElementFromPath} from "./PathUtils";
import {ClientInfoController} from "./questionnaire/clientinfo/ClientInfoController";
import {ObjectivesController} from "./questionnaire/profile/ObjectivesController";
import {ExpectationsController} from "./questionnaire/profile/ExpectationsController";
import {ResultsController} from "./questionnaire/profile/ResultsController";
import NewProfileController from "./questionnaire/profile/NewProfileController";
import Meeting, {createFromDraft} from "./investment/Meeting";

import {createClientFromDraft} from "./investment/clients/Client";
import {
  createProfileFromDraft,
  injectAvailableAllocationRanges,
  setVersion as setVersionForProfiles
} from "./investment/profiles/Profile";
import {applyToNavigation, createFromNavigation as createNavigationVM} from "./navigation/NavigationDraftVM";
import {fetchAllocationRanges} from "./api/allocationApi";
import {save as saveDraft} from './api/draftApi';

// import {init as initCollaboration} from "./Collaboration";
async function init() {

const drafted = window.settings &&
  ! (Object.keys(window.settings.draft).length === 0 && window.settings.draft.constructor === Object);

let meeting;
if(drafted) {
  meeting = createFromDraft(window.settings.draft.meeting);
} else {
  meeting = new Meeting();
}

const version = parseInt(document.forms['metadata'].elements['version'].value);

await fetchAllocationRanges(version)
    .then(ranges => injectAvailableAllocationRanges(ranges));

setVersionForProfiles(version);

// initCollaboration(meeting);

const clientStore = new ClientStore();
const profileStore = new ProfileStore();


const navigation = document.getElementById('navigation');

if(window.settings && window.settings.production) {
  let lock = false;

  //TODO better way to trigger saving of draft from anywhere.
  window.saveDraft = function() {
    const navigationVM = createNavigationVM(navigation);
    return saveDraft(meeting, clientStore, profileStore, navigationVM)
  };

  setInterval(function() {
    if(lock || window.draftLock) return;

    lock = true;
    window.saveDraft()
      .then(() => {
        lock = false;
      })
      .catch(() => {
        //TODO limit number of retries before showing and logging errors.
        lock = false;
        console.warn('Problem auto-saving meeting draft. Will continue retrying.');
      });
  }, (window.settings ? window.settings.autoSave : 5000));
}

const meetingController = new MeetingController(clientStore, profileStore, meeting);
new NavigationController(clientStore, profileStore);
if(drafted) {
  // =============== CLIENTS =========================
  for (let clientDraft of window.settings.draft.clients) {
    const client = createClientFromDraft(clientDraft);
    clientStore.addClient(client);
  }

  // ================= PROFILES ==============================
  for (let profileDraft of window.settings.draft.profiles) {
    profileDraft.primaryClient = clientStore.fetch(profileDraft.primaryClientId);
    if(profileDraft.jointClientId) {
      profileDraft.jointClient = clientStore.fetch(profileDraft.jointClientId);
    }
    const profile = createProfileFromDraft(profileDraft);
    profileStore.addProfile(profile);
  }

  applyToNavigation(window.settings.draft.navigation, navigation);
}

const clientInfoController = new ClientInfoController();
const objectivesController = new ObjectivesController(profileStore);
const expectationsController = new ExpectationsController();
const resultsController = new ResultsController(profileStore, meeting);
const newProfileController = new NewProfileController(profileStore, clientStore);

let router = new Navigo('/', true, '#');
// null object pattern
let currentController = {deactivateView: function () {}};

let currentTooltip = {
  hide: function() {}
};


const objectivesHandler = function(params) {
  currentTooltip.hide();
  let profile;
  try {
    profile = profileStore.fetch(params.profileID);
  } catch (e) {
    router.navigate('/');
    return;
  }

  currentController.deactivateView();
  currentController = objectivesController;
  objectivesController.activateView(profile);

  navigation.activate(profile.url+'/objectives');
};

const expectationsHandler = function(params) {
  currentTooltip.hide();
  let profile;
  try {
    profile = profileStore.fetch(params.profileID);
  } catch (e) {
    router.navigate('/');
    return;
  }

  currentController.deactivateView();
  currentController = expectationsController;
  expectationsController.activateView(profile);

  navigation.activate(profile.url+'/expectations');
};

const resultsHandler = function(params) {
  currentTooltip.hide();
  let profile;
  try {
    profile = profileStore.fetch(params.profileID);
  } catch (e) {
    router.navigate('/');
    return;
  }

  currentController.deactivateView();
  currentController = resultsController;
  resultsController.activateView(profile, meeting);

  navigation.activate(profile.url+'/results');
};

router.on({
  '*': function() {
    console.info('route: *');
    currentController.deactivateView();
    meetingController.activateView(meeting);
    currentController = meetingController;
    navigation.activate('/');
  },
  'client/:id/': function(params) {
    router.navigate('/client/'+params.id+'/info');
  },
  'client/:id/info': function(params) {
    currentTooltip.hide();
    console.info('route: client/'+params.id);
    //TODO do not deactivate if same controller with different ID. or maybe still activate, because need to reattach listeners to new ID
    currentController.deactivateView();
    try {
      const client = clientStore.fetch(params.id);
      navigation.activate('/client/'+client.id+'/info');
      clientInfoController.activateView(client);
    } catch (e) {
      router.navigate('/');
      return;
    }
    currentController = clientInfoController;
  },
  'client/:clientID/profile/add': function(params) {
    const prevCurrent = currentController;
    currentController = { deactivateView: function() {
        prevCurrent.deactivateView();
        newProfileController.deactivateView();
      }
    };
    newProfileController.activateView({
      addForClients: [
        params.clientID
      ],
      isJoint: false
    });
  },
  'client/:clientID/profile/:profileID/objectives': objectivesHandler,
  'client/:clientID/profile/:profileID/expectations': expectationsHandler,
  'client/:clientID/profile/:profileID/results': resultsHandler,
  'profile/add': function() {
    const prevCurrent = currentController;
    currentController = { deactivateView: function() {
        prevCurrent.deactivateView();
        newProfileController.deactivateView();
      }
    };
    newProfileController.activateView({
      addForClients:[
        clientStore.primaryClient.id,
        clientStore.jointClient.id
      ],
      isJoint: true
    });
  },
  'profile/:profileID/objectives': objectivesHandler,
  'profile/:profileID/expectations': expectationsHandler,
  'profile/:profileID/results': resultsHandler,
})
.resolve();


document.addEventListener('TooltipOpened', (e) => {
  if(currentTooltip !== e.detail.tooltip) {
    currentTooltip.hide();
  }
  currentTooltip = e.detail.tooltip;
});

if(window.settings.collaboration) {
  const pusher = new Pusher('d9c0cd5802788ffaee6b', {
    authEndpoint: '/auth',
    encrypted: true
  });

  // UUID of meeting
  const channel = pusher.subscribe('private-questionnaire-meeting');

  // ============= KEY UP =========================
  document.body.addEventListener('keyup', e => {
    let path = createPathFromEvent(e);
    if (path.length !== 0) {
      channel.trigger('client-keyup', {path: path, value: e.target.value});
    }
  });

  channel.bind('client-keyup', function (data) {
    console.log('received keyboard');
    const targetElement = getElementFromPath(data.path);
    targetElement.value = data.value;
    window.setTimeout(() => {
      targetElement.focus();
    }, 0);
    targetElement.dispatchEvent(new KeyboardEvent('input', {bubbles: true, composed: true}));
  });

  // ============ CLICK ==========================
  document.body.addEventListener('click', e => {
    //detail for MouseEvent('click') is number of clicks
    // -1 clicks is set at pipelistener
    if (e.detail === -1) return;
    const path = createPathFromEvent(e);
    if (path.length !== 0) {
      channel.trigger('client-click', {path: path});
    }

    if (e.target.tagName === 'SELECT') {
      e.target.dispatchEvent(new KeyboardEvent('keyup', {bubbles: true, composed: true}));
    }
  });

  channel.bind('client-click', function (data) {
    const targetElement = getElementFromPath(data.path);
    window.setTimeout(() => {
      targetElement.focus();
    }, 0);
    console.log('received to click on ', targetElement);
    const e = new MouseEvent('click', {
      bubbles: true,
      composed: true,
      cancelable: true,
      view: window,
      detail: -1
    });
    targetElement.dispatchEvent(e);
  });

  // ========== ROUTING ========================
  router.hooks({
    // after: function() {
    // if(history.state && history.state.isRemote) return;
    // console.log('send to pusher');

    // let hash = window.location.hash;
    // if(hash.length === 0 ) {
    //   hash = '#/';
    // }

    // channel.trigger('client-RouteChanged', {route: hash});
    // }
  });

  channel.bind('client-RouteChanged', function (data) {
    console.info('RouteChanged received from pusher: ', data);
    // history.pushState({isRemote: true}, '', data.route);
    // router.resolve();
  });
}
}

init();
