const dropDownTemplate = document.createElement('template');
dropDownTemplate.innerHTML = 
`<select>
    
</select>
`;

const FETCH_MODE_API_MODE = 'API_MODE';
const FETCH_MODE_STORE_MODE = 'STORE_MODE';

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
        this.selectNode.addEventListener('focus', this.#fetchDictionary.bind(this));
        this.selectNode.addEventListener('change', this.#bubbleChangeHandler.bind(this));
    }

    async attributeChangedCallback(attribute, oldValue, newValue) {
        if (oldValue === newValue) return;

        switch (attribute) {
            case 'sourceurl':
                this.#fetchMode = FETCH_MODE_API_MODE;

                break;
            case 'storeType':
                this.#fetchMode = FETCH_MODE_STORE_MODE;
                
                break;
            default:
                return;
        }
    }
    
    #items;
    #fetchMode;
    
    async #fetchDictionary() {
        if (this.#fetchMode === FETCH_MODE_API_MODE) {
            const response = await fetch(this.getAttribute('sourceurl'));
            // // TODO 2) add lazy loading attribute true/false - load only on dropdown
            this.#items = await response.json();
            
            if (!this.hasAttribute('collectionkey')) {
                throw new Error(
                    "GlossaryLookup::fetchDictionary. При использовании компонента ata-glossary-lookup через fetch api с collection key обязательно указать collectionkey где находится массив данных."
                );
            }
            
            const collectionKey = this.getAttribute('collectionkey');
            this.#items = this.#items[collectionKey];
        } else {
            this.#items = await window.uiUtils.getDataStore(this.getAttribute('storeType')).invokeMethodAsync('GetAll');
        }

        const optionsFragment = document.createDocumentFragment();

        const valueKey = this.valueKey;
        const displayKey = this.displayKey;
        
        for (const item of this.#items) {

            // <option value="valueKey">displayKey</option>
            const optionNode = document.createElement('option');
            optionNode.value = item[valueKey];
            optionNode.innerHTML = item[displayKey];

            optionsFragment.append(optionNode);
        }

        this.selectNode.replaceChildren(optionsFragment);
    }
    #bubbleChangeHandler(e) {
        let event;
        if (this.#fetchMode === FETCH_MODE_API_MODE) {
            event = new CustomEvent("change", {
                detail: {
                    from: null, // TODO держать от кокого значения перешли
                    to: this.#items[e.target.value]
                }
            });
        } else {
            event = new CustomEvent("customchange", {
                detail: {
                    from: null, // TODO держать от кокого значения перешли
                    to: e.target.value
                }
            });
        }

        this.dispatchEvent(event);
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
    
    set value(value) {
        
    }
    
    get value() {
        return this.#items[this.selectNode.value];
    }
}

if (!customElements.get('ata-glossary-lookup')) {
    customElements.define('ata-glossary-lookup', GlossaryLookup);
}