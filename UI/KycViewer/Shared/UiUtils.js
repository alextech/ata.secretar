export function openNewTab(url) {
    window.open(url, '_blank');
}

// =================== RICH TEXT AREA BRIDGE ==================

export function getRichTextValue(textAreaName) {
    const textAreaRef = document.querySelector(`textarea[name="${textAreaName}"]`);

    if (textAreaRef == null) {
        return "";
    }
    
    return textAreaRef.value;
}

export function setRichTextValue(textAreaName, value) {
    const textAreaRef = document.querySelector(`textarea[name="${textAreaName}"]`);
    
    textAreaRef.value = value;
}

export function toggleReadOnlyRichTextValue(textAreaName, readOnly) {
    const textAreaRef = document.querySelector(`textarea[name="${textAreaName}"]`);

    textAreaRef.isReadOnly = readOnly;
}

let documentBridgeInstance;
function saveRichTextToDocument (e) {
    const metadata = e.target?.form?.elements?.sectionMetadata?.dataset;

    if (metadata == null) {
        return;
    }

    documentBridgeInstance.invokeMethodAsync(
        "SaveNote",
        e.target.name, e.target.value, metadata.section, metadata.sectionId
    );
}
export function registerDocumentNotesBridge(bridgeInstance) {
    documentBridgeInstance = bridgeInstance;
    document.addEventListener('richTextSaved', saveRichTextToDocument);
}

export function unregisterDocumentNotesBridge() {
    document.removeEventListener('richTextSaved', saveRichTextToDocument);
}

// ============ CHART BRIDGE ==================

let currentChartSelectionId = null;
export function setChartData(data, bridgeInstance) {

    for (let option of data.options) {
        const chart = document.getElementById(option.chartId);
        chart.option = option;
        if (option.isSelected) {
            chart.removeAttribute('disabled');
            chart.setAttribute('selected', '');
            currentChartSelectionId = option.chartId;
        }

        chart.addEventListener('optionChartSelected', (e) => {
            bridgeInstance.invokeMethodAsync(
                'SetRecommendation',
                option.optionId
            );

            if (currentChartSelectionId != null) {
                document.getElementById(currentChartSelectionId).removeAttribute('selected');
            }

            currentChartSelectionId = option.chartId;
        });
    }
}


// ============== QUESTION HIGHLIGHT BRIDGE ==================
export function highlightQuestion(questionId) {
    const questionNode = document.getElementById(questionId);
    questionNode.scrollIntoView(true);
    questionNode.classList.add("activeAnimation");
    setTimeout(() => {
        questionNode.classList.remove("activeAnimation");
    }, 3000)
}

// ============== TIMELINE BRIDGE ===========================
// TODO replace when .net 6 comes out:
//  https://github.com/dotnet/aspnetcore/issues/17552
//  https://github.com/dotnet/aspnetcore/issues/27651
export function registerTimelineBridge(timelineInstance) {
    const timeline = document.getElementsByTagName('ata-timeline')[0];

    timeline.addEventListener('NewItemRequest', (e) => {
        timelineInstance.invokeMethodAsync(
          'NewItemRequestHandler',
            e.detail.id, e.detail.start, e.detail.end
        );


    });


}

export function deleteTimelinePlaceholder(id) {
    const timeline = document.getElementsByTagName('ata-timeline')[0];

    timeline.removeChild(timeline.querySelector(`ata-timeline-item[item-id="${id}"]`));
}

export function registerTimelineItemBridge(timelineItemInstance) {
    const timeline = document.getElementsByTagName('ata-timeline')[0];

    timeline.addEventListener('ItemChanged', (e) => {
        timelineItemInstance.invokeMethodAsync(
            'ItemChangedHandler',
            e.detail.itemId, e.detail.to.start, e.detail.to.end, e.detail.to.iconYear
        );
    });
}

const dataStoreMap = {}
export function registerDataStore(dataStoreInstance, storeType) {
    if (dataStoreMap.hasOwnProperty(storeType)) {
        return;
    }
    dataStoreMap[storeType] = dataStoreInstance;
}

export function getDataStore(storeType) {
    return dataStoreMap[storeType];
}
