// import idb from 'idb';
// import workbox from 'workbox-sw';
// import workbox from 'workbox-sw';
// import 'workbox-strategies';
// import 'workbox-precaching';
// import workbox from 'workbox-sw/build/workbox-sw';
// const DBVERSION = 1;

importScripts('https://storage.googleapis.com/workbox-cdn/releases/3.4.1/workbox-sw.js');
// const retrieveClassTemplates = require('../database/wireindexdb').retrieveClassTemplates;
// const workbox = new WorkboxSW();

if (workbox) {
    workbox.skipWaiting();
    workbox.clientsClaim();
    // workbox.setConfig({
    //   debug: true
    //   // modulePathCb: (moduleName) => {
    //   //   throw new Error(`Attempt was made to load module '${moduleName}', use loadModule() instead.`);
    //   // }
    // });

    // workbox.loadModule('workbox-core');
    // workbox.loadModule('workbox-precaching');
    // workbox.loadModule('workbox-strategies');
    console.log(`Yay! Workbox is loaded 🎉`);

    workbox.precaching.precacheAndRoute(self.__precacheManifest);

} else {
    console.log(`Boo! Workbox didn't load 😬`);
}

// self.addEventListener('activate', function (event) {
//   event.waitUntil(
//     // () => console.log('activating service worker')
//   );
// });

// The fetch handler serves responses for same-origin resources from a cache.
// If no response is found, it populates the runtime cache with the response
// from the network before returning it to the page.
//8.15.18 not sure if this needs to be removed
// self.addEventListener('fetch', event => {
//   // Skip cross-origin requests, like those for Google Analytics.
//   if (event.request.url.startsWith(self.location.origin)) {
//     event.respondWith(
//       caches.match(event.request).then(cachedResponse => {
//         if (cachedResponse) {
//           return cachedResponse;
//         }
//       })
//     );
//   }
// });

if (workbox) {
    let baseURL = '';

    // if (process.env.REACT_APP_API_URL && process.env.REACT_APP_API_URL !== '') {
    if (true) {
        //use following for localhost development
        // baseURL = `http://localhost:12758/api/`;
        //use this for test server deployment
        baseURL = `https://bullsbluffcore.azurewebsites.net/api/`;
    } else if (window.coreApp) {
        baseURL = window.coreApp.showsApi.baseUrl;
    }
    // workbox.routing.registerRoute(/(.*)article(.*)\.html/, args => {
    //   return articleHandler.handle(args).then(response => {
    //     if (!response) {
    //       return caches.match('pages/offline.html');
    //     } else if (response.status === 404) {
    //       return caches.match('pages/404.html');
    //     }
    //     return response;
    //   });
    // });

    workbox.routing.registerRoute(
        new RegExp('https://fonts.(?:googleapis|gstatic).com/(.*)'),
        workbox.strategies.cacheFirst({
            cacheName: 'googleapis',
            plugins: [
                new workbox.expiration.Plugin({
                    maxEntries: 30
                })
            ]
        })
    )
    workbox.routing.registerRoute(new RegExp(`${baseURL}breeds`),
        workbox.strategies.cacheFirst({
            cacheName: 'showassist-references',
            cacheExpiration: {
                maxEntries: 50
            },
            cacheableResponse: {
                statuses: [0, 200]
            }
        })
    );

    const showNotification = () => {
        self.registration.showNotification('Background sync success!', {
            body: '🎉`🎉`🎉`'
        });
    };
    ///see here for api
    //https://developers.google.com/web/tools/workbox/reference-docs/latest/workbox.backgroundSync.Queue
    const bgSyncPlugin = new workbox.backgroundSync.Plugin('showassist-savedata-queue', {
        maxRetentionTime: 96 * 60, // Retry for max of 24 Hours
        callbacks: {
            queueDidReplay: (evt) => {
                // console.log(evt);
                if (evt && evt.length > 0 && !evt[0].error) {
                    showNotification("Offline data sent to the server");
                }
            }, //show notification after successful replay
            // requestWillEnqueue: showNotification, //() => showNotification("Server unavailable, storing data to send later.")
            // other types of callbacks could go here
        }
    });
    workbox.routing.registerRoute(
        new RegExp(`${baseURL}shows`),
        workbox.strategies.networkOnly({
            plugins: [bgSyncPlugin]
        }),
        'POST'
    );
    workbox.routing.registerRoute(
        new RegExp(`${baseURL}shows`),
        workbox.strategies.networkOnly({
            plugins: [bgSyncPlugin]
        }),
        'PUT'
    );

    // self.addEventListener('fetch', (event) => {
    //   // Clone the request to ensure it's save to read when
    //   // adding to the Queue.
    //   const promiseChain = fetch(event.request.clone())
    //     .catch((err) => {
    //       return queue.addRequest(event.request);
    //     });

    //   event.waitUntil(promiseChain);
    // });
}