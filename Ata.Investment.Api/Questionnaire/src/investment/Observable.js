export class Observable {
  constructor() {
    this.observers = {};
  }

  dispatchEvent(e) {
    const observers = this.observers[e.type];
    if(observers === undefined) {
      return;
    }

    for(let i = 0, to = observers.length; i < to; i++) {
      observers[i](e);
    }
  }

  addEventListener(event, observer) {
    if(!this.observers.hasOwnProperty(event)) {
      this.observers[event] = [];
    }
    this.observers[event].push(observer);
  }

  removeEventListener(event, observer) {
    const index = this.observers[event].indexOf(observer);
    this.observers[event].splice(index, 1);
  }
}

export function createObservableProxy(subject, suffix) {
  if(!suffix) {
    suffix = 'Changed';
  }
  return new Proxy(subject, {
    construct: function(target, args) {
      const proxyTarget = new Proxy(new target(...args), {
        set: function(target, name, value) {
          let observers = target['observers'],
            eventName = name+suffix;
          const oldValue = target[name];
          target[name] = value;
          if(!observers.hasOwnProperty(eventName)) {
            eventName = '*'+suffix;
            if(!observers.hasOwnProperty(eventName)) {
              return true;
            }
          }

          observers = observers[eventName];
          let e = {
            target: proxyTarget,
            type: eventName,
            details: {property: name, value: value, oldValue: oldValue}
          };
          for(let i = 0, to = observers.length; i < to; i++) {
            observers[i](e);
          }

          return true;
        }
      });
      return proxyTarget;
    }
  });
}
