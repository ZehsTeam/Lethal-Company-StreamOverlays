class CustomOverlay extends HTMLElement {
    connectedCallback() {
        this.classList.add('hidden');
    }
}

class CustomCrew extends HTMLElement {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <div class="item">
                <p class="value" data-minsize="Crew: 999">Crew: 1</p>
            </div>
        `;
    }

    setValue(value) {
        if (value === undefined) return;

        const valueElement = this.querySelector('.value');

        if (valueElement) {
            valueElement.textContent = `Crew: ${value}`;
        }
    }
}

class CustomMoon extends HTMLElement {
    connectedCallback() {
        this.classList.add('item');
        this.classList.add('grow');

        this.innerHTML = `
            <p class="value" data-minsize="Moon: 41 Experimentation">Moon: 41 Experimentation</p>
            <p class="icon" style="margin-left: 20px;">&#xe900;</p>
        `;
    }

    setValue(value) {
        if (value === undefined) return;

        const valueElement = this.querySelector('.value');

        if (valueElement) {
            valueElement.textContent = `Moon: ${value}`;
        }
    }

    setWeatherIcon(value) {
        if (value === undefined) return;

        const iconElement = this.querySelector('.icon');

        if (iconElement) {
            iconElement.innerHTML = value;
        }
    }

    setWeatherIconVisibility(value) {
        if (value === undefined) return;

        const iconElement = this.querySelector('.icon');

        if (iconElement) {
            if (value) {
                iconElement.classList.remove('collapse');
            } else {
                iconElement.classList.add('collapse');
            }
        }
    }
}

class CustomDay extends HTMLElement {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <p class="value" data-minsize="Day: 999 (1/3)">Day: 1 (1/3)</p>
        `;
    }

    setValue(day, dayInQuota) {
        if (day === undefined) return;
        if (dayInQuota === undefined) return;

        const valueElement = this.querySelector('.value');

        if (valueElement) {
            valueElement.textContent = `Day: ${day} (${dayInQuota}/3)`;
        }
    }
}

class CustomQuota extends HTMLElement {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <p class="value" data-minsize="Quota 1: $999999">Quota 1: $130</p>
        `;
    }

    setValue(quotaIndex, quota) {
        if (quotaIndex === undefined) return;
        if (quota === undefined) return;

        const valueElement = this.querySelector('.value');

        if (valueElement) {
            valueElement.textContent = `Quota ${quotaIndex}: $${quota}`;
        }
    }
}

class CustomLoot extends HTMLElement {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <p class="value" data-minsize="Ship Loot: $999999">Ship Loot: $0</p>
        `;
    }

    setValue(value) {
        if (value === undefined) return;

        const valueElement = this.querySelector('.value');

        if (valueElement) {
            valueElement.textContent = `Ship Loot: $${value}`;
        }
    }
}

class CustomAveragePerDay extends HTMLElement {
    connectedCallback() {
        this.classList.add('item');

        this.innerHTML = `
            <p class="value" data-minsize="AVG/Day: $9999">AVG/Day: $0</p>
        `;
    }

    setValue(value) {
        if (value === undefined) return;

        const valueElement = this.querySelector('.value');

        if (valueElement) {
            valueElement.textContent = `AVG/Day: $${value}`;
        }
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