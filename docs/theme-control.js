const darkThemeCSSTagId = 'dark-theme-css';

export default class ThemeControl {

    constructor(darkThemeCssFilePath) {
        this.darkThemeCssFilePath = darkThemeCssFilePath;
    }

    initialize = () => {
        this.isDarkMode = false;    // true: dark mode; false: light mode
        if (window.matchMedia) {
            const darkThemeMq = window.matchMedia('(prefers-color-scheme: dark)');
            this.isDarkMode = darkThemeMq.matches;

            // Toggle theme upon theme change
            darkThemeMq.addEventListener("change", e => {
                this.toggleTheme(e.matches)
            });
        }

        this.#setThemeTo(this.isDarkMode);
        return this.isDarkMode;
    }

    toggleTheme = () => this.#setThemeTo(!this.isDarkMode);  // Flip the result;

    #addDarkModeStyleSheet = () => {
        // Already added.
        const darkThemeCssTag = document.getElementById(darkThemeCSSTagId);

        if (!!darkThemeCssTag) {
            console.log(`dark theme css with id ${darkThemeCSSTagId} already exists`);
            return;
        }

        const head = document.getElementsByTagName('head')[0];

        // Creating link element
        const style = document.createElement('link')
        style.href = this.darkThemeCssFilePath;
        style.type = 'text/css';
        style.rel = 'stylesheet';
        style.id = darkThemeCSSTagId;
        head.append(style);
    };

    #removeDarkThemeStyleSheet() {
        const darkThemeCssTag = document.getElementById(darkThemeCSSTagId);
        if (!!darkThemeCssTag) {
            console.log('dark theme css exists');
            darkThemeCssTag.remove();
        }
    }

    #setThemeTo = (isDarkMode) => {
        if (isDarkMode) {
            this.#addDarkModeStyleSheet();
        }
        else {
            this.#removeDarkThemeStyleSheet();
        }
        this.isDarkMode = isDarkMode;
        return this.isDarkMode;
    }
}