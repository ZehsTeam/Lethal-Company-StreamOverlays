const express = require('express');
const WebSocket = require('ws');
const path = require('path');
const fs = require('fs');

process.title = "StreamOverlays Server";

let clientPort = 8000; // Default port for Browser Source (WebSocket & HTTP server)
let unityPort = 8080;  // Default port for Unity WebSocket

// Check command-line arguments
const args = process.argv.slice(2);

let argsClientPort = undefined;
let argsUnityPort = undefined;

// Parse command-line arguments for ports
args.forEach((arg, index) => {
    if (arg === '--clientPort' && index + 1 < args.length) {
        argsClientPort = parseInt(args[index + 1], 10);
    } else if (arg === '--unityPort' && index + 1 < args.length) {
        argsUnityPort = parseInt(args[index + 1], 10);
    }
});

// Determine path to config.json
const configPath = path.join(process.cwd(), 'config.json');

let configClientPort = undefined;
let configUnityPort = undefined;

if (fs.existsSync(configPath)) {
    try {
        const config = JSON.parse(fs.readFileSync(configPath, 'utf8'));

        if (config.clientPort !== undefined) {
            configClientPort = config.clientPort;
        }

        if (config.unityPort !== undefined) {
            configUnityPort = config.unityPort;
        }
    } catch (error) {
        console.error("Error reading config.json:", error);
    }
}

if (argsClientPort !== undefined) {
    clientPort = argsClientPort;
    console.log(`Using clientPort from command-line arguments - clientPort: ${clientPort}`);
} else if (configClientPort !== undefined) {
    clientPort = configClientPort;
    console.log(`Using clientPort from config.json - clientPort: ${clientPort}`);
} else {
    console.log(`Using default clientPort - clientPort: ${clientPort}`);
}

if (argsUnityPort !== undefined) {
    unityPort = argsUnityPort;
    console.log(`Using unityPort from command-line arguments - unityPort: ${unityPort}`);
} else if (configUnityPort !== undefined) {
    unityPort = configUnityPort;
    console.log(`Using unityPort from config.json - unityPort: ${unityPort}`);
} else {
    console.log(`Using default unityPort - unityPort: ${unityPort}`);
}

console.log("");

const app = express();

// Serve all static files from the "public" folder
app.use(express.static(path.join(__dirname, 'public')));

// Serve clientPort as a global variable in a dynamically generated config.js file
app.get('/config.js', (req, res) => {
    res.type('application/javascript');
    res.send(`const serverPort = ${clientPort};`);
});

// Middleware to handle requests without an extension
app.get('/:source', (req, res, next) => {
    const source = req.params.source;
    const htmlPath = path.join(__dirname, 'public', `${source}.html`);
    const sourcePath = path.join(__dirname, 'public', source);

    if (path.extname(source) === '' && fs.existsSync(htmlPath)) {
        res.sendFile(htmlPath);
    } else {
        res.sendFile(sourcePath, (err) => {
            if (err) {
                res.status(404).send('File not found');
            }
        });
    }
});

// Start the server
const server = app.listen(clientPort, () => {
    console.log(`Web server for Browser Sources running at http://localhost:${clientPort}\n`);
    console.log(`Browser Source -> http://localhost:${clientPort}/overlay (Recommended 1450x75)\n`);
});

// WebSocket Server for Browser Sources
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
        const messageString = message.toString();
        console.log("Received message from Unity:", messageString);

        clientWSS.clients.forEach(client => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(messageString);
            }
        });
    });

    socket.on('close', () => {
        console.log("Unity disconnected.");
        unitySocket = null;

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

// Handle Browser Source client WebSocket connections
clientWSS.on('connection', (clientSocket) => {
    console.log("Browser Source connected.");

    clientSocket.on('message', (message) => {
        const clientMessage = JSON.parse(message);

        if (clientMessage.request === "latestData") {
            console.log("Requesting latest data from Unity...");

            if (unitySocket && unitySocket.readyState === WebSocket.OPEN) {
                unitySocket.send(JSON.stringify({ request: "latestData" }));
            } else {
                clientWSS.clients.forEach(client => {
                    if (client.readyState === WebSocket.OPEN) {
                        client.send(JSON.stringify({ source: 'overlay', visible: false }));
                    }
                });
            }
        }
    });

    clientSocket.on('close', () => {
        console.log("Browser Source disconnected.");
    });
});

console.log(`WebSocket server for Browser Sources running on ws://localhost:${clientPort}`);