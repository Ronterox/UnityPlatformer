using UnityEngine;
using StarterAssets;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class WeaponSystem : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform GunBarrelEnd;
    public float Range = 100f;
    public LayerMask EnemyLayerMask;
    public float FireCooldown = 0.2f;

    [Header("Input Reference")]
    public StarterAssetsInputs Input;

    private float _fireCooldownTimer;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        if (GunBarrelEnd == null)
        {
            GunBarrelEnd = transform.Find("GunBarrelEnd");
        }
    }

    private void Update()
    {
        if (_fireCooldownTimer > 0f)
        {
            _fireCooldownTimer -= Time.deltaTime;
        }

        if (Input != null && Input.attack && _fireCooldownTimer <= 0f)
        {
            Fire();
            _fireCooldownTimer = FireCooldown;
        }
    }

    private void Fire()
    {
        Vector3 rayOrigin = GunBarrelEnd != null
            ? GunBarrelEnd.position
            : _mainCamera.transform.position;

        Vector3 rayDirection = _mainCamera.transform.forward;

        Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * Range, Color.red, 0.1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Range, EnemyLayerMask))
        {
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1);
            }
        }
    }
}