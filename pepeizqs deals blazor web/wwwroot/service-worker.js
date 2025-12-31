// v1.0.8

self.addEventListener('push', function (event) {
    const data = event.data ? JSON.parse(event.data.text()) : {};

    const icono = data.icon ? `https://pepe.deals${data.icon}` : 'https://pepe.deals/favicon.ico';

    const options = {
        body: " ",
        icon: icono,
        tag: 'notification', 
        requireInteraction: false,
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