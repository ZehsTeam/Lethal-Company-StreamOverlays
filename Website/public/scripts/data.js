const reconnectInterval = 5000; // milliseconds.

let webSocket;

// Mapping for element updates
const elementUpdateMapping = {
    'custom-crew': updateGeneric,
    'custom-moon': updateGeneric,
    'custom-day': updateGeneric,
    'custom-quota': updateGeneric,
    'custom-loot': updateGeneric,
    'custom-averageperday': updateGeneric,
};

// Function to connect to the WebSocket server
function connectWebSocket() {
    if (!webSocketPort) {
        console.error("WebSocket Port is not defined. Ensure 'webSocketPort' is set.");
        return;
    }

    webSocket = new WebSocket(`ws://${window.location.hostname}:${webSocketPort}/overlay`);

    webSocket.onopen = () => {
        console.log(`[WebSocket] Connected to server at ws://${window.location.hostname}:${webSocketPort}/overlay`);
        reconnectAttempts = 0; // Reset attempts on successful connection
    };

    webSocket.onmessage = (event) => webSocket_OnMessage(event);

    webSocket.onclose = () => {
        console.warn("[WebSocket] Connection closed. Attempting to reconnect...");
        setOverlayVisible(false);
        setTimeout(connectWebSocket, reconnectInterval);
    };

    webSocket.onerror = (error) => {
        console.error("[WebSocket] Error occurred:", error);
        webSocket.close(); // Ensure WebSocket is closed before retrying
    };
}

// Handle incoming WebSocket messages
function webSocket_OnMessage(event) {
    let data;

    try {
        data = JSON.parse(event.data);
    } catch (error) {
        console.error("[WebSocket] Invalid message received:", error);
        return;
    }

    if (!data || !data.type) {
        console.warn("[WebSocket] Message missing required properties:", data);
        return;
    }

    if (data.type === 'data') {
        setOverlayVisible(data.showOverlay ?? false);
    }

    if (data.type === 'data' || data.type === 'formatting') {
        updateElements(data);
    }
}

// Generalized update function
function updateGeneric(tag, data) {
    const elements = document.querySelectorAll(tag);
    elements.forEach(element => element.update(data));
}

// Update all elements based on mapping
function updateElements(data) {
    for (const tag in elementUpdateMapping) {
        elementUpdateMapping[tag](tag, data);
    }
}

// Set overlay visibility
function setOverlayVisible(value) {
    if (typeof value !== 'boolean') return; // Ensure only boolean values are processed

    const elements = document.querySelectorAll('custom-overlay');
    elements.forEach(element => element.setVisible(value));
}

// Initiate WebSocket connection
connectWebSocket();