using UnityEngine;
using System;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    //[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        //public CinemachineData CinemachineData;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private const float _threshold = 0.01f;

        [SerializeField] private ActorComponent _actorComponent;
        [SerializeField] private CinemachineData _cinemachineData;
        [SerializeField] private Transform aimObjectPos;
        [SerializeField] private CharacterController _controller;

        private PlayerStateController _stateController = new PlayerStateController();
        private PlayerState _activeState;
        
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;    

        public event Action<bool> OnLanding;

        public ActorComponent ActorComponent => _actorComponent;
        
        private void Awake()
        {
            if (_mainCamera == null)
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        
        public void Initialize()
        {
            InitializeInput();
            AssingAnimationsID();
            InitializeStateController();
        }

        private void InitializeStateController()
        {
            _stateController.Initialize(_actorComponent);
            _activeState = _stateController.GetState(PlayerStateType.ALIVE);
            _actorComponent.OnDeath += properties => { StartNewState(PlayerStateType.DEAD); };

            StartNewState(PlayerStateType.ALIVE);
        }  

        private void InitializeInput()
        {
            _input = GetComponent<StarterAssetsInputs>();
            
            #if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                _playerInput = GetComponent<PlayerInput>();
            #else
            	Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
            #endif
        }

        private void AssingAnimationsID()
        {
            _actorComponent.AssignAnimationIDs();
        }

        private void StartNewState(PlayerStateType type)
        {
            _activeState = _stateController.GetState(type);
            _activeState.Enter(_controller, _mainCamera);
        }

        private void Update()
        {
            SetInputValues();
            _activeState.Update();
        } 
        
        private void LateUpdate()
        {
            CameraRotation(_input.look);
        }

        private void SetInputValues()
        {
            if(_activeState is IValidatorsSetter)
            {
                ((IValidatorsSetter)_activeState).SetValidatorsValue(_input.aim, _input.shooting, _input.sprint,
                                                                    _input.cruch, _input.reloading, _input.move, 
                                                                    _input.analogMovement, _input.jump, aimObjectPos.position);
            }
        }


        private void CameraRotation(Vector2 look)
        {
            // if there is an input and camera position is not fixed
            if (look.sqrMagnitude >= _threshold && !_cinemachineData.LockCameraPosition)
            {
              //Don't multiply mouse input by Time.deltaTime;
              //float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

              _cinemachineTargetYaw += look.x * 1.0f * _cinemachineData.Sensativity;
              _cinemachineTargetPitch += look.y * 1.0f * _cinemachineData.Sensativity;
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
