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
        setOverlayVisible(false);
        setTimeout(connectWebSocket, reconnectInterval);
    };

    webSocket.onerror = (error) => {
        console.error("WebSocket error:", error);
        webSocket.close();  // Ensure WebSocket is closed before retrying
    };
}

function webSocket_OnMessage(event) {
    const data = JSON.parse(event.data);

    if (data.type === undefined) {
        return;
    }

    if (data.type === 'data' || data.type === 'formatting') {
        setOverlayVisible(data.showOverlay);
        updateCrew(data);
        updateMoon(data);
        updateDay(data);
        updateQuota(data);
        updateLoot(data);
        updateAveragePerDay(data);
    }
}

function setOverlayVisible(value) {
    if (value === undefined) return;

    const elements = document.querySelectorAll('custom-overlay');
    elements.forEach(element => element.setVisible(value));
}

function updateCrew(data) {
    const elements = document.querySelectorAll('custom-crew');
    elements.forEach(element => element.update(data));
}

function updateMoon(data) {
    const elements = document.querySelectorAll('custom-moon');
    elements.forEach(element => element.update(data));
}

function updateDay(data) {
    const elements = document.querySelectorAll('custom-day');
    elements.forEach(element => element.update(data));
}

function updateQuota(data) {
    const elements = document.querySelectorAll('custom-quota');
    elements.forEach(element => element.update(data));
}

function updateLoot(data) {
    const elements = document.querySelectorAll('custom-loot');
    elements.forEach(element => element.update(data));
}

function updateAveragePerDay(data) {
    const elements = document.querySelectorAll('custom-averageperday');
    elements.forEach(element => element.update(data));
}

console.log("WebSocket Port:", webSocketPort);

// Initiate WebSocket connection
connectWebSocket();