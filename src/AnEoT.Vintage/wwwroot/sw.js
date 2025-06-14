self.addEventListener('periodicsync', event => {
    if (event.tag === 'fetch-aneot-latest-volume') {
        const title = "来自回归线简易版的问候！";
        const option = { body: "尤里卡~尤里卡~" };

        try {
            const notification = new Notification(title, option);
        }
        catch (err) {
            self.registration.showNotification(title, option);
        }
   
    }
});