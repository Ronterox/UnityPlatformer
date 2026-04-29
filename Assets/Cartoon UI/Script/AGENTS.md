# Cartoon UI Directory

**Parent:** `Assets/Cartoon UI/`

## OVERVIEW
UI prefabs and scripts. Separate from main game scripts in SourceFiles/.

## FILES
| File | Lines | Purpose |
|------|-------|---------|
| MainMenu.cs | ~100 | Main menu navigation, scene transitions |
| StoreScript.cs | ~80 | Store/in-game purchases |
| MainControls.cs | ~60 | Controls display / keybindings |

## WHERE TO LOOK
| Task | File |
|------|------|
| Menu buttons | MainMenu.cs |
| Scene transitions | MainMenu.cs:LoadScene() |
| Store logic | StoreScript.cs |

## CONVENTIONS
- No namespace
- MonoBehaviour scripts on UI GameObjects
- Scene loading via `SceneManager.LoadScene()`

## ANTI-PATTERNS
- Scripts separated from main game logic in SourceFiles/Scripts
- No common UI base class or inheritance

## NOTES
- Uses Unity UI (uGUI) not UIElements
- Cartoon style assets included