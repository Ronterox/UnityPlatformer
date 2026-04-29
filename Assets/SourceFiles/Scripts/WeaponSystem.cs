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

    [Header("Laser Visual")]
    public LineRenderer LaserLine;
    public float LaserDuration = 0.15f;
    public Color LaserColor = Color.red;
    public float LaserWidth = 0.05f;

    [Header("Input Reference")]
    public StarterAssetsInputs Input;

    [Header("Audio")]
    public AudioClip ShootSound;
    private AudioSource _audioSource;

    private float _fireCooldownTimer;
    private Camera _mainCamera;
    private float _laserTimer;
    private Vector3 _laserEndPoint;
    private bool _hasFiredThisPress;
    private bool _firedThisPress;

    private void Awake()
    {
        _mainCamera = Camera.main;
        if (GunBarrelEnd == null)
        {
            GunBarrelEnd = transform.Find("GunBarrelEnd");
        }

        if (Input == null)
        {
            Input = GetComponent<StarterAssetsInputs>();
        }

        if (LaserLine == null)
        {
            GameObject laserObj = new GameObject("LaserLine");
            laserObj.transform.SetParent(transform);
            LaserLine = laserObj.AddComponent<LineRenderer>();
            LaserLine.startWidth = 0.05f;
            LaserLine.endWidth = 0.05f;
            LaserLine.startColor = Color.red;
            LaserLine.endColor = Color.yellow;
            LaserLine.useWorldSpace = true;
            LaserLine.enabled = false;
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

private void Update()
    {
        if (_fireCooldownTimer > 0f)
        {
            _fireCooldownTimer -= Time.deltaTime;
        }

        if (_laserTimer > 0f)
        {
            _laserTimer -= Time.deltaTime;
            LaserLine.enabled = true;
            LaserLine.SetPosition(0, GunBarrelEnd != null ? GunBarrelEnd.position : _mainCamera.transform.position);
            LaserLine.SetPosition(1, _laserEndPoint);
        }
        else
        {
            LaserLine.enabled = false;
        }

        if (_fireCooldownTimer <= 0f)
        {
            Fire();
            _fireCooldownTimer = FireCooldown;
            Debug.Log("[WeaponSystem] >>> FIRE! <<<");
        }
    }

    private void Fire()
    {
        Vector3 rayOrigin = GunBarrelEnd != null
            ? GunBarrelEnd.position
            : _mainCamera.transform.position;

        Vector3 rayDirection = _mainCamera.transform.forward;
        Vector3 rayEnd = rayOrigin + rayDirection * Range;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Range, EnemyLayerMask))
        {
            rayEnd = hit.point;
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1);
            }
        }

        _laserEndPoint = rayEnd;
        _laserTimer = LaserDuration;

        if (ShootSound != null)
        {
            _audioSource.PlayOneShot(ShootSound);
        }
    }

    private void OnDrawGizmos()
    {
        if (_mainCamera == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * 5f);
    }
}