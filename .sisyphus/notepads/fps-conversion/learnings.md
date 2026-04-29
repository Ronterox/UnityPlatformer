# FPS Conversion Learnings

## EnemyProjectile.cs Creation (2026-04-29)

### Pattern Reference: OnTriggerEnter collision with Player
From Spiky.cs:30-37:
```csharp
void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        GameManager.Instance.LoseLife();
    }
}
```

### Pattern Reference: LoseLife() call
From GameManager.cs:52-59:
```csharp
public void LoseLife() 
{
    lives--;
    StartCoroutine(RespawnPlayer(2.0f));
    Debug.Log("Lives " + lives);
}
```

### Pattern Reference: ForceMode.Impulse
ThirdPersonController.cs uses gravity with `Gravity * Time.deltaTime` but for projectiles we use:
```csharp
rb.AddForce(transform.forward * speed, ForceMode.Impulse);
```

### Key Decisions
- speed = 20f (as specified)
- lifetime = 5f (auto-destroy safety)
- damage = 1 (placeholder - LoseLife() doesn't take damage param)
- No gravity on projectile (fast, not arced)

### Notes
- Spiky uses CompareTag("Player") not layer comparison
- Destroy(gameObject, lifetime) as safety cleanup in Start()
- rb required on prefab (checked with Debug.LogError in Start)

## EnemyStateMachine.cs Creation (2026-04-29)

### Player Reference Pattern
From GameManager.cs:26 - `player` is a Transform:
```csharp
public Transform player, checkpoint;
```
Access via: `GameManager.Instance.player`

### NavMeshAgent Control Methods
- `agent.SetDestination(Player.position)` - moves agent to target
- `agent.Stop()` - deprecated, use `agent.isStopped = true`
- `agent.Resume()` - deprecated, use `agent.isStopped = false`
- `agent.isStopped` - current preferred way to pause/resume movement

### State Machine Structure
Each state has its own Update method (Idle_Update, Chasing_Update, Shooting_Update) called from main Update() switch.

### State Transitions
- Idle → Chasing: player distance < chaseRange (15m)
- Chasing → Shooting: player distance < attackRange (8m)
- Chasing → Idle: player distance > chaseRange (15m)
- Shooting → Chasing: player distance > attackRange (8m) after firing

### FireProjectile() Implementation
```csharp
Vector3 direction = (Player.position - firePoint.position).normalized;
GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
Rigidbody rb = bullet.GetComponent<Rigidbody>();
if (rb != null)
{
    rb.linearVelocity = direction * 20f;
}
```
Uses Quaternion.LookRotation to orient bullet toward player, sets velocity directly on Rigidbody.

### Key Values (from plan)
- chaseRange = 15f
- attackRange = 8f
- shootCooldown = 2f
- bullet speed = 20f## WeaponSystem Findings (2026-04-29)

### Pattern: Raycast-based weapon
- Physics.Raycast(origin, direction, out hit, range, layerMask)
- Layer mask for Enemy only - must be configured in Unity Layer Manager
- Origin: GunBarrelEnd child transform (auto-find if null), fallback to Camera.main

### Pattern: Fire cooldown
- _fireCooldownTimer decrements in Update
- Resets to FireCooldown on successful fire
- Input.attack boolean (not held) checked against cooldown

### Pattern: Input callback
- StarterAssetsInputs.OnAttack(InputValue) → AttackInput(bool)
- attack boolean stored for polling in WeaponSystem.Update

### Enemy damage
- EnemyHealth.TakeDamage(1) called on raycast hit
- No LoseLife() call - player weapon only damages enemies

### Note
- Enemy layer must exist in Unity Layer Manager for EnemyLayerMask to work
- GunBarrelEnd child transform must be named exactly 'GunBarrelEnd' or assigned in inspector

# FPS Controller Implementation Learnings

## Created Files
- `Assets/SourceFiles/Scripts/FPSController.cs` - New FPS camera rotation controller

## Key Patterns Found

### CameraRotation (ThirdPersonController.cs:237-270)
- Yaw accumulated without clamping (`float.MinValue, float.MaxValue`)
- Pitch clamped between BottomClamp/TopClamp
- Uses `_cinemachineTargetYaw` and `_cinemachineTargetPitch` as accumulators
- Rotation applied via `Quaternion.Euler(pitch + CameraAngleOverride, yaw, 0)`

### ClampAngle pattern (line 416-421)
```csharp
private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
{
    if (lfAngle < -360f) lfAngle += 360f;
    if (lfAngle > 360f) lfAngle -= 360f;
    return Mathf.Clamp(lfAngle, lfMin, lfMax);
}
```

### StarterAssetsInputs pattern
- `OnLook(InputValue value)` calls `LookInput(value.Get<Vector2>())`
- Look stored as `public Vector2 look`
- Cursor locked by default in `Awake()`

### MyPlayer.prefab hierarchy
- `MyPlayer` (root)
  - `Body` (CharacterController, ThirdPersonController, StarterAssetsInputs, PlayerInput, Animator)
    - `CameraRoot` (CinemachineTarget tag, child of Body) - used for camera rotation target
    - `character-female-b` (model)
  - `Camera` (actual camera with MainCamera tag)
  - `CameraFollow` (CinemachineVirtualCamera)

### InputSystem_Actions.inputactions
- "Attack" action exists, not "fire"
- Attack bound to: leftButton (mouse), buttonWest (gamepad), trigger (joystick)
- Plan mentions `StarterAssetsInputs.fire` boolean, but only `attack` exists in current input actions

## Decisions Made
1. FPSController uses CameraRoot transform (like ThirdPersonController uses CinemachineCameraTarget)
2. Added `fire` boolean to StarterAssetsInputs to match plan requirements
3. Added `OnFire(InputValue value)` callback mapped to Attack action in input system
4. Fire rate: 0.5s between shots (as specified)
5. Pitch clamped -70 to +70 degrees (as specified)
6. Yaw unlimited rotation (as specified)
7. No GameManager.Instance.LoseLife() called - raycast only, no damage logic

## Notes
- The plan says `StarterAssetsInputs.fire` but input actions have "Attack" not "Fire"
- FPSController looks for `_input.fire` boolean which I added to StarterAssetsInputs
- Need to bind OnFire to Attack action in PlayerInput component
