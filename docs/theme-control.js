const darkThemeCSSTagId = 'dark-theme-css';

export default class ThemeControl {

    constructor(darkThemeCssFilePath) {
        this.darkThemeCssFilePath = darkThemeCssFilePath;
    }

    initialize = () => {
        this.isDarkMode = false;    // true: dark mode; false: light mode
        if (window.matchMedia) {
            if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
                this.isDarkMode = true;
            }
        }

        if (this.isDarkMode) {
            this.addDarkModeStyleSheet();
        }

        return this.isDarkMode;
    }

    toggleTheme = () => {
        if (this.isDarkMode) {
            this.removeDarkThemeStyleSheet();
            this.isDarkMode = false;
        }
        else {
            this.addDarkModeStyleSheet();
            this.isDarkMode = true;
        }
        return this.isDarkMode;
    }

    addDarkModeStyleSheet = () => {
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

    removeDarkThemeStyleSheet() {
        const darkThemeCssTag = document.getElementById(darkThemeCSSTagId);
        if (!!darkThemeCssTag) {
            console.log('dark theme css exists');
            darkThemeCssTag.remove();
        }
    }
}