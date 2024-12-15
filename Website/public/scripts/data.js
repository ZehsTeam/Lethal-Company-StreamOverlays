const reconnectInterval = 5000; // milliseconds.

let webSocket;

// Function to connect to the WebSocket server
function connectWebSocket() {
    webSocket = new WebSocket(`ws://${window.location.hostname}:${webSocketPort}/overlay`); // Connect to the WebSocket server

    webSocket.onopen = () => {
        console.log("Connected to WebSocket server.");
    };

    webSocket.onmessage = (event) => webSocket_OnMessage(event);

    webSocket.onclose = () => {
        console.log("WebSocket connection closed. Attempting to reconnect...");
        setOverlayVisibility(false);
        setTimeout(connectWebSocket, reconnectInterval);
    };

    webSocket.onerror = (error) => {
        console.error("WebSocket error:", error);
        webSocket.close();  // Ensure WebSocket is closed before retrying
    };
}

function webSocket_OnMessage(event) {
    const data = JSON.parse(event.data);

    setOverlayVisibility(data.visible);
    setCrewCount(data.crew);
    setMoon(data.moon);
    setWeather(data.weather);
    setWeatherIconVisibility(data.showWeatherIcon);
    setDayCount(data.day, 1);
    setQuota(1, data.quota);
    setLoot(data.loot)
    setAveragePerDay(data.averagePerDay);
}

function setOverlayVisibility(value) {
    if (value === undefined) {
        return;
    }

    const elements = document.querySelectorAll('custom-overlay');

    if (elements.length === 0) {
        return;
    }

    elements.forEach(element => {
        if (value) {
            element.classList.remove('hidden');
        } else {
            element.classList.add('hidden');
        }
    });
}

function setCrewCount(value) {
    if (value === undefined) {
        return;
    }

    const elements = document.querySelectorAll('custom-crew');

    if (elements.length === 0) {
        return;
    }

    elements.forEach(element => {
        element.setValue(value);
    });
}

function setMoon(value) {
    if (value === undefined) {
        return;
    }

    const elements = document.querySelectorAll('custom-moon');

    if (elements.length === 0) {
        return;
    }

    elements.forEach(element => {
        element.setValue(value);
    });
}

function setWeather(value) {
    if (value === undefined) {
        return;
    }

    const elements = document.querySelectorAll('custom-moon');

    if (elements.length === 0) {
        return;
    }

    const weatherIconCode = getWeatherIconCode(value);

    elements.forEach(element => {
        element.setWeatherIcon(weatherIconCode);
    });
}

function setWeatherIconVisibility(value) {
    if (value === undefined) {
        return;
    }

    const elements = document.querySelectorAll('custom-moon');

    if (elements.length === 0) {
        return;
    }

    elements.forEach(element => {
        element.setWeatherIconVisibility(value);
    });
}

function getWeatherIconCode(weather) {
    if (weather === undefined) {
        return "";
    }

    const weatherIconCodes = {
        none: "&#xe900;",
        dustclouds: "&#xe906;",
        rainy: "&#xe901;",
        stormy: "&#xe903;",
        foggy: "&#xe904;",
        flooded: "&#xe902;",
        eclipsed: "&#xe905;"
    };

    return weatherIconCodes[weather.toLowerCase()] || "";
}

function setDayCount(day, dayInQuota) {
    if (day === undefined) return;
    if (dayInQuota === undefined) return;

    const elements = document.querySelectorAll('custom-day');

    if (elements.length === 0) {
        return;
    }

    elements.forEach(element => {
        element.setValue(day, dayInQuota);
    });
}

function setQuota(quotaIndex, quota) {
    if (quotaIndex === undefined) return;
    if (quota === undefined) return;

    const elements = document.querySelectorAll('custom-quota');

    if (elements.length === 0) {
        return;
    }
    
    elements.forEach(element => {
        element.setValue(quotaIndex, quota);
    });
}

function setLoot(value) {
    if (value === undefined) {
        return;
    }

    const elements = document.querySelectorAll('custom-loot');

    if (elements.length === 0) {
        return;
    }
    
    elements.forEach(element => {
        element.setValue(value);
    });
}

function setAveragePerDay(value) {
    if (value === undefined) {
        return;
    }

    const elements = document.querySelectorAll('custom-averageperday');

    if (elements.length === 0) {
        return;
    }
    
    elements.forEach(element => {
        element.setValue(value);
    });
}

console.log("WebSocket Port:", webSocketPort);

// Initiate WebSocket connection
connectWebSocket();