const overlayDiv = document.querySelector("#overlay");
const crewText = document.querySelector("#crew .value");
const moonText = document.querySelector('#moon .value');
const weatherIcon = document.querySelector('#moon .icon');
const dayText = document.querySelector('#day .value');
const quotaText = document.querySelector('#quota .value');
const lootText = document.querySelector('#loot .value');

const sourceId = 'overlay';

const reconnectInterval = 5000; // milliseconds.

let webSocket;

// Function to connect to the WebSocket server
function connectWebSocket() {
    webSocket = new WebSocket(`ws://localhost:${serverPort}`); // Connect to the WebSocket server

    webSocket.onopen = () => {
        console.log("Connected to WebSocket server.");
        
        // Send a request to the Node.js server for the latest data
        requestLatestData();
    };

    webSocket.onmessage = (event) => webSocket_OnMessage(event);

    webSocket.onclose = () => {
        console.log("WebSocket connection closed. Attempting to reconnect...");
        hideOverlay();
        setTimeout(connectWebSocket, reconnectInterval);
    };

    webSocket.onerror = (error) => {
        console.error("WebSocket error:", error);
        webSocket.close();  // Ensure WebSocket is closed before retrying
    };
}

// Function to request the latest data from Node.js server
function requestLatestData() {
    if (webSocket && webSocket.readyState === WebSocket.OPEN) {
        webSocket.send(JSON.stringify({ request: "latestData" }));
    }
}

function webSocket_OnMessage(event) {
    const data = JSON.parse(event.data);

    if (data.source === undefined || data.source !== sourceId) {
        return;
    }

    if (data.visible !== undefined) {
        if (data.visible) {
            overlayDiv.classList.remove('hidden');
        } else {
            overlayDiv.classList.add('hidden');
        }
    }

    if (data.crew !== undefined) {
        crewText.textContent = `Crew: ${data.crew}`;
    }

    if (data.moon !== undefined) {
        moonText.textContent = `Moon: ${data.moon}`;
    }

    if (data.weather !== undefined) {
        weatherIcon.innerHTML = getWeatherIconCode(data.weather);
    }

    if (data.showWeatherIcon !== undefined) {
        if (data.showWeatherIcon) {
            weatherIcon.classList.remove('collapse');
        } else {
            weatherIcon.classList.add('collapse');
        }
    }

    if (data.day !== undefined) {
        dayText.textContent = `Day: ${data.day}`;
    }

    if (data.quota !== undefined) {
        quotaText.textContent = `Quota: $${data.quota}`;
    }

    if (data.loot !== undefined) {
        lootText.textContent = `Loot: $${data.loot}`;
    }
}

function getWeatherIconCode(weather) {
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

function hideOverlay() {
    overlayDiv.classList.add('hidden');
}

console.log("Server Port:", serverPort);

// Initiate WebSocket connection
connectWebSocket();