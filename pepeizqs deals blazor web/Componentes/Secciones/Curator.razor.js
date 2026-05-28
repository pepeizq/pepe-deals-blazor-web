export function cargarVideos() {
	setTimeout(() => {
		const videos = document.querySelectorAll('.video-autoplay');

		if (videos.length === 0) {
			return;
		}

		const observer = new IntersectionObserver((entries) => {
			entries.forEach(entry => {
				const video = entry.target;

				if (entry.isIntersecting) {
					const playPromise = video.play();

					if (playPromise !== undefined) {
						playPromise.catch(error => {
							console.log("Autoplay bloqueado:", error);
						});
					}
				} else {
					video.pause();
				}
			});
		}, {
			threshold: 0.5
		});

		videos.forEach(video => observer.observe(video));
	}, 100);
}