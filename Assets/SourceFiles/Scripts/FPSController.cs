using UnityEngine;
using StarterAssets;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class FPSController : MonoBehaviour
    {
        public float LookSensitivity = 7.5f;
        public float BottomClamp = -70f;
        public float TopClamp = 70f;
        public float FireRate = 0.5f;
        public float RaycastDistance = 100f;

        public Transform CameraRoot;
        public StarterAssetsInputs _input;

        private float _cameraPitch;
        private float _cameraYaw;
        private float _lastFireTime;

        private static RaycastHit _hit;

        private void Awake()
        {
            if (_input == null)
            {
                _input = GetComponent<StarterAssetsInputs>();
            }

            if (CameraRoot != null)
            {
                _cameraPitch = CameraRoot.localEulerAngles.x;
                if (_cameraPitch > 180f) _cameraPitch -= 360f;
                _cameraYaw = CameraRoot.localEulerAngles.y;
            }
        }

        private void Update()
        {
            HandleCameraRotation();
            HandleFiring();
        }

        private void HandleCameraRotation()
        {
            if (_input == null || CameraRoot == null) return;

            float mouseSensitivity = LookSensitivity;

            _cameraYaw += _input.look.x * mouseSensitivity * Time.deltaTime;
            _cameraPitch += _input.look.y * mouseSensitivity * Time.deltaTime;

            _cameraPitch = ClampAngle(_cameraPitch, BottomClamp, TopClamp);

            CameraRoot.localRotation = Quaternion.Euler(_cameraPitch, _cameraYaw, 0f);
        }

        private void HandleFiring()
        {
            if (_input == null) return;

            if (_input.fire && Time.time >= _lastFireTime + FireRate)
            {
                FireRaycast();
                _lastFireTime = Time.time;
            }
        }

        private void FireRaycast()
        {
            if (CameraRoot == null) return;

            Vector3 rayOrigin = CameraRoot.position;
            Vector3 rayDirection = CameraRoot.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out _hit, RaycastDistance))
            {
                Debug.Log($"FPS Raycast hit: {_hit.collider.name} at {_hit.point}");
            }
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }
    }
}