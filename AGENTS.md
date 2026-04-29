# PROJECT KNOWLEDGE BASE

**Generated:** 2026-04-29
**Updated:** 2026-04-29
**Commit:** ca30024
**Branch:** master

## OVERVIEW
Unity 3D Parkour Platformer. Third-person movement with Roblox-style climbing, collectible stars, checkpoint respawns, lives system.

## STRUCTURE
```
UnityPlatformer/
├── Assets/SourceFiles/
│   ├── Scripts/      # Core game logic (11 C# files)
│   ├── InputSystem/  # Unity Input System wrapper
│   ├── StarterAssets/ # Unity Starter Assets (ThirdPersonController, animations)
│   ├── Animation/   # Platform animator
│   ├── SoundFX/      # Audio files
│   ├── VFX/          # Particle materials
│   ├── Models/       # FBX meshes (platforms, star)
│   ├── Textures/     # Tile textures
│   └── Settings/     # URP renderer assets
├── Assets/Scenes/    # .unity scene files
├── Assets/Cartoon UI/ # UI prefabs + scripts
├── Packages/         # Unity package manifest
└── ProjectSettings/  # Unity editor settings
```

## WHERE TO LOOK
| Task | Location | Notes |
|------|----------|-------|
| Player movement | `Assets/SourceFiles/Scripts/ThirdPersonController.cs` | 471 lines - parkour, climbing, jump, gravity |
| Game state | `Assets/SourceFiles/Scripts/GameManager.cs` | Singleton - lives, respawn, scene mgmt |
| Input handling | `Assets/SourceFiles/InputSystem/StarterAssetsInputs.cs` | Unity Input System wrapper |
| Collectibles | `Assets/SourceFiles/Scripts/Pickup.cs` | Star pickup trigger |
| Checkpoints | `Assets/SourceFiles/Scripts/Checkpoint.cs` | Respawn point save/load |
| UI menu | `Assets/Cartoon UI/Script/MainMenu.cs` | Main menu navigation |

## CONVENTIONS (THIS PROJECT)
- Singleton pattern for GameManager (`GameManager.Instance`)
- `RequireComponent(typeof(CharacterController))` on player scripts
- `#if ENABLE_INPUT_SYSTEM` guards for new input system
- `[Header("...")]` tooltip attributes on serialized fields
- Coroutines for delayed respawn (`IEnumerator RespawnPlayer(float delay)`)

## ANTI-PATTERNS (THIS PROJECT)
- `_Recovery/` folder in Assets - should be removed or gitignored
- No test infrastructure - game logic has no unit tests
- Settings stored in `SourceFiles/Settings/` instead of `ProjectSettings/`
- Cartoon UI scripts in separate folder from main game scripts

## UNIQUE STYLES
- ThirdPersonController uses Cinemachine camera target
- Audio sources attached to same GameObject as script
- Prefabs stored inline in scene, not in dedicated Prefabs folder
- GameManager references player/checkpoint transforms directly (not find by tag)

## COMMANDS
```bash
# Open in Unity Editor (manual)
open -a "Unity"

# Build via Unity CLI (if installed)
/Applications/Unity/Hub/Editor/2022.3.20f1/Unity.app/Contents/MacOS/Unity \
  -projectPath . \
  -buildTarget MacStandalone \
  -quit
```

## NOTES
- 8 scene files: MainScene, Main Menu, YouLose, BuyGame, YouWin, etc.
- ThirdPersonController has custom gravity (-15.0f) overriding engine default (-9.81f)
- No CI pipeline - manual builds only
- URP render pipeline (UniversalRenderPipeline)