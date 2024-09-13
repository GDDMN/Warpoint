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

        private PlayerStateController _stateController = new PlayerStateController();
        private PlayerState _activeState;
        
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        public event Action<bool> OnLanding;
        
        private void Awake()
        {
            if (_mainCamera == null)
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            
            _stateController.Initialize();
            _activeState = _stateController.GetState(PlayerStateType.ALIVE);
            
            #if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                        _playerInput = GetComponent<PlayerInput>();
            #else
            			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
            #endif

            _activeState.Enter(_actorComponent, _cinemachineData, _controller, _input, _mainCamera, _playerInput);
            _actorComponent.AssignAnimationIDs();
        }

        private void Update()
        {
            if(!_actorComponent.IsAlive)
            {
              return;     
            }

            _activeState.Update();
        } 

        private void LateUpdate()
        {
          _activeState.LateUpdate();
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

        //private void OnFootstep(AnimationEvent animationEvent)
        //{
        //    if (animationEvent.animatorClipInfo.weight > 0.5f)
        //    {
        //        if (_actorComponent.ActorData.FootstepAudioClips.Length > 0)
        //        {
        //            var index = UnityEngine.Random.Range(0, _actorComponent.ActorData.FootstepAudioClips.Length);
        //            AudioSource.PlayClipAtPoint(_actorComponent.ActorData.FootstepAudioClips[index], transform.TransformPoint(_controller.center),
        //            _actorComponent.ActorData.FootstepAudioVolume);
        //        }
        //    }
        //}

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
