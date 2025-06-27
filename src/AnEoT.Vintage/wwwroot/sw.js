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
    if (event.tag === 'fetch-aneot-latest-volume' || event.tag === 'fetch-aneot-latest-volume-dbg') {
        const skipChecking = event.tag === 'fetch-aneot-latest-volume-dbg';

        const response = await fetch('/latest-volume.json');
        const data = await response.json();
        const volumeName = data.VolumeName;
        const volumeFolderName = data.VolumeFolderName;

        const storedVolumeFolderName = await get("latest-volume");
        if (storedVolumeFolderName == undefined && !skipChecking) {
            set("latest-volume", volumeFolderName);
            return;
        }
        else if (volumeFolderName === storedVolumeFolderName && !skipChecking) {
            return;
        }

        set("latest-volume", volumeFolderName);

        const title = "新期刊已发布";
        const option = {
            body: volumeName,
            actions: [
                {
                    action: "view-latest-volume",
                    title: "查看"
                }
            ],
            data: {
                url: `/posts/${volumeFolderName}/`
            }
        };

        self.registration.showNotification(title, option);
    }
});

self.addEventListener('notificationclick', event => {
    event.notification.close();

    //if (event.action === 'view-latest-volume') {
    //    event.waitUntil(clients.openWindow(event.notification.data.url))
    //}

    clients.openWindow(event.notification.data.url)
}, false);