const pages = [
    {
        "name": "page1",
        "index": 0,
        "path": ["/", "/index.html", "/404.html"]
    },
    {
        "name": "page2",
        "index": 1,
        "path": ["/videos"]
    },
    {
        "name": "page3",
        "index": 2,
        "path": ["/projects"]
    }
];

const activePageClass = 'main-page-content active';
const hiddenPageClass = 'main-page-content';

document.addEventListener('DOMContentLoaded', () => {
    console.log('doc is ready. Start the script!');

    hideAllPages();

    let page = getPageRoute();
    goToPage(page.name);
});

window.onpopstate = (e => {
    console.log(`Popped history: ${e.state}`);
    if (!!e.state) {
        goToPage(e.state);
    }
});

function hideAllPages() {
    pages.forEach(p => {
        let pageDiv = document.getElementById(p.name);
        if (!!pageDiv) {
            console.log('Hiding page: ' + pageDiv.id);
            pageDiv.setAttribute('class', hiddenPageClass);
        }
    });
}

function navigateTo(route) {
    console.log(`Invoking navigate to ${route}`);
    let pageObj = getPageRoute(route);
    window.history.pushState(pageObj.name, "", route);
    console.log(`Navigating to page: ${pageObj.name}`);
    goToPage(pageObj.name);
    highlightNav(pageObj.index);
}

function goToPage(pageName) {
    console.log(`Go to page by name: ${pageName}`)

    if (!pageName) {
        console.log('Use default page name.');
        pageName = "page1";
    }

    hideAllPages();

    let targetPage = document.getElementById(pageName);
    if (!!targetPage) {
        targetPage.setAttribute("class", activePageClass);
    }
}

function highlightNav(index) {
    let navBarDiv = document.getElementById('nav-bar');
    let ulTag = navBarDiv.getElementsByTagName('ul')[0];
    let liArray = ulTag.getElementsByTagName('li');
    if (!!liArray && !!liArray.length && liArray.length > 0) {
        for (let i = 0; i < liArray.length; i++) {
            if (i === index) {
                liArray[i].setAttribute('class', 'active');
            }
            else {
                liArray[i].setAttribute('class', null);
            }
        }
    }
    else {
        console.error('No navigation tab?');
    }
}

function getPageRoute(path) {
    if (!path) {
        path = window.location.pathname;
    }
    path = path.toLowerCase();
    console.log(`Current path: ${path}`);

    // Route matching
    let page = null;
    pages.forEach(p => {
        p.path.forEach(r => {
            // Already found one.
            if (!!page) {
                return;
            }

            // Special case for root
            if (r === '/' && path === '/') {
                console.log('Routing special: /');
                page = p;
            }

            if (r !== '/' && path.startsWith(r)) {
                console.log(`Hit by routing. Location: ${path}, Route: ${r}`)
                page = p;
            }
        });
    });

    if (!page) {
        page = pages[0];
        console.log(`No route matched. Falls back to default: ${page.name}`);
    }
    // Haven't returned yet?
    console.log(`Decided page name: ${page.name}`);
    return page;
}