export function cambiarEnlace(url) {
    history.pushState(null, '', url);
}

export function copiarTexto(text) {
    if (navigator.clipboard) {
        navigator.clipboard.writeText(text);
    }
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