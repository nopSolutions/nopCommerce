// Cache static assets
self.addEventListener('install', function (event) {
  event.waitUntil(
    caches.open('your-cache-name').then(function (cache) {
      return cache.addAll([
        '/css/style.css',
        '/js/script.js',
        '/images/logo.png'
        // Add more static assets to cache
      ]);
    })
  );
});

// Serve cached assets when offline
self.addEventListener('fetch', function (event) {
  event.respondWith(
    caches.match(event.request).then(function (response) {
      return response || fetch(event.request);
    })
  );
});