using System;
using UnityEngine;

[Serializable]
public struct ActorData
{
  [Header("Player")]
  [Tooltip("Move speed of the character in m/s //Default is 2.0f")]
  public float MoveSpeed; //Default is 2.0f;

  [Tooltip("Sprint speed of the character in m/s //Default is 5.335f")]
  public float SprintSpeed; //Default is 5.335f;

  public float CruchSpeed; //Default is 2.0f;

  [Tooltip("Aiming move speed ofthe character //Default is 2.0f")]
  public float AimSpeed; //Default is 2.0f;

  [Tooltip("How fast the character turns to face movement direction //Default is 0.12f")]
  [Range(0.0f, 0.3f)]
  public float RotationSmoothTime; //Default is 0.12f;

  [Tooltip("Acceleration and deceleration //Default is 10.f")]
  public float SpeedChangeRate;  //Default is 10.f;

  public AudioClip LandingAudioClip;
  public AudioClip[] FootstepAudioClips;

  [Tooltip("//Default is 0.5f")]
  [Range(0, 1)] public float FootstepAudioVolume; //Default is 0.5f;

  [Space(10)]
  [Tooltip("The height the player can jump //Default is 1.2f")]
  public float JumpHeight; //Default is 1.2f

  [Tooltip("The character uses its own gravity value. The engine default is -9.81f //Default is -15.0f")]
  public float Gravity; //Default is -15.0f

  [Space(10)]
  [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again //Default is 0.50f")]
  public float JumpTimeout; //Default is 0.50f;

  [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs //Default is 0.15f")]
  public float FallTimeout; //Default is 0.15f;

  [Header("Player Grounded")]
  [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check //Default is true")]
  public bool Grounded; //Default is true;

  [Tooltip("Useful for rough ground //Default is -0.14f")]
  public float GroundedOffset; //Default is -0.14f;

  [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController //Default is 0.28f")]
  public float GroundedRadius; //Default is 0.28f;

  [Tooltip("What layers the character uses as ground")]
  public LayerMask GroundLayers;
}

