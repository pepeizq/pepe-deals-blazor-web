export function moverScroll(id) {
	const yOffset = -90;
	const element = document.getElementById(id);
	const y = element.getBoundingClientRect().top + window.pageYOffset + yOffset;

	window.scrollTo({ top: y, behavior: 'smooth' });
}

export function enseñarTooltip(e, id) {
    var x = e.clientX,
        y = e.clientY;

    var tooltip = document.getElementById(id);
    if (!tooltip) return;

    if (screen.width / 2 > x) {
        tooltip.style.top = (y + 10) + 'px';
        tooltip.style.left = (x + 20) + 'px';
    }
    else {
        tooltip.style.top = (y - 10) + 'px';
        tooltip.style.left = (x - 20 - tooltip.getBoundingClientRect().width) + 'px';
    }
}