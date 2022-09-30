export default class Search {
    constructor(searchInputId) {
        console.log('In search ctor.')
        this.searchInputId = searchInputId;

        const searchInputControl = document.getElementById(searchInputId);
        if (!!searchInputControl) {
            searchInputControl.addEventListener('input', (e) => {
                console.log('event:' + e.target.value);
                if (!!e && !!e.target && !!e.target.value) {
                    this.search(e.target.value);
                    return;
                }

                this.clearSearch();
            });
        }
    }

    search = (keyword) => {
        console.log(`Search for keyword: ${keyword}`);
        // Restore
        this.clearSearch();

        // Hide items that has no match
        const targetTags = document.getElementsByClassName('searchable-item');
        if (!!targetTags) {
            const foundTagLength = targetTags.length;
            for (let i = 0; i < foundTagLength; i++) {
                targetTags[i].removeAttribute('style');

                if (targetTags[i].innerText.toLowerCase().indexOf(keyword.toLowerCase()) < 0) {
                    targetTags[i].style.display = "none";
                }
            }
        }

        // Post process sections that doesn't have any item
        const ulTags = document.getElementsByTagName('ul');
        if (!!ulTags && ulTags.length > 0) {
            const ulTagCount = ulTags.length;
            for (let i = 0; i < ulTagCount; i++) {
                const ulTag = ulTags[i];
                if (!ulTag.innerText) {
                    const noItemPlaceHolder = document.createElement('li');
                    noItemPlaceHolder.setAttribute('class', 'no-item-placeholder');
                    noItemPlaceHolder.innerText = "No match.";
                    ulTag.appendChild(noItemPlaceHolder);
                }
            }
        }
    };

    clearSearch = () => {
        console.log('Clear search!');
        const targetTags = document.getElementsByClassName('searchable-item');
        const foundTagLength = targetTags.length;
        for (let i = 0; i < foundTagLength; i++) {
            targetTags[i].removeAttribute('style');
        }

        const noItemPlaceHolders = document.getElementsByClassName('no-item-placeholder');
        if (!!noItemPlaceHolders && noItemPlaceHolders.length > 0) {
            const noItemPlaceHolderLength = noItemPlaceHolders.length;
            for (let i = noItemPlaceHolderLength - 1; i >= 0; i--) {
                noItemPlaceHolders[i].remove();
            }
        }
    }
}