if (window.addEventListener) {
    window.addEventListener("DOMContentLoaded", onPageLoad)
}
else if (window.attachEvent) {
    window.attachEvent('onload', onPageLoad);
}

function onPageLoad() {
    autoSwitchTheme();
}

function switchTheme(theme) {

    if (theme == "") {
        return;
    }
    else if (theme == null || theme == "null") {
        docCookies.setItem("pageTheme", "", 0, "/")
        autoSwitchTheme();
        return;
    }
    else if (theme != "light" && theme != "dark") {
        autoSwitchTheme();
        return;
    }

    modifyThemeCssLink(theme);
    docCookies.setItem("pageTheme", theme, Infinity, "/")
}

function autoSwitchTheme() {
    var preDefTheme = docCookies.getItem("pageTheme");
    if (preDefTheme != null && (preDefTheme == "light" || preDefTheme == "dark")) {
        modifyThemeCssLink(preDefTheme);
        return;
    }

    if (window.matchMedia == undefined) {
        return;
    }
    else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
        modifyThemeCssLink("dark");
    }
    else {
        modifyThemeCssLink("light");
    }
}

function modifyThemeCssLink(theme) {
    var link = document.getElementById("theme-css");
    var str = link.href.replace(/(light|dark)\.css/i, theme + ".css");
    link.href = str;
}