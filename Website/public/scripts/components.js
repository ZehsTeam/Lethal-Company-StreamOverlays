class CustomOverlay extends HTMLElement {
    connectedCallback() {
        this.classList.add('hidden');
    }

    setVisible(value) {
        if (value === undefined) return;

        this.classList.toggle('hidden', !value);
    }
}

class CustomStat extends HTMLElement {
    connectedCallback() {
        this.classList.add('item');
        this.innerHTML = this.getBaseHTML();
    }

    update(data) {
        if (data.type === 'formatting') {
            this.updateFormatting(data);
        } else if (data.type === 'data') {
            this.updateData(data);
        }
    }

    updateFormatting(data) {
        if (this.labelKey && data[this.labelKey]) {
            this.labelFormatting = data[this.labelKey]; // Dynamically fetch the correct label
        }
        this.updateElement();
    }

    updateData(data) {
        Object.assign(this, data); // Dynamically update properties
        this.updateElement();
    }

    updateElement() {
        const sizeContainer = this.querySelector('.size-container');
        const displayContainer = this.querySelector('.display-container');

        const sizeContent = this.getHTML(...this.getSizeData());
        const displayContent = this.getHTML(...this.getDisplayData());

        if (sizeContainer) sizeContainer.innerHTML = sizeContent;
        if (displayContainer) displayContainer.innerHTML = displayContent;
    }

    getBaseHTML() {
        return `
            <div class="container size-container"></div>
            <div class="container display-container"></div>
        `;
    }

    getSizeData() {
        return []; // Define in subclasses
    }

    getDisplayData() {
        return []; // Define in subclasses
    }

    getHTML() {
        return ''; // Define in subclasses
    }
}

class CustomCrew extends CustomStat {
    connectedCallback() {
        super.connectedCallback();
        this.labelKey = 'crewLabel'; // Key to fetch the label formatting
        this.labelFormatting = 'Crew: {value}';
        this.crewCount = 1;
        this.updateElement();
    }

    getSizeData() {
        return [99]; // Example static data for size-container
    }

    getDisplayData() {
        return [this.crewCount]; // Dynamic data for display-container
    }

    getHTML(crewCount) {
        const text = this.labelFormatting.replace('{value}', crewCount);
        return `<p class="value">${text}</p>`;
    }
}

class CustomMoon extends CustomStat {
    connectedCallback() {
        super.connectedCallback();
        this.classList.add('grow');
        this.labelKey = 'moonLabel'; // Key to fetch the label formatting
        this.labelFormatting = 'Moon: {value}';
        this.moonName = '41 Experimentation';
        this.weatherName = 'None';
        this.showWeatherIcon = true;
        this.updateElement();
    }

    getSizeData() {
        return ['41 Experimentation', 'None'];
    }

    getDisplayData() {
        return [this.moonName, this.weatherName];
    }

    getHTML(moonName, weatherName) {
        const text = this.labelFormatting.replace('{value}', moonName);
        const weatherIconHTML = this.showWeatherIcon
            ? `<p class="icon" style="margin-left: 20px;">${this.getWeatherIconCode(weatherName)}</p>`
            : '';
        return `
            <p class="value">${text}</p>
            ${weatherIconHTML}
        `;
    }

    getWeatherIconCode(weatherName) {
        const weatherIconCodes = {
            none: '&#xe900;',
            dustclouds: '&#xe906;',
            rainy: '&#xe901;',
            stormy: '&#xe903;',
            foggy: '&#xe904;',
            flooded: '&#xe902;',
            eclipsed: '&#xe905;',
        };
        return weatherIconCodes[weatherName?.toLowerCase()] || '';
    }
}

class CustomDay extends CustomStat {
    connectedCallback() {
        super.connectedCallback();
        this.labelKey = 'dayLabel'; // Key to fetch the label formatting
        this.labelFormatting = 'Day: {value} ({value2}/{value3})';
        this.dayCount = 1;
        this.dayInQuota = 1;
        this.daysInQuota = 3;
        this.updateElement();
    }

    getSizeData() {
        return [99, 1, this.daysInQuota];
    }

    getDisplayData() {
        return [this.dayCount, this.dayInQuota, this.daysInQuota];
    }

    getHTML(dayCount, dayInQuota, daysInQuota) {
        const text = this.labelFormatting
            .replace('{value}', dayCount)
            .replace('{value2}', dayInQuota)
            .replace('{value3}', daysInQuota);
        return `<p class="value">${text}</p>`;
    }
}

class CustomQuota extends CustomStat {
    connectedCallback() {
        super.connectedCallback();
        this.labelKey = 'quotaLabel'; // Key to fetch the label formatting
        this.labelFormatting = 'Quota {value2}: ${value}';
        this.quotaValue = 130;
        this.quotaIndex = 1;
        this.updateElement();
    }

    getSizeData() {
        return [99999, 99];
    }

    getDisplayData() {
        return [this.quotaValue, this.quotaIndex];
    }

    getHTML(quotaValue, quotaIndex) {
        const text = this.labelFormatting
            .replace('{value}', quotaValue)
            .replace('{value2}', quotaIndex);
        return `<p class="value">${text}</p>`;
    }
}

class CustomLoot extends CustomStat {
    connectedCallback() {
        super.connectedCallback();
        this.labelKey = 'lootLabel'; // Key to fetch the label formatting
        this.labelFormatting = 'Ship Loot: ${value}';
        this.lootValue = 0;
        this.updateElement();
    }

    getSizeData() {
        return [99999];
    }

    getDisplayData() {
        return [this.lootValue];
    }

    getHTML(lootValue) {
        const text = this.labelFormatting.replace('{value}', lootValue);
        return `<p class="value">${text}</p>`;
    }
}

class CustomAveragePerDay extends CustomStat {
    connectedCallback() {
        super.connectedCallback();
        this.labelKey = 'averagePerDayLabel'; // Key to fetch the label formatting
        this.labelFormatting = 'Avg/Day: ${value}';
        this.averagePerDayValue = 0;
        this.updateElement();
    }

    getSizeData() {
        return [9999];
    }

    getDisplayData() {
        return [this.averagePerDayValue];
    }

    getHTML(averagePerDayValue) {
        const text = this.labelFormatting.replace('{value}', averagePerDayValue);
        return `<p class="value">${text}</p>`;
    }
}

// Register all custom elements
customElements.define('custom-overlay', CustomOverlay);
customElements.define('custom-crew', CustomCrew);
customElements.define('custom-moon', CustomMoon);
customElements.define('custom-day', CustomDay);
customElements.define('custom-quota', CustomQuota);
customElements.define('custom-loot', CustomLoot);
customElements.define('custom-averageperday', CustomAveragePerDay);