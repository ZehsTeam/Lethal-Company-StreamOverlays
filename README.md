# StreamOverlays
#### Display real-time game stats on your stream using browser sources. Shows the player count, current moon, weather, day count, quota, ship loot, and average per day. Works with OBS, Streamlabs OBS, and similar software for Twitch, YouTube, TikTok, Kick, and more.

#### <ins>This mod is fully client-side and works in vanilla lobbies.</ins>
#### This mod has integration with the [ShipInventory](https://thunderstore.io/c/lethal-company/p/WarperSan/ShipInventory/) mod.

## <img src="https://i.imgur.com/TpnrFSH.png" width="20px"> Download

Download [StreamOverlays](https://thunderstore.io/c/lethal-company/p/Zehs/StreamOverlays/) on Thunderstore.

## Features
This mod hosts a local website that you can use as a browser source in OBS, Streamlabs OBS, or other streaming applications.

The overlays provide real-time game data, including:
- Player count
- Current moon
- Current weather
- Day count
- Quota
- Loot on the ship
- Average collected scrap per day

## Installation & Usage
### Step 1: Installation
Install the mod using one of the following mod managers:  
- [Thunderstore Mod Manager](https://www.overwolf.com/app/thunderstore-thunderstore_mod_manager)  
- [r2modman](https://thunderstore.io/c/lethal-company/p/ebkr/r2modman/)  
- [Gale Mod Manager](https://thunderstore.io/c/lethal-company/p/Kesomannen/GaleModManager/)  

### Step 2: Launch the Game
Ensure the game is running. The overlays will not work otherwise.

### Step 3: Add the Browser Source
1. Add this URL as a browser source in your streaming application (e.g., OBS, Streamlabs OBS):  
   ```
   http://localhost:8080/overlay
   ```  
2. Adjust the browser source settings:  
   - **Width**: Set to **1535** or **1920** *(Flexible)*  
   - **Height**: Set to **75**

#### Additional Notes:
- **Streaming from another PC?** Replace `localhost` in the URL with the local IP address of the PC running the game (e.g., `192.168.0.1`).  
- **Troubleshooting:** If the overlay doesn't appear, refresh the browser source in your streaming application. 

## Config Settings
This mod uses a global config file.

You can locate the config file at:
```
%localappdata%\..\LocalLow\ZeekerssRBLX\Lethal Company\StreamOverlays\global.cfg
```

> **Tip:** You can use the [LethalConfig](https://thunderstore.io/c/lethal-company/p/AinaVT/LethalConfig/) mod to easily edit the config settings in-game.

## Overlays
Choose from multiple overlays to customize your stream.  
> **Tip:** You can use multiple overlays simultaneously.  

<details><summary>Click to Expand</summary>

### Default Overlay
- URL: `http://localhost:8080/overlay`  
- Displays: **Crew**, **Moon**, **Day**, **Quota**, **Loot**  
- Settings:  
  - **Width**: 1535 or 1920 *(Flexible)*  
  - **Height**: 75  

### Overlay 2
- URL: `http://localhost:8080/overlay2`  
- Displays: **Crew**, **Moon**, **Day**, **Quota**, **Loot**, **Average per day**  
- Settings:  
  - **Width**: 1800 or 1920 *(Flexible)*  
  - **Height**: 75  

### Individual Overlays
#### Crew  
- URL: `http://localhost:8080/crew`  
- Displays: Player count  
- Settings:  
  - **Width**: 500 *(Flexible)*  
  - **Height**: 75  

#### Moon & Weather
- URL: `http://localhost:8080/moon`  
- Displays: Current moon and weather
- Settings:  
  - **Width**: 500 *(Flexible)*  
  - **Height**: 75  

#### Day  
- URL: `http://localhost:8080/day`  
- Displays: Day count  
- Settings:  
  - **Width**: 500 *(Flexible)*  
  - **Height**: 75  

#### Quota  
- URL: `http://localhost:8080/quota`  
- Displays: Quota  
- Settings:  
  - **Width**: 500 *(Flexible)*  
  - **Height**: 75  

#### Loot  
- URL: `http://localhost:8080/loot`  
- Displays: Loot on the ship  
- Settings:  
  - **Width**: 500 *(Flexible)*  
  - **Height**: 75  

#### Average Per Day  
- URL: `http://localhost:8080/averageperday`  
- Displays: Average scrap collected per day  
- Settings:  
  - **Width**: 500 *(Flexible)*  
  - **Height**: 75  

</details>

## Compatibility & Integration
- This mod has integration with the [LethalConfig](https://thunderstore.io/c/lethal-company/p/AinaVT/LethalConfig/) mod.
  - Allows you to easily edit the config settings in-game.
<br><br>
- This mod has integration with the [ShipInventory](https://thunderstore.io/c/lethal-company/p/WarperSan/ShipInventory/) mod.
  - Includes the total scrap value in the ship inventory in the loot stat.

## Developer Contact
#### Report bugs, suggest features, or provide feedback:  
- **GitHub Issues Page:** [StreamOverlays](https://github.com/ZehsTeam/Lethal-Company-StreamOverlays/issues)  

| **Discord Server** | **Forum** | **Post** |  
|--------------------|-----------|----------|  
| [Lethal Company Modding](https://discord.gg/XeyYqRdRGC) | `#mod-releases` | [StreamOverlays](https://discord.com/channels/1168655651455639582/1309938877405855856) |  
| [Unofficial Lethal Company Community](https://discord.gg/nYcQFEpXfU) | `#mod-releases` | [StreamOverlays](https://discord.com/channels/1169792572382773318/1309939026744053860) |  

- **Email:** crithaxxog@gmail.com  
- **Twitch:** [CritHaxXoG](https://www.twitch.tv/crithaxxog)  
- **YouTube:** [Zehs](https://www.youtube.com/channel/UCb4VEkc-_im0h8DKXlwmIAA)

## Credits
* <a href="https://www.flaticon.com/free-icons/sun" title="sun icons">Sun icons created by Good Ware - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/cloudy" title="cloudy icons">Cloudy icons created by bqlqn - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/rain" title="rain icons">Rain icons created by bqlqn - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/flash" title="flash icons">Flash icons created by Smashicons - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/climate" title="climate icons">Climate icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/river" title="river icons">River icons created by Freepik - Flaticon</a>
* <a href="https://www.flaticon.com/free-icons/eclipse" title="eclipse icons">Eclipse icons created by Muhamad Ulum - Flaticon</a>

## Screenshots
<img src="https://i.imgur.com/JCtTmEZ.png">
<img src="https://i.imgur.com/Twfxu0z.png">
<img src="https://i.imgur.com/MugOPwD.png">
<img src="https://i.imgur.com/80GrHQ2.png">
<img src="https://i.imgur.com/wRUpp0P.png">
<img src="https://i.imgur.com/ulibb6U.png">
<img src="https://i.imgur.com/qYbWabP.png">
<img src="https://i.imgur.com/rAjjhMP.png">
<img src="https://i.imgur.com/YgtE1PN.png">
<img src="https://i.imgur.com/vVFPtye.png">
<img src="https://i.imgur.com/8p9mwHr.png">
<img src="https://i.imgur.com/4stZ8hE.png">
<img src="https://i.imgur.com/k4JgrEQ.png">
<img src="https://i.imgur.com/yDfkJKd.png">