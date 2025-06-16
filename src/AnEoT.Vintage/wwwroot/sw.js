importScripts("js/workbox-v7.3.0/workbox-sw.js");

workbox.setConfig({
    modulePathPrefix: '/js/workbox-v7.3.0/',
});

const { strategies, expiration } = workbox;

workbox.routing.registerRoute(
    new RegExp(".*\.html$"),
    new workbox.strategies.NetworkFirst({
        cacheName: 'basic-pages',
        networkTimeoutSeconds: 10,
    })
);
workbox.routing.registerRoute(
    new RegExp(".*\.js$"),
    new workbox.strategies.NetworkFirst({
        cacheName: 'basic-pages',
        networkTimeoutSeconds: 10,
    })
);
workbox.routing.registerRoute(
    new RegExp(".*\.css$"),
    new workbox.strategies.NetworkFirst({
        cacheName: 'basic-pages',
        networkTimeoutSeconds: 10,
    })
);
workbox.routing.registerRoute(
    new RegExp(".*\.svg$"),
    new workbox.strategies.NetworkFirst({
        cacheName: 'basic-pages',
        networkTimeoutSeconds: 10,
    })
);

workbox.routing.registerRoute(
    new RegExp(".*\.woff2$"),
    new workbox.strategies.CacheFirst({
        cacheName: 'fonts',
        plugins: [
            new workbox.expiration.ExpirationPlugin({
                purgeOnQuotaError: true,
                maxEntries: 70,
            })
        ],
        matchOptions: {
            ignoreVary: true,
        }
    })
);

workbox.routing.registerRoute(
    new RegExp("https:\/\/unpkg\.com\/.*\/style\.css$"),
    new workbox.strategies.StaleWhileRevalidate()
);

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