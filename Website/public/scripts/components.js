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
        // Common initialization can go here if needed
    }

    updateDataMinSize(valueSelector, getValueCallback) {
        const valueElement = this.querySelector(valueSelector);
        if (valueElement) {
            valueElement.setAttribute('data-minsize', getValueCallback());
            // Trigger min-width recalculation
            calculateItemsMinSize();
        }
    }

    updateTextContent(valueSelector, text) {
        const valueElement = this.querySelector(valueSelector);
        if (valueElement) {
            valueElement.textContent = text;
        }
    }

    dispatchUpdateEvent(data) {
        this.dispatchEvent(new CustomEvent('update', { detail: data }));
    }
}

class CustomCrew extends CustomStat {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <p class="value" data-minsize="Crew: 999">Crew: 1</p>
        `;

        this.labelFormatting = 'Crew: {value}';
        this.crewCount = 1;
    }

    update(data) {
        if (data.type === 'formatting') {
            this.updateFormatting(data);
        } else if (data.type === 'data') {
            this.updateData(data);
        }
    }

    updateFormatting(data) {
        this.labelFormatting = data.crewLabel;
        this.updateDataMinSize('.value', () => this.getText(999));
        this.updateTextContent('.value', this.getText(this.crewCount));
    }

    updateData(data) {
        if (typeof data.crewCount !== 'undefined') {
            this.crewCount = data.crewCount;
        }
        this.updateTextContent('.value', this.getText(this.crewCount));
    }

    getText(crewCount) {
        return this.labelFormatting.replace('{value}', crewCount);
    }
}

class CustomMoon extends CustomStat {
    connectedCallback() {
        this.classList.add('item', 'grow');

        this.innerHTML = `
            <p class="value" data-minsize="Moon: 41 Experimentation">Moon: 41 Experimentation</p>
            <p class="icon" style="margin-left: 20px;">&#xe900;</p>
        `;

        this.labelFormatting = 'Moon: {value} {icon}';
        this.moonName = '41 Experimentation';
        this.weatherName = 'None';
        this.showWeatherIcon = true;
    }

    update(data) {
        if (data.type === 'formatting') {
            this.updateFormatting(data);
        } else if (data.type === 'data') {
            this.updateData(data);
        }
    }

    updateFormatting(data) {
        this.labelFormatting = data.moonLabel;
        this.updateDataMinSize('.value', () => this.getText('41 Experimentation'));
        this.updateText();
    }

    updateData(data) {
        if (typeof data.moonName !== 'undefined') {
            this.moonName = data.moonName;
        }
        if (typeof data.weatherName !== 'undefined') {
            this.weatherName = data.weatherName;
        }
        if (typeof data.showWeatherIcon !== 'undefined') {
            this.showWeatherIcon = data.showWeatherIcon;
        }
        this.updateText();
    }

    updateText() {
        this.updateTextContent('.value', this.getText(this.moonName));
        this.updateIcon();
    }

    getText(moonName) {
        return this.labelFormatting.replace('{value}', moonName);
    }

    updateIcon() {
        const iconElement = this.querySelector('.icon');

        if (iconElement) {
            iconElement.innerHTML = this.getWeatherIconCode(this.weatherName);
            iconElement.classList.toggle('collapse', !this.showWeatherIcon);
        }
    }

    getWeatherIconCode(weatherName) {
        const weatherIconCodes = {
            none: "&#xe900;",
            dustclouds: "&#xe906;",
            rainy: "&#xe901;",
            stormy: "&#xe903;",
            foggy: "&#xe904;",
            flooded: "&#xe902;",
            eclipsed: "&#xe905;"
        };
        return weatherIconCodes[weatherName?.toLowerCase()] || "";
    }
}

class CustomDay extends CustomStat {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <p class="value" data-minsize="Day: 999 (1/3)">Day: 1 (1/3)</p>
        `;

        this.labelFormatting = 'Day: {value} ({value2}/{value3})';
        this.dayCount = 1;
        this.dayInQuota = 1;
        this.daysInQuota = 3;
    }

    update(data) {
        if (data.type === 'formatting') {
            this.updateFormatting(data);
        } else if (data.type === 'data') {
            this.updateData(data);
        }
    }

    updateFormatting(data) {
        this.labelFormatting = data.dayLabel;
        this.updateDataMinSize('.value', () => this.getText(999, 1, 3));
        this.updateText();
    }

    updateData(data) {
        if (typeof data.dayCount !== 'undefined') {
            this.dayCount = data.dayCount;
        }
        if (typeof data.dayInQuota !== 'undefined') {
            this.dayInQuota = data.dayInQuota;
        }
        this.updateText();
    }

    updateText() {
        this.updateTextContent('.value', this.getText(this.dayCount, this.dayInQuota, this.daysInQuota));
    }

    getText(dayCount, dayInQuota, daysInQuota) {
        return this.labelFormatting
            .replace('{value}', dayCount)
            .replace('{value2}', dayInQuota)
            .replace('{value3}', daysInQuota);
    }
}

class CustomQuota extends CustomStat {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <p class="value" data-minsize="Quota 99: $999999">Quota 1: $130</p>
        `;

        this.labelFormatting = 'Quota {value2}: ${value}';
        this.quotaValue = 130;
        this.quotaIndex = 1;
    }

    update(data) {
        if (data.type === 'formatting') {
            this.updateFormatting(data);
        } else if (data.type === 'data') {
            this.updateData(data);
        }
    }

    updateFormatting(data) {
        this.labelFormatting = data.quotaLabel;
        this.updateDataMinSize('.value', () => this.getText(999999, 99));
        this.updateText();
    }

    updateData(data) {
        if (typeof data.quotaValue !== 'undefined') {
            this.quotaValue = data.quotaValue;
        }
        if (typeof data.quotaIndex !== 'undefined') {
            this.quotaIndex = data.quotaIndex;
        }
        this.updateText();
    }

    updateText() {
        this.updateTextContent('.value', this.getText(this.quotaValue, this.quotaIndex));
    }

    getText(quotaValue, quotaIndex) {
        return this.labelFormatting
            .replace('{value}', quotaValue)
            .replace('{value2}', quotaIndex);
    }
}

class CustomLoot extends CustomStat {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <p class="value" data-minsize="Ship Loot: $999999">Ship Loot: $0</p>
        `;

        this.labelFormatting = 'Ship Loot: ${value}';
        this.lootValue = 0;
    }

    update(data) {
        if (data.type === 'formatting') {
            this.updateFormatting(data);
        } else if (data.type === 'data') {
            this.updateData(data);
        }
    }

    updateFormatting(data) {
        this.labelFormatting = data.lootLabel;
        this.updateDataMinSize('.value', () => this.getText(999999));
        this.updateText();
    }

    updateData(data) {
        if (typeof data.lootValue !== 'undefined') {
            this.lootValue = data.lootValue;
        }
        this.updateText();
    }

    updateText() {
        this.updateTextContent('.value', this.getText(this.lootValue));
    }

    getText(lootValue) {
        return this.labelFormatting.replace('{value}', lootValue);
    }
}

class CustomAveragePerDay extends CustomStat {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <p class="value" data-minsize="Avg/Day: $9999">Avg/Day: $0</p>
        `;

        this.labelFormatting = 'Avg/Day: ${value}';
        this.averagePerDayValue = 0;
    }

    update(data) {
        if (data.type === 'formatting') {
            this.updateFormatting(data);
        } else if (data.type === 'data') {
            this.updateData(data);
        }
    }

    updateFormatting(data) {
        this.labelFormatting = data.averagePerDayLabel;
        this.updateDataMinSize('.value', () => this.getText(9999));
        this.updateText();
    }

    updateData(data) {
        if (typeof data.averagePerDayValue !== 'undefined') {
            this.averagePerDayValue = data.averagePerDayValue;
        }
        this.updateText();
    }

    updateText() {
        this.updateTextContent('.value', this.getText(this.averagePerDayValue));
    }

    getText(averagePerDayValue) {
        return this.labelFormatting.replace('{value}', averagePerDayValue);
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