# v1.6.0

- Updated vanilla weather icons: DustClouds, Rainy, Foggy, Flooded, Eclipsed
- Updated weather icons for the [Wesleys Weathers](https://thunderstore.io/c/lethal-company/p/Magic_Wesley/Wesleys_Weathers/) mod v1.2.11
- Added weather icons for the [CodeRebirth](https://thunderstore.io/c/lethal-company/p/XuXiaolan/CodeRebirth/) mod v1.6.1
- Added weather icons for the [LegendWeathers](https://thunderstore.io/c/lethal-company/p/Zigzag/LegendWeathers/) mod v2.2.1
- Added weather icons for the [LethalElementsTheta](https://thunderstore.io/c/lethal-company/p/pacoito/LethalElementsTheta/) mod v1.3.5
- Added weather icons for the [MrovWeathers](https://thunderstore.io/c/lethal-company/p/mrov/MrovWeathers/) mod v0.0.7

Thanks to [Zeta](https://www.twitch.tv/zetaarcade) for all of the new weather icons.

## v1.5.2

- Fixed a compatibility error with older versions of Lethal Company (regression introduced in v1.5.0)

## v1.5.1

- Added compatibility with the [Wesleys Weathers](https://thunderstore.io/c/lethal-company/p/Magic_Wesley/Wesleys_Weathers/) mod v1.2.10
  - Added icons for all weathers, thanks to [Tomatobird8](https://www.twitch.tv/tomatobird8).

## v1.5.0

- Added compatibility with the [HQoL](https://thunderstore.io/c/lethal-company/p/HQHQTeam/HQoL) mod v1.0.8 ([#3](https://github.com/ZehsTeam/Lethal-Company-StreamOverlays/pull/3))
  - Includes the total scrap value in the ship storage in the loot stat.
- Fixed compatibility with the [ShipInventoryUpdated](https://thunderstore.io/c/lethal-company/p/LethalCompanyModding/ShipInventoryUpdated/) mod v2.0.7
- Fixed the average collected scrap per day stat being out of sync for non-host clients.

## v1.4.5

- Updated manifest to show that this mod works on v73+
- Hopefully fixed some bugs with the average collected scrap per day stat.

## v1.4.4

- Fixed compatibility with the [ShipInventoryUpdated](https://thunderstore.io/c/lethal-company/p/LethalCompanyModding/ShipInventoryUpdated/) mod v1.2.9+

## v1.4.3

- Fixed compatibility with the [ShipInventory](https://thunderstore.io/c/lethal-company/p/WarperSan/ShipInventory/) mod v1.2.6+
- This mod is compatible with the [ShipInventoryUpdated](https://thunderstore.io/c/lethal-company/p/SoftDiamond/ShipInventoryUpdated/) mod v1.2.8+

## v1.4.2

- Fixed the server trying to start not stopping properly if another Lethal Company instance has already started the server on the same ports.
  - You can now join two or more clients in LAN mode with the `AutoStart` config setting enabled under the **Server** category.

## v1.4.1

- Fixed compatibility with the [ShipInventory](https://thunderstore.io/c/lethal-company/p/WarperSan/ShipInventory/) mod v1.2.0+
- Dead player bodies will no longer influence the average scrap collected per day stat.

## v1.4.0

- Added support for all game versions v40 to v69+
- Fixed the loot stat not updating after losing all your scrap.

## v1.3.0

- Added networking to sync some values with clients.
  - This mod still works in vanilla lobbies.
  - You can still play with players that don't have this mod installed.
- The Day stat now shows the day number without counting days spent at the company.

## v1.2.0

- Updated config settings.
- Added the ability to change the formatting of each stat in the config settings.
- Removed `DayOffset` config setting in the **Overlays** category.
- Added `OnlyUpdateEndOfDay` config setting to the **Loot Stat** category.
  - *Description: If enabled, the Loot stat will only update when the day ends or if you are in orbit.*
- Updated overlays.
- Other changes and improvements.

## v1.1.0

- Added a new stat to display the average collected scrap per day.
- Added a new overlay.
  - URL: `http://localhost:8080/overlay2`  
  - Displays: Crew, Moon, Day, Quota, Loot, Average per day 
- Added standalone overlays for specific stats
  - **Crew**
    - URL: `http://localhost:8080/crew`  
    - Displays: Player count  
  - **Moon & Weather**
    - URL: `http://localhost:8080/moon`  
    - Displays: Current moon and weather
  - **Day**
    - URL: `http://localhost:8080/day`  
    - Displays: Day count  
  - **Quota**
    - URL: `http://localhost:8080/quota`  
    - Displays: Quota 
  - **Loot**
    - URL: `http://localhost:8080/loot`  
    - Displays: Loot on the ship  
  - **Average Per Day**
    - URL: `http://localhost:8080/averageperday`  
    - Displays: Average scrap collected per day

See the README for full details.

## v1.0.4

- Fixed issue that prevented the overlay from connecting on other computers in the same network.

## v1.0.3

- Added support for the [ShipInventory](https://thunderstore.io/c/lethal-company/p/WarperSan/ShipInventory/) mod.

## v1.0.2

- Fixed issue with the overlay not working.

## v1.0.1

- Initial release.
