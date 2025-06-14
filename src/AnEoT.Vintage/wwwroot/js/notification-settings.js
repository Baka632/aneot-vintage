var enableNotificationButton = document.getElementById("enable-notification-button");
var disableNotificationButton = document.getElementById("disable-notification-button");

determineNotificationButtonVisiblity(enableNotificationButton, disableNotificationButton);

function determineNotificationButtonVisiblity(enableBtn, disableBtn) {
    try {
        determineNotificationButtonVisiblityCore(enableBtn, disableBtn)
            .catch(reason => console.error("Notification error: " + reason))
    } catch (e) {
        // qwq
    }
}

async function determineNotificationButtonVisiblityCore(enableBtn, disableBtn) {
    // 基本 API 检查
    let basicRequirement = "Notification" in window && 'serviceWorker' in navigator;

    if (!basicRequirement) {
        disableBtn.style.display = enableBtn.style.display = "none";
        return;
    }

    let result = (await navigator.serviceWorker.ready).periodicSync ? true : false;

    if (!result) {
        disableBtn.style.display = enableBtn.style.display = "none";
        return;
    }

    enableBtn.addEventListener("click", async function () {
        await requestForNotification(enableBtn, disableBtn);
    });
    disableBtn.addEventListener("click", async function () {
        await unregisterForNotification(enableBtn, disableBtn);
    });

    // 权限检查
    const notifGranted = Notification.permission === "granted";
    const periodicStatus = await navigator.permissions.query({ name: 'periodic-background-sync' });

    if (notifGranted && periodicStatus.state === "granted") {
        const registration = await navigator.serviceWorker.ready;
        const tags = await registration.periodicSync.getTags();

        if (tags.includes("fetch-aneot-latest-volume")) {
            modifyEnableNotificationButtonVisiblity(false, enableBtn, disableBtn);
            return;
        }
    }

    modifyEnableNotificationButtonVisiblity(true, enableBtn, disableBtn);
}

async function unregisterForNotification(enableBtn, disableBtn) {
    const registration = await navigator.serviceWorker.ready;
    registration.periodicSync.unregister("fetch-aneot-latest-volume");
    alert("完成");
    modifyEnableNotificationButtonVisiblity(true, enableBtn, disableBtn);
}

async function requestForNotification(enableBtn, disableBtn) {
    alert("接下来您可能需要允许我们使用一些权限，这样才能启用更新通知推送功能。");

    let notifGranted = Notification.permission === "granted";

    if (!notifGranted) {
        notifGranted = await Notification.requestPermission() === "granted";
        if (!notifGranted) {
            alert("您需要同意通知权限");
            return;
        }
    }

    let periodicSyncGranted = (await navigator.permissions.query({ name: 'periodic-background-sync' })).state === "granted";
    const registration = await navigator.serviceWorker.ready;
    const tags = await registration.periodicSync.getTags();

    if (!periodicSyncGranted || !tags.includes("fetch-aneot-latest-volume")) {
        try {
            await registration.periodicSync.register('fetch-aneot-latest-volume', {
                minInterval: 43200000 // 十二小时
            });

            periodicSyncGranted = (await navigator.permissions.query({ name: 'periodic-background-sync' })).state === "granted";

            if (!periodicSyncGranted) {
                alert("您需要同意定期同步权限");
                return;
            }
        }
        catch (err) {
            alert(`出现问题，未注册定期同步任务，请尝试安装本站点的 PWA 应用：${err}`);
            return;
        }
    }

    alert("成功！");
    modifyEnableNotificationButtonVisiblity(false, enableBtn, disableBtn);
}

function modifyEnableNotificationButtonVisiblity(enableButtonVisible, enableButton, disableButton) {
    if (enableButtonVisible) {
        enableButton.style.display = "block";
        disableButton.style.display = "none";
    }
    else {
        disableButton.style.display = "block";
        enableButton.style.display = "none";
    }
}