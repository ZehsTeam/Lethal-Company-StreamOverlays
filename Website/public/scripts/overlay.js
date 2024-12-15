
function getGapValue() {
    const overlay = document.querySelector('custom-overlay');
    const computedStyle = getComputedStyle(overlay);
    const gapValue = parseFloat(computedStyle.gap); // Retrieve and parse the gap value
    return gapValue;
}

function recalculateItemsMinSize() {
    const overlayItems = document.querySelectorAll('custom-overlay .item');

    overlayItems.forEach(item => {
        const valueElement = item.querySelector('.value');
        const iconElement = item.querySelector('.icon');

        const minsizeText = valueElement?.getAttribute('data-minsize');

        // Create a temporary element to measure text width
        const tempElement = document.createElement('span');
        tempElement.style.position = 'absolute';
        tempElement.style.visibility = 'hidden';
        tempElement.style.whiteSpace = 'nowrap';
        tempElement.style.font = getComputedStyle(valueElement).font;
        tempElement.innerText = minsizeText;

        document.body.appendChild(tempElement);
        const textWidth = tempElement.offsetWidth;
        document.body.removeChild(tempElement);

        // Measure icon width if it exists
        let iconWidth = 0;
        
        if (iconElement) {
            const iconTemp = document.createElement('span');
            iconTemp.style.position = 'absolute';
            iconTemp.style.visibility = 'hidden';
            iconTemp.style.whiteSpace = 'nowrap';
            iconTemp.style.font = getComputedStyle(iconElement).font;
            iconTemp.innerText = iconElement.innerText;

            document.body.appendChild(iconTemp);
            iconWidth = iconTemp.offsetWidth;
            document.body.removeChild(iconTemp);
        }

        let gapWidth = 0;

        if (item.classList.contains('grow')) {
            gapWidth = getGapValue();
        }
        
        // Add the dynamic gap width
        const totalMinWidth = textWidth + iconWidth + gapWidth;
        item.style.minWidth = `${totalMinWidth}px`;
    });
}

document.addEventListener('DOMContentLoaded', recalculateItemsMinSize);
