using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SinglePlayerController))]
public class SinglePlayerEditor : Editor
{
	protected static bool m_bShowFunctions = true;
	protected static bool m_bShowKeyBindings = true;
	protected static bool m_bShowCameraSettings = true;
	protected static bool m_bShowStanceSettings = true;
	protected static bool m_bShowGroundMovement = true;
	protected static bool m_bShowAirMovement = true;
	protected static bool m_bShowSlideSettings = true;
	protected static bool m_bShowSlopeSettings = true;
	protected static bool m_bShowLeanSettings = true;
	protected static bool m_bAnimatorSettings = true;


	SerializedProperty m_bCanSprint;
	SerializedProperty m_bCanJump;
	SerializedProperty m_bCanCrouch;
	SerializedProperty m_bCanProne;
	SerializedProperty m_bCanSlide;
	SerializedProperty m_bWillSlideOnSlopes;
	SerializedProperty m_bCanStrafe;
	SerializedProperty m_bCanLean;

	SerializedProperty m_bHoldToJump;
	SerializedProperty m_jump;
	SerializedProperty m_bHoldToSprint;
	SerializedProperty m_sprint;
	SerializedProperty m_bHoldToCrouch;
	SerializedProperty m_crouch;
	SerializedProperty m_bHoldToProne;
	SerializedProperty m_prone;
	SerializedProperty m_bHoldToLeanLeft;
	SerializedProperty m_leanLeft;
	SerializedProperty m_bHoldToLeanRight;
	SerializedProperty m_leanRight;

	SerializedProperty m_cameraTransform;
	SerializedProperty m_fCameraChangeStanceSpeed;
	SerializedProperty m_fStandCameraYOffset;
	SerializedProperty m_fFieldOfView;
	SerializedProperty m_fXMouseSensitivity;
	SerializedProperty m_fYMouseSensitivity;
	SerializedProperty m_playerCamera;

	SerializedProperty m_fCrouchCameraYOffset;
	SerializedProperty m_crouchHeight;
	SerializedProperty m_fProneCameraYOffset;
	SerializedProperty m_proneHeight;

	SerializedProperty m_fGravity;
	SerializedProperty m_fFriction;
	SerializedProperty m_bSprintToWalk;
	SerializedProperty m_fWalkSpeed;
	SerializedProperty m_fRunSpeed;
	SerializedProperty m_fCrouchSpeed;
	SerializedProperty m_fProneSpeed;
	SerializedProperty m_fGroundAcceleration;
	SerializedProperty m_fGroundDeceleration;

	SerializedProperty m_fLeanAmount;
	SerializedProperty m_playerHitBox;

	SerializedProperty m_fAirControl;
	SerializedProperty m_fJumpFriction;
	SerializedProperty m_fAirAcceleration;
	SerializedProperty m_fAirDeceleration;
	SerializedProperty m_fSideStrafeAcceleration;
	SerializedProperty m_fSideStrafeSpeed;

	SerializedProperty m_fSlideSpeed;
	SerializedProperty m_fSpeedToStartSlide;
	SerializedProperty m_fSlideSlopeSpeed;
	SerializedProperty m_fSlideSlopeLimit;
	SerializedProperty m_fSlideAcceleration;
	SerializedProperty m_fSlideDeceleration;

	SerializedProperty m_fSlopeSpeed;
	SerializedProperty m_fSlopeForce;
	SerializedProperty m_fSlopeRayLength;

	SerializedProperty m_animator;
	SerializedProperty m_playerSkinnedMeshRendererObj;

	protected virtual void OnEnable()
	{
		m_bCanSprint = this.serializedObject.FindProperty("m_bCanSprint");
		m_bCanJump = this.serializedObject.FindProperty("m_bCanJump");
		m_bCanCrouch = this.serializedObject.FindProperty("m_bCanCrouch");
		m_bCanProne = this.serializedObject.FindProperty("m_bCanProne");
		m_bCanSlide = this.serializedObject.FindProperty("m_bCanSlide");
		m_bWillSlideOnSlopes = this.serializedObject.FindProperty("m_bWillSlideOnSlopes");
		m_bCanStrafe = this.serializedObject.FindProperty("m_bCanStrafe");
		m_bCanLean = this.serializedObject.FindProperty("m_bCanLean");

		m_bHoldToJump = this.serializedObject.FindProperty("m_bHoldToJump");
		m_jump = this.serializedObject.FindProperty("m_jump");
		m_bHoldToSprint = this.serializedObject.FindProperty("m_bHoldToSprint");
		m_sprint = this.serializedObject.FindProperty("m_sprint");
		m_bHoldToCrouch = this.serializedObject.FindProperty("m_bHoldToCrouch");
		m_crouch = this.serializedObject.FindProperty("m_crouch");
		m_bHoldToProne = this.serializedObject.FindProperty("m_bHoldToProne");
		m_prone = this.serializedObject.FindProperty("m_prone");
		m_bHoldToLeanLeft = this.serializedObject.FindProperty("m_bHoldToLeanLeft");
		m_leanLeft = this.serializedObject.FindProperty("m_leanLeft");
		m_bHoldToLeanRight = this.serializedObject.FindProperty("m_bHoldToLeanRight");
		m_leanRight = this.serializedObject.FindProperty("m_leanRight");

		//m_cameraTransform = this.serializedObject.FindProperty("m_cameraTransform");
		m_playerCamera = this.serializedObject.FindProperty("m_playerCamera");
		m_fCameraChangeStanceSpeed = this.serializedObject.FindProperty("m_fCameraChangeStanceSpeed");
		m_fStandCameraYOffset = this.serializedObject.FindProperty("m_fStandCameraYOffset");
		m_fFieldOfView = this.serializedObject.FindProperty("m_fFieldOfView");
		m_fXMouseSensitivity = this.serializedObject.FindProperty("m_fXMouseSensitivity");
		m_fYMouseSensitivity = this.serializedObject.FindProperty("m_fYMouseSensitivity");

		m_fCrouchCameraYOffset = this.serializedObject.FindProperty("m_fCrouchCameraYOffset");
		m_crouchHeight = this.serializedObject.FindProperty("m_crouchHeight");
		m_fProneCameraYOffset = this.serializedObject.FindProperty("m_fProneCameraYOffset");
		m_proneHeight = this.serializedObject.FindProperty("m_proneHeight");

		m_fGravity = this.serializedObject.FindProperty("m_fGravity");
		m_fFriction = this.serializedObject.FindProperty("m_fFriction");
		m_bSprintToWalk = this.serializedObject.FindProperty("m_bSprintToWalk");
		m_fWalkSpeed = this.serializedObject.FindProperty("m_fWalkSpeed");
		m_fRunSpeed = this.serializedObject.FindProperty("m_fRunSpeed");
		m_fCrouchSpeed = this.serializedObject.FindProperty("m_fCrouchSpeed");
		m_fProneSpeed = this.serializedObject.FindProperty("m_fProneSpeed");
		m_fGroundAcceleration = this.serializedObject.FindProperty("m_fGroundAcceleration");
		m_fGroundDeceleration = this.serializedObject.FindProperty("m_fGroundDeceleration");

		m_fLeanAmount = this.serializedObject.FindProperty("m_fLeanAmount");
		m_playerHitBox = this.serializedObject.FindProperty("m_playerHitBox");

		m_fAirControl = this.serializedObject.FindProperty("m_fAirControl");
		m_fJumpFriction = this.serializedObject.FindProperty("m_fJumpFriction");
		m_fAirAcceleration = this.serializedObject.FindProperty("m_fAirAcceleration");
		m_fAirDeceleration = this.serializedObject.FindProperty("m_fAirDeceleration");
		m_fSideStrafeAcceleration = this.serializedObject.FindProperty("m_fSideStrafeAcceleration");
		m_fSideStrafeSpeed = this.serializedObject.FindProperty("m_fSideStrafeSpeed");

		m_fSlideSpeed = this.serializedObject.FindProperty("m_fSlideSpeed");
		m_fSpeedToStartSlide = this.serializedObject.FindProperty("m_fSpeedToStartSlide");
		m_fSlideSlopeSpeed = this.serializedObject.FindProperty("m_fSlideSlopeSpeed");
		m_fSlideSlopeLimit = this.serializedObject.FindProperty("m_fSlideSlopeLimit");
		m_fSlideAcceleration = this.serializedObject.FindProperty("m_fSlideAcceleration");
		m_fSlideDeceleration = this.serializedObject.FindProperty("m_fSlideDeceleration");

		m_fSlopeSpeed = this.serializedObject.FindProperty("m_fSlopeSpeed");
		m_fSlopeForce = this.serializedObject.FindProperty("m_fSlopeForce");
		m_fSlopeRayLength = this.serializedObject.FindProperty("m_fSlopeRayLength");

		m_animator = this.serializedObject.FindProperty("m_animator");
		m_playerSkinnedMeshRendererObj = this.serializedObject.FindProperty("m_playerSkinnedMeshRendererObj");
	}
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		var _playerScript = target as SinglePlayerController;
		EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
		m_bShowFunctions = EditorGUILayout.Foldout(m_bShowFunctions, "Enable Functions");
		if (m_bShowFunctions)
		{
			m_bCanSprint.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Sprint", "Allows the player to sprint and enables sprint variables"), _playerScript.m_bCanSprint);
			m_bCanJump.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Jump", "Allows the player to jump and enables jump variables"), _playerScript.m_bCanJump);
			m_bCanCrouch.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Crouch", "Allows the player to crouch and enables crouch variables"), _playerScript.m_bCanCrouch);
			m_bCanProne.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Prone", "Allows the player to prone and enables prone variables"), _playerScript.m_bCanProne);
			m_bCanSlide.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Slide", "Allows the player to slide and enables slide variables"), _playerScript.m_bCanSlide);
			m_bWillSlideOnSlopes.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Sliding Down Slopes", "Makes the player slide down a slope over the character controller slope limit"), _playerScript.m_bWillSlideOnSlopes);
			m_bCanStrafe.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Strafing", "Allows the player to strafe and enables strafe variables"), m_bCanStrafe.boolValue);
			m_bCanLean.boolValue = EditorGUILayout.Toggle(new GUIContent("Enable Leaning", "Allows the player to Lean and enables strafe variables"), m_bCanLean.boolValue);
		}
		//EditorGUILayout.Space();
		EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
		m_bShowKeyBindings = EditorGUILayout.Foldout(m_bShowKeyBindings, "Key Bindings");
		if (m_bShowKeyBindings)
		{
			m_bHoldToJump.boolValue = EditorGUILayout.Toggle(new GUIContent("Hold to Jump", "Used to change if jump should be pressed or held down"), _playerScript.m_bHoldToJump);
			EditorGUILayout.PropertyField(m_jump);
			m_bHoldToSprint.boolValue = EditorGUILayout.Toggle(new GUIContent("Hold to Sprint", "Used to change if sprint should be pressed or held down"), _playerScript.m_bHoldToSprint);
			EditorGUILayout.PropertyField(m_sprint);
			m_bHoldToCrouch.boolValue = EditorGUILayout.Toggle(new GUIContent("Hold to Crouch", "Used to change if crouch should be pressed or held down"), _playerScript.m_bHoldToCrouch);
			EditorGUILayout.PropertyField(m_crouch);
			m_bHoldToProne.boolValue = EditorGUILayout.Toggle(new GUIContent("Hold to Prone", "Used to change if prone should be pressed or held down"), _playerScript.m_bHoldToProne);
			EditorGUILayout.PropertyField(m_prone);
			m_bHoldToLeanLeft.boolValue = EditorGUILayout.Toggle(new GUIContent("Hold to Lean", "Used to change if Lean should be pressed or held down"), _playerScript.m_bHoldToLeanLeft);
			EditorGUILayout.PropertyField(m_leanLeft);
			m_bHoldToLeanRight.boolValue = EditorGUILayout.Toggle(new GUIContent("Hold to Lean", "Used to change if Lean should be pressed or held down"), _playerScript.m_bHoldToLeanRight);
			EditorGUILayout.PropertyField(m_leanRight);
		}

		EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
		m_bShowCameraSettings = EditorGUILayout.Foldout(m_bShowCameraSettings, "Camera Settings");
		if (m_bShowCameraSettings)
		{
			//m_cameraTransform.objectReferenceValue = EditorGUILayout.ObjectField("Camera Transform", _playerScript.m_cameraTransform, typeof(Transform), true) as Transform;
			m_playerCamera.objectReferenceValue = EditorGUILayout.ObjectField("Player Camera", _playerScript.m_playerCamera, typeof(Camera), true) as Camera;
			m_fCameraChangeStanceSpeed.floatValue = EditorGUILayout.FloatField(new GUIContent("Camera Stance Speed", "How fast the camera lerps between crouch, prone, etc"), _playerScript.m_fCameraChangeStanceSpeed);
			m_fStandCameraYOffset.floatValue = EditorGUILayout.FloatField(new GUIContent("Camera Standing Y Offset", "Used to move the camera to the desired head position"), _playerScript.m_fStandCameraYOffset);
			m_fFieldOfView.floatValue = EditorGUILayout.Slider(new GUIContent("Field Of View", "Changes the field of view"), _playerScript.m_fFieldOfView, 60.0f, 110.0f);
			m_fXMouseSensitivity.floatValue = EditorGUILayout.FloatField(new GUIContent("Mouse X Sensitivity", "Used change to mouse X sensitivity"), _playerScript.m_fXMouseSensitivity);
			m_fYMouseSensitivity.floatValue = EditorGUILayout.FloatField(new GUIContent("Mouse Y Sensitivity", "Used change to mouse Y sensitivity"), _playerScript.m_fYMouseSensitivity);
		}

		if (_playerScript.m_bCanCrouch || _playerScript.m_bCanProne)
		{
			EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
			m_bShowStanceSettings = EditorGUILayout.Foldout(m_bShowStanceSettings, "Stance Settings");
			if (m_bShowStanceSettings)
			{
				if (_playerScript.m_bCanCrouch)
				{
					m_fCrouchCameraYOffset.floatValue = EditorGUILayout.FloatField(new GUIContent("Crouch Camera Y Offset", "Used to move the camera to the desired head position"), _playerScript.m_fCrouchCameraYOffset);
					m_crouchHeight.floatValue = EditorGUILayout.FloatField(new GUIContent("Crouch Height", "Used to change player height"), _playerScript.m_crouchHeight);
				}
				if (_playerScript.m_bCanProne)
				{
					m_fProneCameraYOffset.floatValue = EditorGUILayout.FloatField(new GUIContent("Prone Camera Y Offset", "Used to move the camera to the desired head position"), _playerScript.m_fProneCameraYOffset);
					m_proneHeight.floatValue = EditorGUILayout.FloatField(new GUIContent("Prone Height", "Used to change player height"), _playerScript.m_proneHeight);
				}
			}
		}

		EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
		m_bShowGroundMovement = EditorGUILayout.Foldout(m_bShowGroundMovement, "Ground Movement");
		if (m_bShowGroundMovement)
		{
			m_fGravity.floatValue = EditorGUILayout.FloatField(new GUIContent("Gravity", "How much gravity to apply to player"), _playerScript.m_fGravity);
			m_fFriction.floatValue = EditorGUILayout.FloatField(new GUIContent("Friction", "How much the player has"), _playerScript.m_fFriction);
			m_fWalkSpeed.floatValue = EditorGUILayout.FloatField(new GUIContent("Walk Speed", "Max speed to Walk at"), _playerScript.m_fWalkSpeed);
			if (_playerScript.m_bCanSprint)
			{
				m_bSprintToWalk.boolValue = EditorGUILayout.Toggle(new GUIContent("Sprint To Walk", "Makes it so sprinting makes the player walk"), _playerScript.m_bSprintToWalk);
				m_fRunSpeed.floatValue = EditorGUILayout.FloatField(new GUIContent("Run Speed", "Max speed to run at"), _playerScript.m_fRunSpeed);
			}
			if (_playerScript.m_bCanCrouch)
			{
				m_fCrouchSpeed.floatValue = EditorGUILayout.FloatField(new GUIContent("Crouch Speed", "Max speed to run at"), _playerScript.m_fCrouchSpeed);
			}
			if (_playerScript.m_bCanProne)
			{
				m_fProneSpeed.floatValue = EditorGUILayout.FloatField(new GUIContent("Prone Speed", "Max speed to run at"), _playerScript.m_fProneSpeed);
			}
			m_fGroundAcceleration.floatValue = EditorGUILayout.FloatField(new GUIContent("Ground Acceleration", "How fast to accelerate to reach speed"), _playerScript.m_fGroundAcceleration);
			m_fGroundDeceleration.floatValue = EditorGUILayout.FloatField(new GUIContent("Ground Deceleration", "How fast to decekerate player"), _playerScript.m_fGroundDeceleration);

		}

		if (_playerScript.m_bCanLean)
		{
			EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
			m_bShowLeanSettings = EditorGUILayout.Foldout(m_bShowLeanSettings, "Lean Settings");
			if (m_bShowLeanSettings)
			{
				m_playerHitBox.objectReferenceValue = EditorGUILayout.ObjectField("Player HitBox", _playerScript.m_playerHitBox, typeof(GameObject), true) as GameObject;
				//m_fLeanAmount.floatValue = EditorGUILayout.FloatField(new GUIContent("Lean Amount", "How far to lean"), _playerScript.m_fLeanAmount);
			}
		}

		EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
		m_bShowAirMovement = EditorGUILayout.Foldout(m_bShowAirMovement, "Jump/Air Movement");
		if (m_bShowAirMovement)
		{
			m_fAirControl.floatValue = EditorGUILayout.FloatField(new GUIContent("Air Control", "How precise air control is, higher number means u will move directions in the air quicker, strafing is better with a lower number"), _playerScript.m_fAirControl);
			m_fJumpFriction.floatValue = EditorGUILayout.FloatField(new GUIContent("Jump Friction", "How much friction to apply when jumping"), _playerScript.m_fJumpFriction);
			if (!_playerScript.m_bCanStrafe)
			{
				m_fAirAcceleration.floatValue = EditorGUILayout.FloatField(new GUIContent("Air Acceleration", "Used to speed the player up in the air"), _playerScript.m_fAirAcceleration);
				m_fAirDeceleration.floatValue = EditorGUILayout.FloatField(new GUIContent("Air Deceleration", "Slows the player down oposite the direction they are jumping, not used when strafing is enabled"), _playerScript.m_fAirDeceleration);
			}
			if (_playerScript.m_bCanStrafe)
			{
				m_fSideStrafeAcceleration.floatValue = EditorGUILayout.FloatField(new GUIContent("Strafe Acceleration", "How fast acceleration occurs to get up to strafe speed"), _playerScript.m_fSideStrafeAcceleration);
				m_fSideStrafeSpeed.floatValue = EditorGUILayout.FloatField(new GUIContent("Strafe Speed", "What the max speed to generate when side strafing"), _playerScript.m_fSideStrafeSpeed);
			}
		}

		if (_playerScript.m_bWillSlideOnSlopes)
		{
			EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
			m_bShowSlideSettings = EditorGUILayout.Foldout(m_bShowSlideSettings, "Slide Settings");
			if (m_bShowSlideSettings)
			{
				if (_playerScript.m_bCanSlide)
				{
					m_fSlideSpeed.floatValue = EditorGUILayout.FloatField(new GUIContent("Slide Speed", "How fast the player slides"), _playerScript.m_fSlideSpeed);
					m_fSpeedToStartSlide.floatValue = EditorGUILayout.FloatField(new GUIContent("Speed To Start Slide", "What speed the player needs to be above to start a slide"), _playerScript.m_fSpeedToStartSlide);
					m_fSlideSlopeSpeed.floatValue = EditorGUILayout.FloatField(new GUIContent("Slide Slope Speed", "How fast the player slides down a slope"), _playerScript.m_fSlideSlopeSpeed);
					m_fSlideSlopeLimit.floatValue = EditorGUILayout.FloatField(new GUIContent("Slide Slope Limit", "How much of a slope the terrain needs to be to slide down while sliding"), _playerScript.m_fSlideSlopeLimit);
					m_fSlideAcceleration.floatValue = EditorGUILayout.FloatField(new GUIContent("Slide Acceleration", "How much to accelerate to reach slide speed"), _playerScript.m_fSlideAcceleration);
					m_fSlideDeceleration.floatValue = EditorGUILayout.FloatField(new GUIContent("Slide Deceleration", "How much to slow down when sliding"), _playerScript.m_fSlideDeceleration);
				}
			}
		}


		{
			EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
			m_bShowSlopeSettings = EditorGUILayout.Foldout(m_bShowSlopeSettings, "Slope Settings");
			if (m_bShowSlopeSettings)
			{
				if (_playerScript.m_bWillSlideOnSlopes)
				{
					m_fSlopeSpeed.floatValue = EditorGUILayout.FloatField(new GUIContent("Slope Speed", "How fast the player gets pushed down a slope"), _playerScript.m_fSlopeSpeed);
				}
				m_fSlopeForce.floatValue = EditorGUILayout.FloatField(new GUIContent("Slope Force", "How much force to apply to the players Y to stop it bouncing down slopes"), _playerScript.m_fSlopeForce);
				m_fSlopeRayLength.floatValue = EditorGUILayout.FloatField(new GUIContent("Slope Raycast Length", "How far to raycast down, starts form half the player height"), _playerScript.m_fSlopeRayLength);
			}
		}

		{
			EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
			m_bAnimatorSettings = EditorGUILayout.Foldout(m_bShowSlopeSettings, "Animator Settings");
			if (m_bAnimatorSettings)
			{
				m_animator.objectReferenceValue = EditorGUILayout.ObjectField("Player Animator", _playerScript.m_animator, typeof(Animator), true) as Animator;
			}
			EditorGUILayout.LabelField("----------------------------------------------------------------------", EditorStyles.boldLabel);
		}

		this.serializedObject.ApplyModifiedProperties();
		//base.OnInspectorGUI();
		//HERE there should be some values that cross over if one thing is ticked for example slide and slope i think bothe use slope force, idk what else.

	}
}
