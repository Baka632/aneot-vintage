var themeSelect = document.getElementById("themeSelect");
var layoutCheckbox = document.getElementById("layoutCheckbox");

setThemeSelect(themeSelect);
setLayoutCheckbox(layoutCheckbox);

function setThemeSelect(select) {
    var preDefTheme = docCookies.getItem("pageTheme");
    if (preDefTheme != null && (preDefTheme == "light" || preDefTheme == "dark")) {
        selectDefaultThemeSelectOption(select, preDefTheme);
    }
    else {
        selectDefaultThemeSelectOption(select, "null");
    }

    if (select.addEventListener) {
        select.addEventListener("change", function () {
            switchTheme(select.value)
        });
    }
    else if (select.attachEvent) {
        select.attachEvent('onchange', function () {
            switchTheme(select.value)
        });
    }
}

function selectDefaultThemeSelectOption(select, targetValue) {
    for (var i = 0; i < select.options.length; i++) {
        var single = select.options[i];
        if (single.value == targetValue) {
            single.selected = true;
            break;
        }
    }
}

function setLayoutCheckbox(checkbox) {
    var preDefLayout = docCookies.getItem("pageLayout");
    if (preDefLayout == "center") {
        checkbox.checked = true;
    }

    if (checkbox.addEventListener) {
        // 侦听 change 而不是 input 的原因是：
        // IE 和老版本 Edge 存在 input 控件状态改变时不会激发 input 事件的 bug
        checkbox.addEventListener("change", function () {
            setLayoutByCheckboxValue(checkbox.checked);
        });
    }
    else if (checkbox.attachEvent) {
        if ('onpropertychange' in checkbox) {
            checkbox.attachEvent('onpropertychange', function () {
                setLayoutByCheckboxValue(checkbox.checked);
            });
        }
        else {
            checkbox.attachEvent('onchange', function () {
                setLayoutByCheckboxValue(checkbox.checked);
            });
        }
    }
}

function setLayoutByCheckboxValue(isChecked) {
    if (isChecked) {
        switchLayout("center")
    }
    else {
        switchLayout("left")
    }
}