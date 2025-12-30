// v1.0.2

self.addEventListener('fetch', () => { });

self.addEventListener('push', function (event) {
    console.log('========== PUSH RECIBIDO ==========');
    console.log('Event:', event);
    console.log('Event data:', event.data);

    const data = event.data ? JSON.parse(event.data.text()) : {};

    const options = {
        body: data.body || 'New Notification',
        icon: '/logos/logo6.png',
        badge: '/logos/logo6.png',
        tag: 'notification', 
        data: {
            url: data.url || '/' 
        }
    };

    event.waitUntil(
        self.registration.showNotification(data.title || 'pepe deals', options)
    );
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();

    const urlToOpen = event.notification.data.url || '/';

    event.waitUntil(
        clients.matchAll({ type: 'window' }).then(function (clientList) {
            for (let i = 0; i < clientList.length; i++) {
                const client = clientList[i];
                if (client.url === urlToOpen && 'focus' in client) {
                    return client.focus();
                }
            }

            return clients.openWindow(urlToOpen);
        })
    );
});