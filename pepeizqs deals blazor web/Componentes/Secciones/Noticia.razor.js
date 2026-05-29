export function cambiarEnlace(url) {
    history.pushState(null, '', url);
}

export function copiarTexto(text) {
    if (navigator.clipboard) {
        navigator.clipboard.writeText(text);
    }
}

export function activarTooltip(triggerId, tooltipId) {
    const element = document.getElementById(triggerId);
    if (!element || element._tooltipHandler) return;

    element._tooltipHandler = (e) => {
        const tooltip = document.getElementById(tooltipId);
        if (!tooltip) return;

        if (window.innerWidth / 2 > e.clientX) {
            tooltip.style.top = (e.clientY + 10) + 'px';
            tooltip.style.left = (e.clientX + 20) + 'px';
        } else {
            tooltip.style.top = (e.clientY - 10) + 'px';
            tooltip.style.left = (e.clientX - 20 - tooltip.getBoundingClientRect().width) + 'px';
        }
    };

    element.addEventListener('mousemove', element._tooltipHandler);
}

export function desactivarTooltip(triggerId) {
    const element = document.getElementById(triggerId);
    if (!element?._tooltipHandler) return;

    element.removeEventListener('mousemove', element._tooltipHandler);
    delete element._tooltipHandler;
}