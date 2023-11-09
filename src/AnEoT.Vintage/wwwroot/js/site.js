window.addEventListener("DOMContentLoaded", (event) => { onPageLoad(); })

function onPageLoad() {
    autoSwitchTheme();
}

function switchTheme(theme) {
    if (theme == null) {
        docCookies.removeItem("pageTheme")
        autoSwitchTheme();
        return;
    }
    else if (theme != "light" && theme != "dark") {
        autoSwitchTheme();
        return;
    }

    modifyThemeCssLink(theme);
    docCookies.setItem("pageTheme", theme, Infinity)
}

function autoSwitchTheme() {
    let preDefTheme = docCookies.getItem("pageTheme");
    if (preDefTheme != null && (preDefTheme == "light" || preDefTheme == "dark")) {
        modifyThemeCssLink(preDefTheme);
        return;
    }

    if (window.matchMedia("(prefers-color-scheme)").media === "not all") {
        console.log("This browser doesn\'t support 'prefers-color-scheme' media query");
        return;
    }
    else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
        modifyThemeCssLink("dark");
    }
    else if (window.matchMedia('(prefers-color-scheme: light)').matches) {
        modifyThemeCssLink("light");
    }
}

function modifyThemeCssLink(theme) {
    let link = document.getElementById("theme-css");
    let str = link.href.replace(/(light|dark)\.css/i, theme + ".css");
    link.href = str;
}