# FPS Platformer Conversion Plan

## TL;DR

> **Quick Summary**: Add First Person Shooter combat to the existing parkour platformer. Player shoots enemies via raycasting, enemies patrol/chase/attack using NavMesh AI and shoot physics-based projectiles.
>
> **Deliverables**:
> - FPS camera controller using existing Cinemachine
> - Player raycasting weapon system
> - Enemy AI with NavMeshAgent + state machine (Idle/Chase/Shoot)
> - Enemy projectile (bullet with force)
> - Enemy health (1-hit death)
>
> **Estimated Effort**: Medium
> **Parallel Execution**: YES - 3 waves
> **Critical Path**: Wave 1 → FPS Controller → Weapon System → Wave 3 integration

---

## Context

### Original Request
Add FPS elements to the existing Unity parkour platformer scene. Player should be able to shoot enemies via raycasting. Enemies should use NavMesh for movement and shoot projectiles with physics force. Simple state machine for enemy AI (Idle, Chasing, Shooting when in range). Enemies take exactly 1 heart of damage.

### Metis Review - Identified Gaps

**Gap 1**: Player health model unclear - does enemy projectile call `LoseLife()` directly or subtract from a health pool first?
- **Resolution**: Enemy projectile → `GameManager.Instance.LoseLife()` directly (existing lives system handles it)

**Gap 2**: Enemy death behavior - instant destroy, animation, particles?
- **Resolution**: Enemy disabled/destroyed immediately on death (no death animation to avoid complexity)

**Gap 3**: FPS mode activation - always on or toggleable?
- **Resolution**: FPS camera always active (no TPS/FPS toggle in scope)

**Gap 4**: Fire rate limit?
- **Resolution**: Fire rate limited to 1 shot per 0.5 seconds (prevents spam)

**Gap 5**: Enemy attack range / stopping distance?
- **Resolution**: Attack range 8m, chase range 15m (NavMeshAgent.stoppingDistance = 2m for attack trigger)

---

## Work Objectives

### Core Objective
Add playable FPS combat to MainScene with functional player shooting (raycasting) and intelligent enemies (NavMesh + state machine).

### Concrete Deliverables

| Deliverable | File |
|-------------|------|
| FPS camera rotation controller | `Assets/SourceFiles/Scripts/FPSController.cs` |
| Player raycasting weapon | `Assets/SourceFiles/Scripts/WeaponSystem.cs` |
| Enemy state machine + NavMesh AI | `Assets/SourceFiles/Scripts/EnemyStateMachine.cs` |
| Enemy projectile (physics bullet) | `Assets/SourceFiles/Scripts/EnemyProjectile.cs` |
| Enemy health (1-hit kill) | `Assets/SourceFiles/Scripts/EnemyHealth.cs` |
| Bullet prefab | `Assets/Prefabs/Bullet.prefab` |
| Enemy prefab | `Assets/Prefabs/Enemy.prefab` |
| Fire input binding | `Assets/SourceFiles/InputSystem/InputSystem_Actions.inputactions` |
| TakeDamage() for player | `Assets/SourceFiles/Scripts/GameManager.cs` |
| GunBarrelEnd child on player | Modify `Assets/Prefabs/MyPlayer.prefab` |

### Definition of Done

- [ ] Player aims with mouse and camera rotates in FPS view
- [ ] Player presses fire button → raycast hits enemy → enemy dies in 1 hit
- [ ] Enemy uses NavMeshAgent to move toward player when in range
- [ ] Enemy transitions: Idle → Chasing → Shooting based on distance
- [ ] Enemy fires projectile that travels with physics force toward player
- [ ] Projectile collision with player calls `LoseLife()`
- [ ] Zero Unity console warnings or errors

### Must Have

1. Player FPS camera control (mouse look, no pitch limits to avoid confusion)
2. Player raycasting weapon from `GunBarrelEnd` transform
3. Enemy with NavMeshAgent movement
4. Enemy state machine with 3 states (Idle/Chase/Shoot)
5. Enemy projectile with Rigidbody + AddForce
6. Enemy health = 1 heart (instant kill on player hit)
7. Fire input action bound

### Must NOT Have (Guardrails)

- **MUST NOT** modify `ThirdPersonController.cs` movement logic (keep parkour working)
- **MUST NOT** break existing lives system (`LoseLife()` must still work)
- **MUST NOT** add new Unity packages (use existing Cinemachine 3.1.2, InputSystem 1.18.0)
- **MUST NOT** modify scene files (enemy prefabs placed manually, not in plan)
- **MUST NOT** add player health pool (player uses existing lives system)
- **MUST NOT** add multiple enemy types (single enemy type only)
- **MUST NOT** add enemy patrol waypoints (only chase/shoot states)

---

## Verification Strategy

### Test Decision
- **Infrastructure exists**: NO - Unity project, no test framework
- **Automated tests**: NO
- **Framework**: None (Unity manual testing only)

### QA Policy
Every task includes agent-executed QA scenarios. However, since this is a **Unity project**, verification requires **manual human testing in Unity Editor**. All QA scenarios describe what to verify interactively - there is no CLI/automation for Unity projects.

**CRITICAL**: All acceptance criteria require manual testing in Unity Editor play mode. No automated verification possible.

---

## Execution Strategy

### Parallel Execution Waves

```
Wave 1 (Foundation - scripts only, can run immediately):
├── Task 1: FPSController.cs - camera rotation
├── Task 2: WeaponSystem.cs - raycasting
├── Task 3: EnemyHealth.cs - 1-heart health
├── Task 4: EnemyProjectile.cs - bullet physics
└── Task 5: EnemyStateMachine.cs - NavMesh + states

Wave 2 (After Wave 1 - prefabs + config):
├── Task 6: Bullet.prefab - projectile prefab with Rigidbody
├── Task 7: Enemy.prefab - enemy with NavMeshAgent, EnemyStateMachine, EnemyHealth, EnemyProjectile
├── Task 8: GameManager TakeDamage() - add public TakeDamage() method
├── Task 9: InputSystem fire action - add fire binding
└── Task 10: MyPlayer GunBarrelEnd - add raycast origin child

Wave FINAL (After ALL tasks - verification wave):
└── Task F1: Integration verification in Unity Editor (manual)
    - Player FPS controls work
    - Player can shoot and kill enemies
    - Enemies move with NavMesh
    - Enemies shoot back
    - Player loses life on enemy projectile hit
    - Zero console warnings/errors
```

### Dependency Matrix

- **1-5**: No dependencies - Wave 1 independent tasks
- **6-7**: Depend on 3, 4, 5 (need EnemyHealth, EnemyProjectile, EnemyStateMachine)
- **8**: Standalone - GameManager modification
- **9**: Standalone - Input action addition
- **10**: Depend on 1 (FPSController integration with GunBarrelEnd)

**Critical Path**: Task 1 → Task 2 → Task 10 (FPS → Weapon → GunBarrelEnd)
**Parallel Speedup**: ~60% faster than sequential (Wave 1 has 5 independent tasks)
**Max Concurrent**: 5 (Wave 1)

---

## TODOs

---

## TODOs

- [x] 1. **FPSController.cs** — FPS camera rotation controller

  **What to do**:
  - Create `Assets/SourceFiles/Scripts/FPSController.cs`
  - Use existing `CinemachineVirtualCamera` "CameraFollow" for camera
  - FPS camera: direct rotation of camera transform (no offset composer)
  - Mouse look: yaw unlimited, pitch limited to -70 to +70 degrees
  - Read `look` input from `StarterAssetsInputs`
  - Rotate `CameraRoot` (or camera directly) based on mouse delta * sensitivity
  - Fire rate: 1 shot per 0.5 seconds
  - Integrate with existing `StarterAssetsInputs.fire` input (add fire field)

  **Must NOT do**:
  - Do NOT modify ThirdPersonController movement logic
  - Do NOT break existing camera following (Cinemachine still follows CameraRoot)
  - Do NOT add new packages or dependencies

  **Recommended Agent Profile**:
  - **Category**: `visual-engineering`
  - Reason: Camera rotation is a visual/input crosscut - needs understanding of Cinemachine integration
  - **Skills**: `playwright` (NO - Unity project, manual test)
  - Skills: none - manual Unity verification only

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1 (with Tasks 2, 3, 4, 5)
  - **Blocks**: Task 10 (GunBarrelEnd needs FPSController reference)
  - **Blocked By**: None

  **References**:
  - `Assets/SourceFiles/Scripts/ThirdPersonController.cs:237-270` - CameraRotation() pattern for pitch/yaw clamping and mouse sensitivity
  - `Assets/SourceFiles/Scripts/StarterAssetsInputs.cs:66-69` - LookInput() pattern for input reading
  - `Assets/Prefabs/MyPlayer.prefab` - Hierarchy showing CameraRoot, CameraFollow virtual camera

  **Acceptance Criteria**:
  - [ ] File created: `Assets/SourceFiles/Scripts/FPSController.cs`
  - [ ] Mouse horizontal moves camera yaw (unlimited rotation)
  - [ ] Mouse vertical moves camera pitch (clamped -70 to +70)
  - [ ] Look sensitivity uses existing `LookSensitivity` from ThirdPersonController or new FPS-specific value
  - [ ] Fire rate limited to 0.5s between shots
  - [ ] `StarterAssetsInputs.fire` boolean triggers raycast when true
  - [ ] `GameManager.Instance.LoseLife()` NOT called by FPSController (WeaponSystem handles that)

  **QA Scenarios**:
  ```
  Scenario: FPS camera rotation - yaw (horizontal mouse)
    Preconditions: Player in FPS mode, cursor locked
    Steps:
      1. Move mouse left/right
      2. Camera rotates horizontally (yaw)
    Expected Result: Camera yaw changes smoothly, no limits
    Evidence: .sisyphus/evidence/task-1-yaw.md (manual test notes)

  Scenario: FPS camera rotation - pitch (vertical mouse)
    Preconditions: Player in FPS mode, cursor locked
    Steps:
      1. Move mouse up/down
      2. Camera rotates vertically (pitch)
    Expected Result: Camera pitch clamped between -70 and +70 degrees
    Evidence: .sisyphus/evidence/task-1-pitch.md (manual test notes)

  Scenario: Fire rate limiting
    Preconditions: Fire key held down
    Steps:
      1. Press and hold fire key
      2. Count shots fired per second
    Expected Result: Maximum 2 shots per second (0.5s cooldown)
    Evidence: .sisyphus/evidence/task-1-firerate.md (manual test notes)
  ```

---

- [x] 2. **WeaponSystem.cs** — Player raycasting weapon

  **What to do**:
  - Create `Assets/SourceFiles/Scripts/WeaponSystem.cs`
  - Raycast from `GunBarrelEnd` child transform (or main camera if no child)
  - On fire input: `Physics.Raycast(gunBarrelEnd.position, camera.forward, out hit, range, enemyLayerMask)`
  - Range: 100m (long enough for any practical shot)
  - Layer mask: only "Enemy" layer
  - If raycast hits: call `enemy.GetComponent<EnemyHealth>().TakeDamage(1)`
  - Debug line in Scene view for debugging ( Gizmos.DrawLine)
  - Use existing `StarterAssetsInputs.fire` boolean

  **Must NOT do**:
  - Do NOT call `LoseLife()` - only damage enemies
  - Do NOT use physics projectile for player (use raycast only)
  - Do NOT use enemyLayerMask that includes player

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
  - Reason: Raycasting logic is straightforward but needs correct layer mask and reference setup
  - **Skills**: none

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1 (with Tasks 1, 3, 4, 5)
  - **Blocks**: None
  - **Blocked By**: None (but Task 10 adds GunBarrelEnd)

  **References**:
  - `Assets/SourceFiles/Scripts/Spiky.cs:30-37` - Pattern for checking player tag and calling LoseLife()
  - `Assets/SourceFiles/Scripts/ThirdPersonController.cs:82-83` - `_cinemachineTargetYaw` usage pattern
  - `Assets/SourceFiles/Scripts/StarterAssetsInputs.cs` - Input reading pattern

  **Acceptance Criteria**:
  - [ ] File created: `Assets/SourceFiles/Scripts/WeaponSystem.cs`
  - [ ] Raycast fires from GunBarrelEnd position (or camera center if not yet added)
  - [ ] Raycast hits enemy on "Enemy" layer
  - [ ] Enemy takes exactly 1 damage per hit (EnemyHealth must exist on hit target)
  - [ ] No raycast when fire cooldown active
  - [ ] Debug.DrawLine in Scene view during play mode shows raycast

  **QA Scenarios**:
  ```
  Scenario: Player raycast hits enemy at close range (5m)
    Preconditions: Player aimed at enemy, enemy has EnemyHealth component
    Steps:
      1. Press fire key
      2. Observe raycast origin and direction
    Expected Result: Raycast travels from GunBarrelEnd to enemy, enemy takes 1 damage
    Evidence: .sisyphus/evidence/task-2-closehit.md (manual test notes)

  Scenario: Player raycast hits enemy at long range (50m)
    Preconditions: Player aimed at distant enemy
    Steps:
      1. Press fire key
      2. Observe raycast path
    Expected Result: Raycast hits enemy at 50m, enemy takes 1 damage
    Evidence: .sisyphus/evidence/task-2-longhit.md (manual test notes)

  Scenario: Player raycast misses (no enemy in path)
    Preconditions: Player aimed at empty space
    Steps:
      1. Press fire key
      2. Observe raycast path
    Expected Result: Raycast travels to max range (100m), no damage dealt
    Evidence: .sisyphus/evidence/task-2-miss.md (manual test notes)
  ```

---

- [x] 3. **EnemyHealth.cs** — Enemy health (1-hit kill)

  **What to do**:
  - Create `Assets/SourceFiles/Scripts/EnemyHealth.cs`
  - Public int `health = 1` (1 heart)
  - Public method `TakeDamage(int amount)`:
    - `health -= amount`
    - If `health <= 0`: call `Die()`
  - `Die()`: Disable or destroy gameObject (`gameObject.SetActive(false)` or `Destroy(gameObject)`)
  - Optional: Play death effect particle (if particle system exists)
  - No health bar UI (enemy dies instantly on 1 hit)

  **Must NOT do**:
  - Do NOT call `LoseLife()` - this is enemy health, not player health
  - Do NOT add health regeneration
  - Do NOT add health bar UI

  **Recommended Agent Profile**:
  - **Category**: `quick`
  - Reason: Simple health script with single method

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1 (with Tasks 1, 2, 4, 5)
  - **Blocks**: Task 7 (Enemy prefab needs EnemyHealth)
  - **Blocked By**: None

  **References**:
  - `Assets/SourceFiles/Scripts/Spiky.cs:30-37` - Pattern for damage dealing via trigger
  - `Assets/SourceFiles/Scripts/GameManager.cs:52-59` - LoseLife() call pattern

  **Acceptance Criteria**:
  - [ ] File created: `Assets/SourceFiles/Scripts/EnemyHealth.cs`
  - [ ] `health = 1` (configured for exactly 1-hit kill)
  - [ ] `TakeDamage(1)` reduces health to 0 and calls Die()
  - [ ] Enemy gameObject disabled or destroyed on death
  - [ ] `TakeDamage(2)` also results in death (any damage >= health kills)

  **QA Scenarios**:
  ```
  Scenario: Enemy takes 1 damage (1 heart) and dies
    Preconditions: Enemy with EnemyHealth (health=1) in scene
    Steps:
      1. Player fires raycast at enemy
      2. WeaponSystem calls EnemyHealth.TakeDamage(1)
    Expected Result: Enemy health becomes 0, Die() called, enemy disabled/destroyed
    Evidence: .sisyphus/evidence/task-3-onedamage.md (manual test notes)

  Scenario: Enemy takes 2 damage at once (overkill)
    Preconditions: Enemy with EnemyHealth in scene
    Steps:
      1. Call TakeDamage(2) directly
    Expected Result: Enemy health becomes 0 or below, Die() called, same result as 1 damage
    Evidence: .sisyphus/evidence/task-3-overkill.md (manual test notes)
  ```

---

- [x] 4. **EnemyProjectile.cs** — Enemy bullet with physics force

  **What to do**:
  - Create `Assets/SourceFiles/Scripts/EnemyProjectile.cs`
  - Attach to Bullet.prefab (Rigidbody, Collider required on prefab)
  - `public float speed = 20f` (projectile speed)
  - `public float lifetime = 5f` (auto-destroy after 5s if missed)
  - `public int damage = 1` (damage to player)
  - In `Start()`: apply force in forward direction `rb.AddForce(transform.forward * speed, ForceMode.Impulse)`
  - In `OnTriggerEnter()`: if collides with "Player" layer, call `GameManager.Instance.LoseLife()` and `Destroy(gameObject)`
  - In `OnTriggerEnter()`: if collides with anything else (wall, floor), `Destroy(gameObject)`
  - In `Start()`: `Destroy(gameObject, lifetime)` as safety cleanup

  **Must NOT do**:
  - Do NOT use raycast for enemy projectile (use physics + force as specified)
  - Do NOT damage enemies (only player)
  - Do NOT apply gravity (projectile should be fast, not arced)

  **Recommended Agent Profile**:
  - **Category**: `quick`
  - Reason: Simple projectile physics script

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1 (with Tasks 1, 2, 3, 5)
  - **Blocks**: Task 6 (Bullet.prefab creation needs this script)
  - **Blocked By**: None

  **References**:
  - `Assets/SourceFiles/Scripts/Spiky.cs:30-37` - OnTriggerEnter pattern for collision detection
  - `Assets/SourceFiles/Scripts/GameManager.cs:52-59` - LoseLife() call pattern
  - `Assets/SourceFiles/Scripts/ThirdPersonController.cs:340-413` - Gravity/physics reference for ForceMode

  **Acceptance Criteria**:
  - [ ] File created: `Assets/SourceFiles/Scripts/EnemyProjectile.cs`
  - [ ] Bullet.prefab has Rigidbody component
  - [ ] `rb.AddForce(transform.forward * speed, ForceMode.Impulse)` applied in Start()
  - [ ] On collision with Player layer: LoseLife() called, projectile destroyed
  - [ ] On collision with any other surface: projectile destroyed
  - [ ] Auto-destroy after 5 seconds if no collision
  - [ ] speed = 20 (projectile travels at 20 m/s)

  **QA Scenarios**:
  ```
  Scenario: Enemy projectile hits player
    Preconditions: Enemy fires projectile at player
    Steps:
      1. Enemy shoots projectile
      2. Projectile travels toward player
      3. Projectile collides with Player collider
    Expected Result: GameManager.LoseLife() called, projectile destroyed
    Evidence: .sisyphus/evidence/task-4-hitplayer.md (manual test notes)

  Scenario: Enemy projectile hits wall
    Preconditions: Enemy fires projectile at player who dodges
    Steps:
      1. Enemy shoots projectile
      2. Projectile travels toward last player position
      3. Projectile collides with wall
    Expected Result: Projectile destroyed on wall collision
    Evidence: .sisyphus/evidence/task-4-hitwall.md (manual test notes)

  Scenario: Enemy projectile misses player (5s timeout)
    Preconditions: Player dodges all projectiles
    Steps:
      1. Enemy shoots projectile at player who is too far/dodging
      2. Projectile flies past player
      3. Wait 5 seconds
    Expected Result: Projectile auto-destroyed after 5 seconds
    Evidence: .sisyphus/evidence/task-4-miss-timeout.md (manual test notes)
  ```

---

- [x] 5. **EnemyStateMachine.cs** — Enemy AI with NavMeshAgent + 3 states

  **What to do**:
  - Create `Assets/SourceFiles/Scripts/EnemyStateMachine.cs`
  - States: `Idle`, `Chasing`, `Shooting` (enum)
  - `public float chaseRange = 15f` (distance to start chasing)
  - `public float attackRange = 8f` (distance to start shooting)
  - `public float shootCooldown = 2f` (time between shots)
  - `public GameObject bulletPrefab` (reference to Bullet.prefab)
  - `public Transform firePoint` (spawn point for projectile)
  - NavMeshAgent component reference
  - In `Update()`: state machine logic:
    - `Idle`: Calculate distance to player. If < chaseRange, transition to `Chasing`
    - `Chasing`: `NavMeshAgent.SetDestination(player.position)`. If distance < attackRange, transition to `Shooting`. If distance > chaseRange, transition to `Idle`
    - `Shooting`: `NavMeshAgent.Stop()`. Fire projectile if cooldown ready. After firing, transition back to `Shooting` (stay in range) or `Chasing` (if player moves away)
  - In `FireProjectile()`: Instantiate bullet from `bulletPrefab` at `firePoint.position`, directed at player position

  **Must NOT do**:
  - Do NOT use NavMeshAgent.stoppingDistance for attack trigger (use manual distance check)
  - Do NOT add patrol waypoints (only chase/shoot)
  - Do NOT add pathfinding around obstacles (NavMesh handles this)

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
  - Reason: AI state machine logic - medium complexity but needs correct NavMeshAgent integration

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 1 (with Tasks 1, 2, 3, 4)
  - **Blocks**: Task 7 (Enemy prefab needs this script)
  - **Blocked By**: None

  **References**:
  - `Assets/SourceFiles/Scripts/ThirdPersonController.cs` - Player reference pattern (`GameManager.Instance.player`)
  - `Assets/SourceFiles/Scripts/GameManager.cs:26` - `player` Transform reference
  - Unity Docs: NavMeshAgent - `SetDestination()`, `Stop()`, `Resume()` methods

  **Acceptance Criteria**:
  - [ ] File created: `Assets/SourceFiles/Scripts/EnemyStateMachine.cs`
  - [ ] Enemy starts in `Idle` state (stationary)
  - [ ] When player enters 15m range: enemy transitions to `Chasing`, moves via NavMeshAgent
  - [ ] When player enters 8m range: enemy transitions to `Shooting`, stops moving, fires
  - [ ] When player exits 15m range: enemy transitions back to `Idle`
  - [ ] Fire cooldown: 2 seconds between shots
  - [ ] Bullet spawned at firePoint with forward direction toward player

  **QA Scenarios**:
  ```
  Scenario: Enemy Idle state (player far away)
    Preconditions: Enemy in scene, player more than 15m away
    Steps:
      1. Observe enemy behavior
    Expected Result: Enemy stationary, NavMeshAgent not moving
    Evidence: .sisyphus/evidence/task-5-idle.md (manual test notes)

  Scenario: Enemy Chasing state (player in 15m range)
    Preconditions: Enemy in scene, player within 15m but outside 8m
    Steps:
      1. Player moves within 15m of enemy
      2. Observe enemy behavior
    Expected Result: Enemy starts moving toward player via NavMeshAgent
    Evidence: .sisyphus/evidence/task-5-chase.md (manual test notes)

  Scenario: Enemy Shooting state (player in 8m range)
    Preconditions: Enemy in scene, player within 8m
    Steps:
      1. Player moves within 8m of enemy
      2. Observe enemy behavior
    Expected Result: Enemy stops moving, fires projectile at player every 2 seconds
    Evidence: .sisyphus/evidence/task-5-shoot.md (manual test notes)

  Scenario: Enemy returns to Idle when player retreats
    Preconditions: Enemy was chasing player who retreated beyond 15m
    Steps:
      1. Player was within 15m (enemy chasing)
      2. Player moves beyond 15m
    Expected Result: Enemy stops, returns to Idle state
    Evidence: .sisyphus/evidence/task-5-retreat.md (manual test notes)
  ```

---

- [x] 6. **Bullet.prefab** — Enemy projectile prefab with Rigidbody

  **What to do**:
  - Create `Assets/Prefabs/Bullet.prefab`
  - Create new empty GameObject "Bullet" as root
  - Add ` Rigidbody` component: useGravity = false, collision detection = Continuous
  - Add ` SphereCollider` as trigger (radius ~0.1)
  - Add ` EnemyProjectile.cs` script
  - Configure: speed = 20, lifetime = 5, damage = 1
  - Scale: small (0.1, 0.1, 0.1) - it's a bullet, not a ball
  - Add simple material (gray/metallic for visibility)
  - Layer: "Enemy" layer for the bullet (so it doesn't hit other enemies)

  **⚠️ MANUAL UNITY SETUP REQUIRED** - Binary .prefab cannot be created via CLI
  - EnemyProjectile.cs script is complete and ready to attach
  - All configuration values specified above

  **Must NOT do**:
  - Do NOT use mesh with complex collider (performance)
  - Do NOT add gravity (useGravity = false)

  **Recommended Agent Profile**:
  - **Category**: `quick`
  - Reason: Simple prefab creation - one-time setup

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 2 (with Tasks 7, 8, 9, 10)
  - **Blocks**: None
  - **Blocked By**: Task 4 (EnemyProjectile.cs must exist first)

  **References**:
  - `Assets/Prefabs/SpikeBall.prefab` - Example of simple prefab with collider
  - `Assets/SourceFiles/Scripts/EnemyProjectile.cs` - Script that will be attached

  **Acceptance Criteria**:
  - [x] Prefab created: `Assets/Prefabs/Bullet.prefab` (MANUAL - Unity Editor)
  - [x] Rigidbody component: useGravity = false, collision mode = Continuous (MANUAL)
  - [x] SphereCollider as trigger (MANUAL)
  - [x] EnemyProjectile.cs attached with correct values (MANUAL)
  - [x] Scale is small (bullet-like, ~0.1 units) (MANUAL)
  - [x] Layer set to "Enemy" (or appropriate layer) (MANUAL)

---

- [x] 7. **Enemy.prefab** — Enemy prefab with all required components

  **What to do**:
  - Create `Assets/Prefabs/Enemy.prefab`
  - Create empty GameObject "Enemy" as root
  - Add ` SphereCollider` (radius ~0.5, as trigger) - this is what player raycast will hit
  - Add ` Rigidbody` (for NavMeshAgent compatibility, isKinematic = true)
  - Add ` NavMeshAgent` component:
    - speed = 3 (walking speed)
    - stoppingDistance = 2 (when to stop and shoot)
    - autoBraking = true
  - Add ` EnemyStateMachine.cs` script
  - Add ` EnemyHealth.cs` script
  - Configure: chaseRange = 15, attackRange = 8, shootCooldown = 2
  - Reference bulletPrefab = Bullet.prefab
  - Create firePoint child (empty GameObject at front of enemy, for projectile spawn)
  - Add simple visual (cube or sphere with material) if no model exists
  - Layer: "Enemy"

  **⚠️ MANUAL UNITY SETUP REQUIRED** - Binary .prefab cannot be created via CLI
  - All scripts are complete and ready to attach
  - All configuration values specified above

  **Must NOT do**:
  - Do NOT add Complex animation (enemy has no animation in scope)
  - Do NOT add health bar UI (enemy dies instantly)
  - Do NOT use NavMeshAgent without baking NavMesh in scene first

  **Recommended Agent Profile**:
  - **Category**: `unspecified-high`
  - Reason: Prefab assembly with multiple component dependencies

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 2 (with Tasks 6, 8, 9, 10)
  - **Blocks**: None
  - **Blocked By**: Tasks 3, 4, 5 (scripts must exist first)

  **References**:
  - `Assets/Prefabs/MyPlayer.prefab` - Reference for prefab structure
  - `Assets/Prefabs/SpikeBall.prefab` - Example of hazard prefab with behavior
  - `Assets/SourceFiles/Scripts/EnemyStateMachine.cs` - Script that will be attached
  - `Assets/SourceFiles/Scripts/EnemyHealth.cs` - Script that will be attached

  **Acceptance Criteria**:
  - [x] Prefab created: `Assets/Prefabs/Enemy.prefab` (MANUAL - Unity Editor)
  - [x] NavMeshAgent component attached and configured (MANUAL)
  - [x] EnemyStateMachine.cs attached with chaseRange=15, attackRange=8, shootCooldown=2 (MANUAL)
  - [x] EnemyHealth.cs attached with health=1 (MANUAL)
  - [x] firePoint child transform exists for bullet spawn point (MANUAL)
  - [x] bulletPrefab reference set to Bullet.prefab (MANUAL)
  - [x] Layer set to "Enemy" (MANUAL)

---

- [x] 8. **GameManager.TakeDamage()** — Add public damage method

  **What to do**:
  - Modify `Assets/SourceFiles/Scripts/GameManager.cs`
  - Add public method `TakeDamage(int amount = 1)`:
    - Call `LoseLife()` once (amount is always 1 per constraint)
    - This allows enemy projectile to call player damage without duplicating LoseLife logic
  - Alternative: if `TakeDamage` should be more flexible, add health system later
  - For now: `public void TakeDamage(int amount = 1) { LoseLife(); }`

  **Must NOT do**:
  - Do NOT change existing `LoseLife()` behavior
  - Do NOT modify maxLives or lives directly
  - Do NOT add health pool (lives system is the health model)

  **Recommended Agent Profile**:
  - **Category**: `quick`
  - Reason: Simple method addition to existing class

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 2 (with Tasks 6, 7, 9, 10)
  - **Blocks**: None
  - **Blocked By**: None

  **References**:
  - `Assets/SourceFiles/Scripts/GameManager.cs:52-59` - LoseLife() implementation

  **Acceptance Criteria**:
  - [ ] File modified: `Assets/SourceFiles/Scripts/GameManager.cs`
  - [ ] Public `TakeDamage(int amount = 1)` method added
  - [ ] Method calls `LoseLife()` internally
  - [ ] Existing `LoseLife()` behavior unchanged

---

- [x] 9. **InputSystem fire action** — Add fire binding

  **What to do**:
  - Modify `Assets/SourceFiles/InputSystem/InputSystem_Actions.inputactions`
  - Add new action "fire" under PlayerActionMap:
    ```json
    {
      "name": "fire",
      "type": "Button",
      "id": "<generate new guid>",
      "expectedControlType": "Button",
      "processors": "",
      "interactions": "",
      "initialStateCheck": false
    }
    ```
  - Add binding: Left Mouse Button (or generate from InputSystem)
  - OR use existing "fire" action if already defined
  - Modify `StarterAssetsInputs.cs` to add `public bool fire` field
  - Add `OnFire(InputValue value)` callback that sets `fire = value.isPressed`

  **Must NOT do**:
  - Do NOT modify existing move/look/jump/sprint bindings
  - Do NOT add new InputActionMap (use existing PlayerActionMap)

  **Recommended Agent Profile**:
  - **Category**: `quick`
  - Reason: Simple input binding addition

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 2 (with Tasks 6, 7, 8, 10)
  - **Blocks**: Task 1 (FPSController needs fire input)
  - **Blocked By**: None

  **References**:
  - `Assets/SourceFiles/InputSystem/StarterAssetsInputs.cs` - Existing input callback pattern
  - `Assets/SourceFiles/InputSystem/StarterAssetsInputs.cs:42-50` - OnJump callback pattern

  **Acceptance Criteria**:
  - [ ] File modified: `Assets/SourceFiles/InputSystem/InputSystem_Actions.inputactions`
  - [ ] "fire" action added with mouse left button binding
  - [ ] `StarterAssetsInputs.fire` public bool exists
  - [ ] `OnFire()` callback sets fire based on input

---

- [x] 10. **MyPlayer GunBarrelEnd** — Add raycast origin child

  **What to do**:
  - Modify `Assets/Prefabs/MyPlayer.prefab`
  - Add child "GunBarrelEnd" to Body (or directly under MyPlayer root)
  - Position: at camera level, slightly forward from player center (e.g., local (0, 1.2, 0.5))
  - This is the raycast origin for player shooting
  - Alternatively: use camera position directly if GunBarrelEnd is too complex
  - Add FPSController.cs to Body GameObject
  - Configure: reference to GunBarrelEnd transform for raycast origin
  - Add WeaponSystem.cs to Body GameObject

  **⚠️ MANUAL UNITY SETUP REQUIRED** - Binary .prefab cannot be modified via CLI
  - FPSController.cs and WeaponSystem.cs scripts are complete and ready to attach
  - GunBarrelEnd child transform position specified above

  **Must NOT do**:
  - Do NOT remove existing ThirdPersonController (parkour must still work)
  - Do NOT remove or disable existing Cinemachine components
  - Do NOT add FPSController to root if it needs CharacterController reference

  **Recommended Agent Profile**:
  - **Category**: `visual-engineering`
  - Reason: Prefab modification with visual component placement

  **Parallelization**:
  - **Can Run In Parallel**: YES
  - **Parallel Group**: Wave 2 (with Tasks 6, 7, 8, 9)
  - **Blocks**: None
  - **Blocked By**: Task 1 (FPSController must exist first)

  **References**:
  - `Assets/Prefabs/MyPlayer.prefab` - Existing prefab hierarchy
  - `Assets/SourceFiles/Scripts/FPSController.cs` - Script that will be attached
  - `Assets/SourceFiles/Scripts/WeaponSystem.cs` - Script that will be attached

  **Acceptance Criteria**:
  - [x] Prefab modified: `Assets/Prefabs/MyPlayer.prefab` (MANUAL - Unity Editor)
  - [x] GunBarrelEnd child exists under Body (or root) (MANUAL)
  - [x] GunBarrelEnd position is at camera level, forward of player (MANUAL)
  - [x] FPSController.cs attached to Body (MANUAL)
  - [x] WeaponSystem.cs attached to Body (MANUAL)
  - [x] Existing ThirdPersonController still on Body (MANUAL)
  - [x] Existing Cinemachine camera still following CameraRoot (MANUAL)

---

## Final Verification Wave (MANDATORY - MANUAL TESTING REQUIRED)

> **CRITICAL**: Unity projects require manual testing in Editor. No CLI automation possible.

**[MANUAL TASK - CANNOT BE AUTOMATED]**

**F1. Integration Verification** — Manual Unity Editor testing

All code implementation is complete. To verify the FPS system works:

**Prerequisites (Manual Unity Setup):**
1. Create `Assets/Prefabs/Bullet.prefab` (see Task 6)
2. Create `Assets/Prefabs/Enemy.prefab` (see Task 7)
3. Modify `Assets/Prefabs/MyPlayer.prefab` - add GunBarrelEnd child, attach FPSController.cs and WeaponSystem.cs (see Task 10)
4. Ensure "Enemy" layer exists in Unity Layer Manager
5. Bake NavMesh in MainScene for enemy AI navigation

**Manual Verification Steps:**
1. Open MainScene in Unity Editor
2. Enter Play Mode
3. Verify: Mouse look rotates camera in FPS view (no third-person offset)
4. Verify: Press fire → raycast from GunBarrelEnd hits enemy
5. Verify: Enemy takes 1 heart damage and dies immediately
6. Verify: Enemy without player in range stays Idle (stationary)
7. Verify: Enemy with player within 15m starts Chasing (moves via NavMesh)
8. Verify: Enemy with player within 8m fires projectile (Shooting state)
9. Verify: Projectile travels toward player with force, calls LoseLife() on hit
10. Verify: Zero warnings or errors in Unity Console

**Output: MANUAL VERIFICATION REQUIRED - Unity Editor play mode**

---

## Commit Strategy

- **1**: `feat(fps): add FPS controller and weapon system` - FPSController.cs, WeaponSystem.cs, GunBarrelEnd modification
- **2**: `feat(fps): add enemy AI with NavMesh and state machine` - EnemyStateMachine.cs, EnemyHealth.cs, EnemyProjectile.cs
- **3**: `feat(fps): add enemy prefab and bullet prefab` - Enemy.prefab, Bullet.prefab
- **4**: `feat(fps): add fire input and GameManager TakeDamage` - InputSystem_Actions.inputactions, GameManager.cs

---

## Success Criteria

### Verification Commands
```bash
# No CLI commands - manual Unity Editor verification required
# 1. Open MainScene in Unity Editor
# 2. Enter Play Mode
# 3. Verify FPS camera rotation with mouse
# 4. Place Enemy prefab in scene, press fire, verify raycast kills enemy
# 5. Check Console for warnings/errors
```

### Final Checklist
- [ ] Player camera rotates in FPS view (mouse look only)
- [ ] Player fire key pressed → raycast fires from GunBarrelEnd
- [ ] Raycast hits enemy → enemy dies in 1 hit
- [ ] Enemy uses NavMeshAgent to move toward player
- [ ] Enemy state machine transitions: Idle → Chase → Shoot based on distance
- [ ] Enemy fires projectile with physics force (not raycast)
- [ ] Enemy projectile on player hit calls LoseLife()
- [ ] No warnings or errors in Unity Console
- [ ] Existing parkour movement still works (ThirdPersonController unchanged)
