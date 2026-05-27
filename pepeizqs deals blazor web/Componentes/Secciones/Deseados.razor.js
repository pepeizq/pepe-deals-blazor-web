export async function downloadFileFromStream(fileName, contentStreamReference) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

export function inicializarLlamadasGlobales() {
    window.enseñarTooltip2 = enseñarTooltip;
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