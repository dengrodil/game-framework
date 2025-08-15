# Game Framework
Unreal-inspired game app lifecycle framework

Unity doesn't really have anything out-of-the-box to handle bootstrapping, loading of another scene while keeping data integrity, handling player data, etc. In constrast, Unreal has this out-of-the box. This plugin brings some of those Unreal functionality to Unity.
I have used this library across multiple games made by our indie group. We're pulling our player's data from Playfab.

`Actual code can be found in Assets/Sylpheed/GameFramework`

# Features
- `GameMode` to encapsulate one real game scene. This can contain a single or multiple Unity scenes. `GameMode` can receive data when it's loaded.
- `Pawn` to represent a player or AI-controlled entity.
- Multiplayer-compatible through `Player` component. This represents a single player/account, including its data. `Player` is bound to a `Pawn`.
- Handle `Player'`s data through `PlayerState`. This represents a set of related data (eg. inventory) and is bound to the `Player`.
- Manage all config-related assets in a single acces point through `ConfigRepository`. We usually store `ScriptableObject` but you can use any `UnityEngine.Object`.


# Components
## Pawn
- Represents a player or AI-controlled entity. This is in contrast to Unity's `GameObject where it can represent other objects (eg. Light) aside from units, players, enemies, etc.
- When a `GameMode` loads it expects and initializes a default `Pawn` that the player can use.

## GameMode
- Represents an actual one playable scene. This is in contrast to Unity's `Scene` where it can be non-game related (eg. terrain, UI, etc.).
- Can contain a single or multiple Unity `Scene`.
- Override `OnLoad` and/or `OnUnload` to initialize and cleanup `GameMode`.
- Initialize the `Pawn` by overriding `OnInitialize` method.
- Handle passed data through `OnParseData`. For testing, you can hardcode `OnWriteDefaultData` and load the `GameMode` as a standalone.

## Player
- Represents an actual player, including its data. This can be used for a single or multiple players.
- Multiplayer-compatible.
- `Player` is bound to the `Pawn` they are controlling. The `Player` object is not the actual entity you're controlling. Think of it as the soul and data of the `Pawn`.
- `Player.Local` to track the local player.
- Attach this beside the `Pawn`.

## PlayerState
- Holds and handles all `Player`'s data.
- PlayerState should only represent a single category/group of data. The `Player` can hold multiple `PlayerStates`.
- This is where serialization happens for data persistence. Handle this in the derived class.

# Todo
- Convert into a UPM-friendly plugin
- Replace IEnumerator with awaitable methods.
- Type-safe GameMode data.
- More improvements...
