'use strict';

let statusEl;
export function initRunningTotal(profile) {
  const bar = document.getElementById('sub-nav');
  if(! statusEl || statusEl.parentNode !== bar) {
    statusEl = document.createElement('div');
    statusEl.classList.add('status');
    bar.insertBefore(statusEl, bar.firstChild);
  }

  statusEl.innerHTML = "Profile score: " + Math.round(profile.addScore());
  return statusEl;
}

export function destructRunningTotal() {
  const bar = document.getElementById('sub-nav');
  bar.removeChild(statusEl);
  statusEl = null;
}
