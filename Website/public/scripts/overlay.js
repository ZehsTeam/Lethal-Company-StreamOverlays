const overlayDiv = document.querySelector("#overlay");
const crewText = document.querySelector("#crew .value");
const moonText = document.querySelector('#moon .value');
const weatherIcon = document.querySelector('#moon .icon');
const dayText = document.querySelector('#day .value');
const quotaText = document.querySelector('#quota .value');
const lootText = document.querySelector('#loot .value');
const averagePerDay = document.querySelector('#average-per-day .value');

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
    setDayCount(data.day);
    setQuota(data.quota);
    setLoot(data.loot)
    setAveragePerDay(data.averagePerDay);
}

function setOverlayVisibility(value) {
    if (value === undefined) {
        return;
    }

    if (overlayDiv === undefined) {
        return;
    }

    if (value) {
        overlayDiv.classList.remove('hidden');
    } else {
        overlayDiv.classList.add('hidden');
    }
}

function setCrewCount(value) {
    if (value === undefined) {
        return;
    }

    if (crewText === null) {
        return;
    }

    crewText.textContent = `Crew: ${value}`;
}

function setMoon(value) {
    if (value === undefined) {
        return;
    }

    if (moonText === null) {
        return;
    }
    
    moonText.textContent = `Moon: ${value}`;
}

function setWeather(value) {
    if (value === undefined) {
        return;
    }

    if (weatherIcon === null) {
        return;
    }

    weatherIcon.innerHTML = getWeatherIconCode(value);
}

function setWeatherIconVisibility(value) {
    if (value === undefined) {
        return;
    }

    if (weatherIcon === null) {
        return;
    }

    if (value) {
        weatherIcon.classList.remove('collapse');
    } else {
        weatherIcon.classList.add('collapse');
    }
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

function setDayCount(value) {
    if (value === undefined) {
        return;
    }

    if (dayText === null) {
        return;
    }

    dayText.textContent = `Day: ${value}`;
}

function setQuota(value) {
    if (value === undefined) {
        return;
    }

    if (quotaText === null) {
        return;
    }

    quotaText.textContent = `Quota: $${value}`;
}

function setLoot(value) {
    if (value === undefined) {
        return;
    }

    if (lootText === null) {
        return;
    }

    lootText.textContent = `Loot: $${value}`;
}

function setAveragePerDay(value) {
    if (value === undefined) {
        return;
    }

    if (averagePerDay === null) {
        return;
    }

    averagePerDay.textContent = `Average per day: $${value}`;
}

console.log("WebSocket Port:", webSocketPort);

// Initiate WebSocket connection
connectWebSocket();