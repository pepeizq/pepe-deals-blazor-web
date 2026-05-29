export function copiarTexto(text) {
    if (navigator.clipboard) {
        navigator.clipboard.writeText(text);
    }
}

export function teclearBusqueda(elemento, dotNetRef, retraso) {
    let timerId = null;

    if (!elemento) return;

    elemento.addEventListener("input", function (e) {
        const valor = e.target.value;
        clearTimeout(timerId);

        timerId = setTimeout(() => {
            dotNetRef.invokeMethodAsync("EventoTecleoBuscador", valor);
        }, retraso);
    });
}

export function toggleBodyScroll(disable) {
    if (disable) {
        document.body.classList.add('overflow-hidden');
    } else {
        document.body.classList.remove('overflow-hidden');
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

export function mostrarTooltipBuscador(id) {
    document.getElementById(id).style.display = 'block';
}

export function ocultarTooltipBuscador(id) {
    document.getElementById(id).style.display = 'none';
}

export function actualizarPosicionTooltipBuscador(clientX, clientY, id) {
    var tooltip = document.getElementById(id);
    if (!tooltip) return;

    var tooltipRect = tooltip.getBoundingClientRect();
    var offsetX = 15;
    var offsetY = 15;

    var left, top;

    if (clientX > window.innerWidth / 2) {
        left = clientX - tooltipRect.width - offsetX;
    } else {
        left = clientX + offsetX;
    }

    top = clientY + offsetY;

    if (left < 0) left = offsetX;
    if (left + tooltipRect.width > window.innerWidth) {
        left = window.innerWidth - tooltipRect.width - offsetX;
    }
    if (top + tooltipRect.height > window.innerHeight) {
        top = clientY - tooltipRect.height - offsetY;
    }

    tooltip.style.left = left + 'px';
    tooltip.style.top = top + 'px';
}