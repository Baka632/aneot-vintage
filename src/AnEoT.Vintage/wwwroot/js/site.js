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

    switchThemeByThemeName(theme);
    docCookies.setItem("pageTheme", theme, Infinity, "/")
}

function autoSwitchTheme() {
    var preDefTheme = docCookies.getItem("pageTheme");
    if (preDefTheme != null && (preDefTheme == "light" || preDefTheme == "dark")) {
        switchThemeByThemeName(preDefTheme);
        return;
    }

    if (window.matchMedia != undefined && window.matchMedia('(prefers-color-scheme: dark)').matches) {
        switchThemeByThemeName("dark");
    }
    else {
        switchThemeByThemeName("light");
    }
}

function switchThemeByThemeName(theme) {
    var link = document.getElementById("theme-css");
    var str = link.href.replace(/(light|dark)\.css/i, theme + ".css");
    link.href = str;

    var supportSvg = !!document.createElementNS && !!document.createElementNS('http://www.w3.org/2000/svg', 'svg').createSVGRect;
    var eodImage = document.getElementById("eod-image-element");

    if (supportSvg && eodImage != null) {
        //var isInGitHubPages = window.location.href.match("https://baka632.github.io/aneot-vintage") != null;

        eodImage.height = 14;
        eodImage.width = 14;
        if (theme == "dark") {
            eodImage.src = "/eod_white.svg";
        }
        else {
            eodImage.src = "/eod_black.svg";
        }
    }
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