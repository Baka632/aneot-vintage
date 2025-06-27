importScripts("/js/lib/workbox-v7.3.0/workbox-sw.js");
importScripts("/js/lib/idb-keyval/umd.js");

const { get, set } = idbKeyval;

workbox.setConfig({
    modulePathPrefix: '/js/lib/workbox-v7.3.0/',
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

self.addEventListener('periodicsync', async event => {
    if (event.tag === 'fetch-aneot-latest-volume') {
        const response = await fetch('/latest-volume.json');
        const data = await response.json();
        const volumeName = data.VolumeName;
        const volumeFolderName = data.VolumeFolderName;

        const storedVolumeFolderName = await get("latest-volume");
        if (storedVolumeFolderName == undefined) {
            set("latest-volume", volumeFolderName);
            return;
        }
        else if (volumeFolderName === storedVolumeFolderName) {
            return;
        }

        set("latest-volume", volumeFolderName);

        const title = "新期刊已发布";
        const option = { body: volumeName };

        try {
            const notification = new Notification(title, option);
        }
        catch (err) {
            self.registration.showNotification(title, option);
        }
   
    }
});