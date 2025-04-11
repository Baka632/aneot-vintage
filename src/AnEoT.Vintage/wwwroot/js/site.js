var supportSvg = !!document.createElementNS && !!document.createElementNS('http://www.w3.org/2000/svg', 'svg').createSVGRect;
var httpsTestImage = null;

if (window.addEventListener) {
    window.addEventListener("DOMContentLoaded", onPageLoad)
}
else if (window.attachEvent) {
    window.attachEvent('onload', onPageLoad);
}

function onPageLoad() {
    autoSwitchTheme();
    autoSwitchLayout();
    autoDetectHttpsSupport();
}

function autoDetectHttpsSupport() {
    if (location.href.indexOf('https://') == -1) {
        var ImgObj = new Image();
        httpsTestImage = ImgObj;

        if (ImgObj.addEventListener) {
            ImgObj.addEventListener("load", onHttpsTestImageLoad)
        }
        else if (ImgObj.attachEvent) {
            ImgObj.attachEvent('onload', onHttpsTestImageLoad);
        }
        else {
            // 回家吧，孩子！
            return;
        }

        ImgObj.src = "https://" + location.host + "/images/https-test.gif";
    }
}

function onHttpsTestImageLoad() {
    if (httpsTestImage.complete == "true" || (httpsTestImage.width > 0 && httpsTestImage.height > 0)) {
        if (confirm("本网站支持安全的 HTTPS 连接，我们更推荐您使用此方式。要立刻使用安全的 HTTPS 连接吗？")) {
            location.href = location.href.replace('http://', 'https://');
        }
    }
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

    var eodImage = document.getElementById("eod-image-element");
    var navbarLogo = document.getElementById("aneot-vintage-navbar-logo");

    if (supportSvg) {
        if (eodImage != null) {
            eodImage.height = 14;
            eodImage.width = 14;
            if (theme == "dark") {
                eodImage.src = "/eod_white.svg";
            }
            else {
                eodImage.src = "/eod_black.svg";
            }
        }

        if (navbarLogo != null) {
            if (theme == "dark") {
                navbarLogo.src = "/images/logo_white.svg";
            }
            else {
                navbarLogo.src = "/images/logo_grey.svg";
            }
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