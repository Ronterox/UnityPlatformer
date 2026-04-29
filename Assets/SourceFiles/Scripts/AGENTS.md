# Scripts Directory

**Parent:** `Assets/SourceFiles/`

## OVERVIEW
Core game logic scripts. 11 C# MonoBehaviours controlling player, enemies, collectibles, UI.

## FILES
| File | Lines | Purpose |
|------|-------|---------|
| ThirdPersonController.cs | 472 | Player movement, parkour, climbing, gravity, camera |
| GameManager.cs | 78 | Singleton - lives, respawn, scene transitions |
| StarterAssetsInputs.cs | 95 | Input wrapper (InputSystem) |
| Hearts.cs | ~50 | Lives/heart UI display |
| Pickup.cs | ~60 | Star collectible trigger + rotation |
| UpdateCollectibleCount.cs | ~40 | Star counter UI |
| Checkpoint.cs | ~70 | Respawn point save/load |
| RespawnPlayer.cs | ~50 | Player respawn logic |
| Teleport.cs | ~40 | Teleportation trigger |
| Spiky.cs | ~30 | Hazard damage |
| MotionAudioController.cs | ~50 | Motion-based audio |

## WHERE TO LOOK
| Task | File |
|------|------|
| Jump physics | ThirdPersonController.cs:JumpPhysics() |
| Climb detection | ThirdPersonController.cs:ClimbCheck() |
| Respawn flow | GameManager.cs:RespawnPlayer() |
| Star collection | Pickup.cs:OnTriggerEnter() |

## CONVENTIONS
- Namespace `StarterAssets` for player/input scripts
- No namespace for game logic (GameManager, Pickup, etc.)
- SerializeField private vars with `[Header("...")]` tooltips
- `_` prefix for private fields (`_speed`, `_cinemachineTargetYaw`)

## ANTI-PATTERNS
- GameManager does `Debug.LogError` on missing instance (not thrown exception)
- ThirdPersonController mixes camera logic with movement logic

## NOTES
- ThirdPersonController modifies Time.fixedDeltaTime for custom gravity
- AudioSource components expected on same GameObject as scripts