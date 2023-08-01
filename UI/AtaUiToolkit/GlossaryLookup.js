const dropDownTemplate = document.createElement('template');
dropDownTemplate.innerHTML = 
`<select>
    
</select>
`;

export default class GlossaryLookup extends HTMLElement {
    
    static get observedAttributes() {
        return ['sourceurl'];
    }
    
    constructor() {
        super();
        
        this.attachShadow({mode: 'open'});
        this.shadowRoot.appendChild(dropDownTemplate.content.cloneNode(true));
        this.selectNode = this.shadowRoot.querySelector('select');
    }
    
    connectedCallback() {
        
    }

    async attributeChangedCallback(attribute, oldValue, newValue) {
        if (oldValue === newValue) return;

        switch (attribute) {
            case 'sourceurl':
                this.selectNode.innerHTML = '';
                // const response = await fetch(newValue)
                // // TODO 1) move to connectedCallback
                // // TODO 2) add lazy loading attribute true/false - load only on dropdown
                // const items = await response.json();
                
                const items = await window.uiUtils.getDataStore().invokeMethodAsync('GetAll');
                
                const optionsFragment = document.createDocumentFragment();
                
                const valueKey = this.valueKey;
                const displayKey = this.displayKey;
                for (const item of items[this.getAttribute('collectionkey')]) {
                    
                    // <option value="valueKey">displayKey</option>
                    const optionNode = document.createElement('option');
                    optionNode.value = item[valueKey];
                    optionNode.innerHTML = item[displayKey];
                    
                    optionsFragment.append(optionNode);
                }

                this.selectNode.replaceChildren(optionsFragment);

                break;
            default:
                return;
        }
    }
    
    set sourceUrl(sourceUrl) {
        this.setAttribute('sourceUrl', sourceUrl);
    }
    
    get sourceUrl() {
        return this.getAttribute('sourceUrl');
    }

    set valueKey(value) {
        this.setAttribute('valueKey', value);
    }
    
    get valueKey() {
        if (this.hasAttribute('valueKey')) {
            return this.getAttribute('valueKey');
        } else {
            return 'id';
        }
    }
    
    set displayKey(value) {
        this.setAttribute('displayKey', value)
    }
    
    get displayKey() {
        if (this.hasAttribute('displayKey')) {
            return this.getAttribute('displayKey');
        } else {
            return 'displayName';
        }
    }
}

if (!customElements.get('ata-glossary-lookup')) {
    customElements.define('ata-glossary-lookup', GlossaryLookup);
}