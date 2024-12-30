using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;

		public bool jump;
		public bool sprint;
		public bool aim;
		public bool cruch;
		public bool shooting;
		public bool reloading;

		public bool cameraSide = true;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}

		public void OnCruch(InputValue value)
		{
			CruchInput(value.isPressed);
		}

		public void OnShoot(InputValue value)
    {
			ShootInput(value.isPressed);
    }
		
		public void OnChangeCameraSide()
    {
			ChangeCameraSide(!cameraSide);
    }

		public void OnReloading(InputValue value)
    {
			ReloadingInput(value.isPressed);
    }

#endif
    public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

		private void CruchInput(bool newCruchInput)
    {
			cruch = newCruchInput;
    }
		private void ShootInput(bool newShootingState)
		{
			shooting = newShootingState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
		private void ChangeCameraSide(bool unversdCameraSide)
		{
			cameraSide = unversdCameraSide;
		}

		private void ReloadingInput(bool newReloadingState)
    	{
			reloading = newReloadingState;
		}
	}
}