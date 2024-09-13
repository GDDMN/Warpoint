using UnityEngine;
using System;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [SerializeField] private ActorComponent _actorComponent;
        [SerializeField] private CinemachineData _cinemachineData;

        private PlayerStateController _stateController;
        private PlayerState _activeState;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        public event Action<bool> OnLanding;
        
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = _cinemachineData.CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            

            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _stateController.Initialize();
            _activeState = _stateController.GetState(PlayerStateType.ALIVE);
            
            #if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                        _playerInput = GetComponent<PlayerInput>();
            #else
            			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
            #endif

            _actorComponent.AssignAnimationIDs();

            // reset our timeouts on start

        }

        private void Update()
        {
            //_hasAnimator = TryGetComponent(out _animator);
            
            if(!_actorComponent.IsAlive)
            {
              return;     
            }

            _actorComponent.JumpAndGravity(_input);
            _actorComponent.GroundedCheck();
            _actorComponent.Move(_input, _cinemachineData, _controller, _mainCamera);
            _actorComponent.Aiming(_input, _cinemachineData);
            _actorComponent.Cruch(_input);
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !_cinemachineData.LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * _cinemachineData.Sensativity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * _cinemachineData.Sensativity;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _cinemachineData.BottomClamp, _cinemachineData.TopClamp);

            // Cinemachine will follow this target
            _cinemachineData.CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cinemachineData.CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }





        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (_actorComponent.ActorData.Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - _actorComponent.ActorData.GroundedOffset, transform.position.z),
                _actorComponent.ActorData.GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (_actorComponent.ActorData.FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, _actorComponent.ActorData.FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(_actorComponent.ActorData.FootstepAudioClips[index], transform.TransformPoint(_controller.center),
                    _actorComponent.ActorData.FootstepAudioVolume);
                }
            }
        }

        public void OnGround()
        {
            OnLanding?.Invoke(true);
        }

        public void SetSensativity(float newSensativity)
        {
          _cinemachineData.Sensativity = newSensativity;
        }
  }
}
