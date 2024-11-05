const overlayDiv = document.querySelector("#overlay");
const crewText = document.querySelector("#crew .value");
const moonText = document.querySelector('#moon .value');
const dayText = document.querySelector('#day .value');
const quotaText = document.querySelector('#quota .value');
const lootText = document.querySelector('#loot .value');

const sourceId = 'overlay';

const serverPort = 8000;
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

// Initiate WebSocket connection
connectWebSocket();