'use strict';

import ClassicEditor from '@ckeditor/ckeditor5-editor-classic/src/classiceditor';
import Essentials from '@ckeditor/ckeditor5-essentials/src/essentials';
import Paragraph from '@ckeditor/ckeditor5-paragraph/src/paragraph';
import Bold from '@ckeditor/ckeditor5-basic-styles/src/bold';
import Italic from '@ckeditor/ckeditor5-basic-styles/src/italic';
import Autosave from '@ckeditor/ckeditor5-autosave/src/autosave';

import AutoformatPlugin from '@ckeditor/ckeditor5-autoformat/src/autoformat';
import HeadingPlugin from '@ckeditor/ckeditor5-heading/src/heading';
// import BlockQuotePlugin from '@ckeditor/ckeditor5-block-quote/src/blockquote';
// import ImagePlugin from '@ckeditor/ckeditor5-image/src/image';
// import ImageCaptionPlugin from '@ckeditor/ckeditor5-image/src/imagecaption';
// import ImageStylePlugin from '@ckeditor/ckeditor5-image/src/imagestyle';
// import ImageToolbarPlugin from '@ckeditor/ckeditor5-image/src/imagetoolbar';
// import ImageUploadPlugin from '@ckeditor/ckeditor5-image/src/imageupload';
// import LinkPlugin from '@ckeditor/ckeditor5-link/src/link';
import ListPlugin from '@ckeditor/ckeditor5-list/src/list';


class RichEditor extends HTMLTextAreaElement {
  constructor() {
    super();
    this.initialized = false;
    this.tmpVal = '';
  }

  connectedCallback() {
    // yes intentional to leave alone scope of autosave
    const that = this;
    this.editor = ClassicEditor
      .create(this, {
        plugins: [ Essentials, Paragraph, Bold, Italic, Autosave, AutoformatPlugin, HeadingPlugin, ListPlugin ],
        toolbar: [ 'heading', '|', 'bold', 'italic', 'bulletedList', 'numberedList', '|', 'undo', 'redo' ],
        autosave: {
          save( editor ) {

            return that.dispatchEvent(new CustomEvent('richTextSaved', {detail: {}, bubbles: true, composed: true}))
          }
        }
      })
      .then(editor => {
        editor.setData(this.tmpVal);
        this.initialized = true;
        delete this.tmpVal;
        this.editor = editor;
        // const editorElement = editor.ui.view.element;
        // for(let className of this.classList) {
        //   editorElement.classList.add(className);
        // }
        //
        // this.className = '';
        // show available toolbar tools
      // console.log(Array.from( editor.ui.componentFactory.names() ));
    })
      .catch(error => {
        console.error( error );
      });
  }

  get value() {
    if(this.initialized) {
      return this.editor.getData();
    } else {
      return this.tmpVal;
    }
  }

  set value(val) {
    if(this.initialized) {
      this.editor.setData(val);
    } else {
      this.tmpVal = val;
    }
  }
}

if(!customElements.get('ata-rich-editor')) {
  customElements.define('ata-rich-editor', RichEditor, {extends: 'textarea'});

}
