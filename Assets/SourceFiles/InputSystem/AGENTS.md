# InputSystem Directory

**Parent:** `Assets/SourceFiles/`

## OVERVIEW
Unity's new Input System wrapper. Converts PlayerInput callbacks to readable input values.

## FILES
| File | Lines | Purpose |
|------|-------|---------|
| StarterAssetsInputs.cs | 95 | Input value wrapper |
| StarterAssets.inputactions | - | Input action definitions (JSON) |
| InputSystem_Actions.inputactions | - | Project-specific actions (JSON) |

## WHERE TO LOOK
| Task | Location |
|------|----------|
| Move input | StarterAssetsInputs.cs:OnMove() |
| Jump callback | StarterAssetsInputs.cs:OnJump() |
| Sprint callback | StarterAssetsInputs.cs:OnSprint() |
| Cursor lock | StarterAssetsInputs.cs:SetCursorState() |

## CONVENTIONS
- `#if ENABLE_INPUT_SYSTEM` guards - code only compiles with new input system
- `public Vector2 move, look` - serialized for debugging
- `cursorLocked = true` default - game starts with locked cursor

## ANTI-PATTERNS
- No input buffering - jump is boolean (pressed/not pressed)
- No input rebinding support

## NOTES
- Uses `InputValue` from UnityEngine.InputSystem
- Calls `MoveInput()`, `LookInput()`, etc. for each callback
- Cursor visibility controlled via `Cursor.lockState`