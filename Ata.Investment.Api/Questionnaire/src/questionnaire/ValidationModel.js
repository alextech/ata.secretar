'use strict';
function checkInvalid(target) {
  let hasInvalid = false;
  target.fields.forEach(function (status, field) {
    if (status === false) {
      hasInvalid = true;
    }
  });
  return hasInvalid;
}


class ValidationModel {

  constructor(callbacks, fields) {

    this.callbacks = callbacks;
    this._falsy = true;
    this._fields = new Map();
    for(let i = 0, numFields = fields.length; i < numFields; i++) {
      let field = fields[i];
      if(Array.isArray(field)) {
        this._fields.set(field[0], field[1]);
      } else if(typeof field === 'string') {
        this._fields.set(fields[i], false);
      } else {
        throw `Field ${field} is not a string or array name of field to validate.`;
      }
    }

    this.falsy = checkInvalid(this);
  }

  get fields() {
    return this._fields;
  }

  get falsy() {
    return this._falsy;
  }

  set falsy(value) {
    if(value === this._falsy) {
      return;
    }

    if(value) {
      this.callbacks.onFalsy();
    } else {
      this.callbacks.onTruthy();
    }

    this._falsy = value;
  }
}

export default class ValidationFactory {
  constructor(target, fields, callbacks) {

    let handlers;
    if(typeof target === 'string') {
      const navigation = document.getElementById('navigation');

      handlers = {
        onFalsy: function() {
          if(callbacks) {
            callbacks.onInvalid();
          }
          navigation.disablePath(target);
        },
        onTruthy: function() {
          if(callbacks) {
            callbacks.onValid();
          }
          navigation.enablePath(target);
        }
      }
    } else {
      handlers = {
        onFalsy: target.onInvalid,
        onTruthy: target.onValid
      }
    }


    return new Proxy(new ValidationModel(handlers, fields), {
      set: function (target, name, isValid) {
        target.fields.set(name, isValid);

        // if new field validity status is different than overall model, there is a chance whole model may have become valid
        // if it is same, then it will remain in (in)valid state.
        if(target.falsy !== isValid) {
          return true;
        }

        target.falsy = checkInvalid(target);

        return true;
      }
    });
  }
}
