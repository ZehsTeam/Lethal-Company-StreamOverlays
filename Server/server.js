const express = require('express');
const WebSocket = require('ws');
const path = require('path');
const fs = require('fs');

const clientPort = 8000; // Port for the OBS browser source (WebSocket & HTTP server)
const unityPort = 8080; // Port for Unity WebSocket

const app = express();

// Serve all static files from the "public" folder
app.use(express.static(path.join(__dirname, 'public')));

// Middleware to handle requests without an extension
app.get('/:source', (req, res, next) => {
    const source = req.params.source;
    const htmlPath = path.join(__dirname, 'public', `${source}.html`);
    const sourcePath = path.join(__dirname, 'public', source);

    // Check if the HTML file exists
    if (path.extname(source) === '' && fs.existsSync(htmlPath)) {
        // Serve the HTML file if no extension was provided
        res.sendFile(htmlPath);
    } else {
        // Fall back to static file serving
        res.sendFile(sourcePath, (err) => {
            if (err) {
                res.status(404).send('File not found');
            }
        });
    }
});

// Start the server
const server = app.listen(clientPort, () => {
    console.log(`Web server for OBS running at http://localhost:${clientPort}`);
});

// WebSocket Server for OBS browser source
const clientWSS = new WebSocket.Server({ server });
let unitySocket = null; // Store Unity WebSocket connection

// WebSocket server for Unity on a separate port
const unityWSS = new WebSocket.Server({ port: unityPort });
console.log(`WebSocket server for Unity running on ws://localhost:${unityPort}`);

// Handle Unity WebSocket connections
unityWSS.on('connection', (socket) => {
    if (unitySocket) {
        console.log("Connection attempt from Unity denied: an existing connection is already active.");
        socket.close(1000, "Only one Unity connection allowed at a time.");
        return;
    }

    console.log("Unity connected to WebSocket server.");
    unitySocket = socket;

    socket.on('message', (message) => {
        // Convert Buffer to string before parsing
        const messageString = message.toString();

        console.log("Received message from Unity:", messageString);

        // Broadcast the message to all connected OBS clients
        clientWSS.clients.forEach(client => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(messageString);
            }
        });
    });

    socket.on('close', () => {
        console.log("Unity disconnected.");
        unitySocket = null;

        // Notify OBS clients that Unity has disconnected
        clientWSS.clients.forEach(client => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(JSON.stringify({ source: 'overlay', visible: false }));
            }
        });
    });

    socket.on('error', (error) => {
        console.error("Unity WebSocket error:", error);
        unitySocket = null;
    });
});

// Handle OBS client WebSocket connections
clientWSS.on('connection', (clientSocket) => {
    console.log("OBS Browser Source connected.");

    clientSocket.on('message', (message) => {
        const clientMessage = JSON.parse(message);

        // If OBS requests latest data and Unity is connected, request from Unity
        if (clientMessage.request === "latestData") {
            console.log("Requesting latest data from Unity...");

            if (unitySocket && unitySocket.readyState === WebSocket.OPEN) {
                unitySocket.send(JSON.stringify({ request: "latestData" }));
            } else {
                // Notify OBS that Unity is not connected
                clientWSS.clients.forEach(client => {
                    if (client.readyState === WebSocket.OPEN) {
                        client.send(JSON.stringify({ source: 'overlay', visible: false }));
                    }
                });
            }
        }
    });

    clientSocket.on('close', () => {
        console.log("OBS Browser Source disconnected.");
    });
});

console.log(`WebSocket server for OBS running on ws://localhost:${clientPort}`);