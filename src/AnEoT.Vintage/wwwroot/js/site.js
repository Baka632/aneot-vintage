if (window.addEventListener) {
    window.addEventListener("DOMContentLoaded", onPageLoad)
}
else if (window.attachEvent) {
    window.attachEvent('onload', onPageLoad);
}

function onPageLoad() {
    autoSwitchTheme();
    autoSwitchLayout();
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

    if (window.matchMedia != undefined && window.matchMedia('(prefers-color-scheme: dark)').matches) {
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

function switchLayout(layout) {
    if (layout == "") {
        return;
    }
    else if (layout == null || layout == "null") {
        docCookies.setItem("pageLayout", "", 0, "/")
        autoSwitchLayout();
        return;
    }
    else if (layout != "left" && layout != "center") {
        autoSwitchLayout();
        return;
    }

    modifyLayoutCssLink(layout);
    docCookies.setItem("pageLayout", layout, Infinity, "/")
}

function autoSwitchLayout() {
    var preDefLayout = docCookies.getItem("pageLayout");
    if (preDefLayout != null && (preDefLayout == "left" || preDefLayout == "center")) {
        modifyLayoutCssLink(preDefLayout);
    }
    else {
        modifyLayoutCssLink("left");
    }
}

function modifyLayoutCssLink(layout) {
    var link = document.getElementById("layout-css");
    var str = link.href.replace(/(left|center)\.css/i, layout + ".css");
    link.href = str;
}