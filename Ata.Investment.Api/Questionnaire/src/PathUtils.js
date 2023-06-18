'use strict';

export function createPathFromEvent(e) {
  const eventStack = e.composedPath().reverse();

  let path = [],
    parent = eventStack.pop();


  while(parent) {
    if(parent.nodeName.toLowerCase() === 'body') {
      break;
    }

    const nodeName = parent.nodeName.toLowerCase();
    if(nodeName === '#document-fragment') {
      path.push('/');
    } else if(nodeName === 'slot') {
      while (parent.shadowRoot === null) {
        parent = eventStack.pop();
      }
    } else {
      let c, e, nthChild;
      for (c=1,e=parent;e.previousElementSibling;e=e.previousElementSibling,c++);
      nthChild = parent.tagName + ":nth-child(" + c + ")";

      if(nodeName === 'svg') {
        path = [];
      }
      path.push(nthChild + (path[path.length-1] === '/' ? '' : ' >'));
    }

    parent = eventStack.pop();
  }
  path = path.reverse();
  path = path.join(' ');

  if(path.slice(-2) === ' >') {
    path = path.slice(0, -2);
  }

  return path.trim();
}

export function getElementFromPath(path) {
  const pathStack = path.split('/').reverse();
  let subPath = pathStack.pop(),
    element,
    root = document;
  do {
    root = root.querySelector(subPath);
    element = root;
    subPath = pathStack.pop();
    root = root.shadowRoot;
  }while(subPath);

  return element;
}
