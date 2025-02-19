using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerController : MonoBehaviour
{
	[Header("Enable Funtions")]
	// Used to turn sprint function off
	public bool m_bCanSprint = false;
	// Used to turn jump function off
	public bool m_bCanJump = false;
	// Used to turn crouch function off
	public bool m_bCanCrouch = false;
	// Used to turn prone function off
	public bool m_bCanProne = false;
	// Used to turn slide function off
	public bool m_bCanSlide = false;
	// Used to turn slope function off
	public bool m_bWillSlideOnSlopes = false;
	// Used to turn strafe function off
	public bool m_bCanStrafe = false;
	// Used to turn lean function off
	public bool m_bCanLean;

	[Header("KeyBindings")]
	// Used to change button press to button hold 
	public bool m_bHoldToJump;
	// Used to set the player control for jump using the keycodes
	public KeyCode m_jump = KeyCode.Space;
	// Used to change button press to button hold 
	public bool m_bHoldToCrouch;
	// Used to set the player control for crouch using the keycodes
	public KeyCode m_crouch = KeyCode.C;
	// Used to change button press to button hold 
	public bool m_bHoldToProne;
	// Used to set the player control for prone using the keycodes
	public KeyCode m_prone = KeyCode.Z;
	// Used to change button press to button hold 
	public bool m_bHoldToSprint;
	// Used to set the player control for sprint using the keycodes
	public KeyCode m_sprint = KeyCode.LeftShift;
	// Used to change button press to button hold 
	public bool m_bHoldToLeanLeft;
	// Used to set the player control for leftLean using the keycodes
	public KeyCode m_leanLeft = KeyCode.Q;
	// Used to change button press to button hold 
	public bool m_bHoldToLeanRight;
	// Used to set the player control for rightLean using the keycodes
	public KeyCode m_leanRight = KeyCode.E;

	[Header("Camera Settings")]
	// Transform of the camera
	//public Transform m_cameraTransform;
	//public GameObject m_playerCameraObj;
	// Player Camera
	public Camera m_playerCamera;
	//public Transform m_head;
	// Used to change how fast the camera moves between stances
	public float m_fCameraChangeStanceSpeed = 0.1f;
	// The height at which the camera is bound to
	public float m_fStandCameraYOffset = 0.65f;
	// Camera rotation X
	private float m_fRotX = 0.0f;
	// Camera rotation Y
	private float m_fRotY = 0.0f;
	// Used to set the FOV for the camera
	[Range(20.0f, 150.0f)]
	public float m_fFieldOfView;
	// Mouse X Sensitivity
	public float m_fXMouseSensitivity = 30.0f;
	// Mouse Y Sensitivity
	 public float m_fYMouseSensitivity = 30.0f;
	

	[Header("Ground Movement Settings")]
	// Used to go from sprinting to walking if ticked
	public bool m_bSprintToWalk = false;
	// Frame occuring factors
	public float m_fGravity = 20.0f;
	// Ground friction
	public float m_fFriction = 3;
	// Ground walk speed
	public float m_fWalkSpeed = 7.5f;
	// Ground run speed
	public float m_fRunSpeed = 10.0f;
	// Ground crouch speed
	public float m_fCrouchSpeed = 5.0f;
	// Ground prone speed
	public float m_fProneSpeed = 2.5f;
	// The speed at which the character's up axis gains when hitting jump
	public float m_fJumpSpeed = 8.0f;
	// Ground accel
	public float m_fGroundAcceleration = 10.0f;
	// Deacceleration that occurs when running on the ground
	public float m_fGroundDeceleration = 15.0f;
	// Stores the original friction
	private float m_fOriginalFriction;
	// Used for sprinting
	private bool m_bRun = false;
	// Vertical Movement
	private float m_fVerticalMovement;
	// Horizontal Movement
	private float m_fHorizontalMovement;

	[Header("Air Movement Settings")]
	// How much u accelerate in air when standing sill, is used when strafing is disabled
	public float m_fAirAcceleration = 10.0f;
	// Slows the player down oposite the direction they are jumping, is used when strafing is disabled
	public float m_fAirDeceleration = 100.0f;
	// How precise air control is, higher number means u will move directions in the air quicker, lower num recomened for strafing higher with strafing disabled
	public float m_fAirControl = 0.3f;
	// How fast acceleration occurs to get up to sideStrafeSpeed
	public float m_fSideStrafeAcceleration = 100.0f;
	// What the max speed to generate when side strafing
	public float m_fSideStrafeSpeed = 10.0f;
	// How much friction to apply when jumping
	public float m_fJumpFriction = 3.0f;
	private bool m_bDisableJump;
	private bool m_bIsJumping;

	[Header("Stance Settings")]
	// Used to set the camera up for crouch pos
	public float m_fCrouchCameraYOffset;
	// Used to set the camera up for prone pos
	public float m_fProneCameraYOffset;
	// Used to change the hitbox
	public float m_proneHeight;
	// Used to change the hitbox
	public float m_crouchHeight;
	// Used to change the hitbox
	private float m_standingHeight;
	// Used to check if no crouch, prone or sprint action is happening
	private bool m_bWalk = true;
	// Used to crouch
	private bool m_bCrouched = false;
	// Used to prone
	private bool m_bProned = false;

	[Header("Slope Settings")]
	// Used for the speed when on a slope over the slope limit
	public float m_fSlopeSpeed = 8.0f;
	// The length of the ray cast for slope dectection
	public float m_fSlopeRayLength = 2.0f;
	// may need to be visible for sliding values
	public float m_fSlopeForce = 20.0f;
	private Vector3 m_v3HitPointNormal;
	private bool m_bIsSliding
	{
		get
		{
			if(FPSCC.isGrounded && m_bWillSlideOnSlopes && Physics.Raycast(transform.position, Vector3.down, out RaycastHit _slopeHit, 2f))
			{
				m_v3HitPointNormal = _slopeHit.normal;
				return Vector3.Angle(m_v3HitPointNormal, Vector3.up) > FPSCC.slopeLimit;
			}
			else
			{
				return false;
			}
		}
	}

	[Header("Slide Settings")]
	// Used to set how fast to slide
	public float m_fSlideSpeed = 10.0f;
	// Used to set how fast to slide down slopes while sliding
	public float m_fSlideSlopeSpeed = 0.2f;
	// Used to determine if the slide should slide down a slope or go straight based on the slope limit
	public float m_fSlideSlopeLimit = 20.0f;
	// Used to accelerate the player to reach the slide speed
	public float m_fSlideAcceleration = 1.0f;
	// Used to start a slide if the player is over the speed
	// for example if they are in the air and are faster then the speed set below then they will slide when landing
	public float m_fSpeedToStartSlide = 5.0f;
	// How fast the slide will come to a stop
	public float m_fSlideDeceleration = 2.0f;
	// Used to check if player is currently sliding
	private bool m_bSlide;
	// Used to check if it is the start of a slide
	private bool m_bSlideStart;
	// Stores the original speed
	private float m_fOriginalSlideSpeed;
	// Stores the Y pos of last frame
	private float m_fOldYPos;

	[Header("Lean Settings")]
	public bool m_bLeaning;
	//public float m_fLeanAmount;
	public float m_fLeanTime;
	public GameObject m_playerHitBox;
	//public Transform m_playerHead;

	// Used to change the collider height and centre for stance changing
	private CapsuleCollider m_capsuleCollider;

	// Used to store the move direction normalised for the movment
	private Vector3 m_v3MoveDirectionNorm = Vector3.zero;
	// Stores the player velocity allowing for the player to have momentum
	private Vector3 m_v3PlayerVelocity = Vector3.zero;
	// Used to get the character controller values
	private CharacterController FPSCC;

	//[Header("Network Settings")]
	//private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1); 

	[Header("Animation Settings")]
	public Animator m_animator;

	//--------------------------------------------------------------------------------
	// Awake used for initialization.
	//--------------------------------------------------------------------------------
	private void Awake()
	{
		// Hide and lock the cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// Stores the original friction
		m_fOriginalFriction = m_fFriction;

		// Sets the player camera enabled
		m_playerCamera.enabled = true;
		m_playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + m_fStandCameraYOffset, transform.position.z);

		// Gets access to the player character controller
		FPSCC = GetComponent<CharacterController>();

		m_capsuleCollider = GetComponent<CapsuleCollider>();

		// Sets the original speed to the speed set on awake
		m_fOriginalSlideSpeed = m_fSlideSpeed;

		// Sets the standing height to the hieght of the character controller
		m_standingHeight = FPSCC.height;
	}

	//--------------------------------------------------------------------------------
	// Update is called once every frame. It allows the player to move as well as
	// changes the FOV, locks the camera and hides the cursor, moves the camera, 
	// and limits the max speed the player can go.
	//--------------------------------------------------------------------------------
	private void Update()
	{
		// Sets the FOV of the camera
		m_playerCamera.fieldOfView = m_fFieldOfView;

		// Ensure that the cursor is locked into the screen 
		//if (Cursor.lockState != CursorLockMode.Locked)
		//{
		//	if (Input.GetButtonDown("Fire1"))
		//		Cursor.lockState = CursorLockMode.Locked;
		//}

		// Gets the mouses input to rotate the camera based on it
		m_fRotX -= Input.GetAxisRaw("Mouse Y") * m_fXMouseSensitivity * 0.02f;
		m_fRotY += Input.GetAxisRaw("Mouse X") * m_fYMouseSensitivity * 0.02f;

		// Clamp the X rotation and prevents gimbal lock
		if (m_fRotX < -90)
			m_fRotX = -90;
		else if (m_fRotX > 90)
			m_fRotX = 90;

		// Rotates the collider
		this.transform.rotation = Quaternion.Euler(0, m_fRotY, 0);
		// Rotates the camera
		m_playerCamera.transform.rotation = Quaternion.Euler(m_fRotX, m_fRotY, 0);
		// Rotates the hitbox leaning
		//m_playerHitBox.transform.rotation = Quaternion.Euler(0, m_fRotY, 0);

		// If the player is grounded then do ground move otherwise use airmove as they are in the air
		if (FPSCC.isGrounded)
		{
			GroundMove();
			m_animator.SetBool("Jump", false);
		}
		else if (!FPSCC.isGrounded)
		{
			AirMove();
			m_animator.SetBool("Jump", true);
			m_animator.SetBool("Walk", false);
			m_animator.SetBool("Run", false);
		}

		if(m_bIsSliding)
		{
			m_bDisableJump = true;
			m_v3PlayerVelocity += new Vector3(m_v3HitPointNormal.x, -m_v3HitPointNormal.y, m_v3HitPointNormal.z) * m_fSlopeSpeed;
		}
		else if(!m_bIsSliding)
		{
			m_bDisableJump = false;
		}

		// Move the controller
		FPSCC.Move(m_v3PlayerVelocity * Time.deltaTime);

		if (FPSCC.isGrounded)
		{
			m_bIsJumping = false;
		}

		if (OnSlope() && !m_bIsJumping)
		{
			FPSCC.Move(Vector3.down * FPSCC.height / 2 * m_fSlopeForce * Time.deltaTime);
		}

		//Checks what stance the player is in and moves the camera to fit it
		if (m_bCrouched)
		{
			// Set the camera's position to the transform
			SetCameraYPos(m_fCrouchCameraYOffset);
			FPSCC.height = Mathf.Lerp(FPSCC.height, m_crouchHeight, m_fCameraChangeStanceSpeed);
			m_capsuleCollider.height = Mathf.Lerp(m_capsuleCollider.height, m_crouchHeight, m_fCameraChangeStanceSpeed);
			FPSCC.center = Vector3.Lerp(FPSCC.center, new Vector3(0,m_crouchHeight / 2, 0), m_fCameraChangeStanceSpeed);
			m_capsuleCollider.center = Vector3.Lerp(m_capsuleCollider.center, new Vector3(0, m_crouchHeight / 2, 0), m_fCameraChangeStanceSpeed);
		}
		else if (m_bProned)
		{
			// Set the camera's position to the transform
			SetCameraYPos(m_fProneCameraYOffset);
			FPSCC.height = Mathf.Lerp(FPSCC.height, m_proneHeight, m_fCameraChangeStanceSpeed);
			m_capsuleCollider.height = Mathf.Lerp(m_capsuleCollider.height, m_proneHeight, m_fCameraChangeStanceSpeed);
			FPSCC.center = Vector3.Lerp(FPSCC.center, new Vector3(0, m_proneHeight / 2, 0), m_fCameraChangeStanceSpeed);
			m_capsuleCollider.center = Vector3.Lerp(m_capsuleCollider.center, new Vector3(0, m_proneHeight / 2, 0), m_fCameraChangeStanceSpeed);
		}
		else if(!m_bLeaning)
		{
			// Set the camera's position to the transform
			SetCameraYPos(m_fStandCameraYOffset);
			FPSCC.height = Mathf.Lerp(FPSCC.height, m_standingHeight, m_fCameraChangeStanceSpeed);
			m_capsuleCollider.height = Mathf.Lerp(m_capsuleCollider.height, m_standingHeight, m_fCameraChangeStanceSpeed);
			FPSCC.center = Vector3.Lerp(FPSCC.center, new Vector3(0, m_standingHeight / 2, 0), m_fCameraChangeStanceSpeed);
			m_capsuleCollider.center = Vector3.Lerp(m_capsuleCollider.center, new Vector3(0, m_standingHeight / 2, 0), m_fCameraChangeStanceSpeed);
		}


	}

	//--------------------------------------------------------------------------------
	// Sets the movement direction based on the input.
	//--------------------------------------------------------------------------------
	private void SetMoveDir()
	{
		// Sets the horizontal movement to the input
		m_fHorizontalMovement = Input.GetAxisRaw("Vertical");
		// Sets the vertical movement to the input
		m_fVerticalMovement = Input.GetAxisRaw("Horizontal");
	}

	//--------------------------------------------------------------------------------
	// Allows the player to control the character in the air, such as strafing which
	// allows the player to gain momentum by moving left and right in a specific way.
	//--------------------------------------------------------------------------------
	private void AirMove()
	{
		// Stores the direction the player wants to move
		Vector3 v3DesDir;
		// Used to accelerate and deccelerate the player
		float fAccel;
		// Sets the move direction based on player input
		SetMoveDir();
		// Sets the desired direction based on the movement
		v3DesDir = new Vector3(m_fVerticalMovement, 0, m_fHorizontalMovement);
		// Changes the desired direction from local space to world space
		v3DesDir = transform.TransformDirection(v3DesDir);
		// Sets the desired speed
		float fDesSpeed = v3DesDir.magnitude;

		// Changes the players stance from prone, crouch, sprint based on the input 
		ChangeStance();

		v3DesDir.Normalize();
		// Sets the normalized move direction
		m_v3MoveDirectionNorm = v3DesDir;

		// Checks if the dot product of player vel and the des direction is less then zero 
		// And accelerates or deccelerates based on the result 
		if (Vector3.Dot(m_v3PlayerVelocity, v3DesDir) < 0)
			fAccel = m_fAirDeceleration;
		else
			fAccel = m_fAirAcceleration;

		if(m_bCanStrafe)
		{
			// If the player is ONLY strafing left or right
			if (m_fVerticalMovement == 0 && m_fHorizontalMovement != 0)
			{
				// If the player is stafing increase the speed based on the variables
				if (fDesSpeed > m_fSideStrafeSpeed)
					fDesSpeed = m_fSideStrafeSpeed;
				fAccel = m_fSideStrafeAcceleration;
			}

			// If the player is ONLY strafing forward or backward
			if (m_fVerticalMovement != 0 && m_fHorizontalMovement == 0)
			{
				// If the player is stafing increase the speed based on the variables
				if (fDesSpeed > m_fSideStrafeSpeed)
					fDesSpeed = m_fSideStrafeSpeed;
				fAccel = m_fSideStrafeAcceleration;
			}
		}

		// Accelerates the player based on the values calculated above
		Accelerate(v3DesDir, fDesSpeed, fAccel);
		// If the air control is greater then zero then use the player air control movement
		// the higher air control the eaiser it is to move left right forward back without strafing
		//strafing can be used with if the air control is not too high as air controll makes the player move more in air based in movement input instead of mouse look / strafing
		if (m_fAirControl > 0)
			AirControl(v3DesDir, fDesSpeed);

		// Apply gravity
		m_v3PlayerVelocity.y -= m_fGravity * Time.deltaTime;
	}
	private void AirControl(Vector3 v3DesDir, float fDesSpeed)
	{
		float zspeed;
		float speed;
		float dot;
		float k;

		// Checks if there is movement as you can't control movement if not moving forward or backward
		if (Mathf.Abs(m_fVerticalMovement) < 0.001 || Mathf.Abs(fDesSpeed) < 0.001)
			return;
		// Stores the y player velocity
		zspeed = m_v3PlayerVelocity.y;
		// Sets the y player velocity to zero as the zspeed is storing the value
		m_v3PlayerVelocity.y = 0;

		// Sets the speed to the magnitude of the player velocity
		speed = m_v3PlayerVelocity.magnitude;
		// Normalizes the player vel
		m_v3PlayerVelocity.Normalize();

		// Sets dot to the Dot product of the player vel and desiered direction
		dot = Vector3.Dot(m_v3PlayerVelocity, v3DesDir);
		k = 32;
		// Calculates k which is used in changing the player vel
		k *= m_fAirControl * dot * dot * Time.deltaTime;

		// Change direction while slowing down
		if (dot > 0)
		{
			m_v3PlayerVelocity.x = m_v3PlayerVelocity.x * speed + v3DesDir.x * k;
			m_v3PlayerVelocity.y = m_v3PlayerVelocity.y * speed + v3DesDir.y * k;
			m_v3PlayerVelocity.z = m_v3PlayerVelocity.z * speed + v3DesDir.z * k;

			m_v3PlayerVelocity.Normalize();
			m_v3MoveDirectionNorm = m_v3PlayerVelocity;

		}
		// Adds the speed to the player velocity
		m_v3PlayerVelocity.x *= speed;
		m_v3PlayerVelocity.y = zspeed;
		m_v3PlayerVelocity.z *= speed;
	}

	//--------------------------------------------------------------------------------
	// Moves the player based on the movement input and stores the velocity to give
	// the feeling of gaining momentum, also makes the player slide down slopes and
	// lets them jump, crouch, prone, and sprint.
	//--------------------------------------------------------------------------------
	private void GroundMove()
	{
		// Stores the direction the player wants to move
		Vector3 v3DesDir;
		// Sets the move direction based on player input
		SetMoveDir();
		// Sets the desired direction based on the movement
		v3DesDir = new Vector3(m_fVerticalMovement, 0, m_fHorizontalMovement);
		// Changes the desired direction from local space to world space
		v3DesDir = transform.TransformDirection(v3DesDir);
		v3DesDir.Normalize();
		// Sets the normalized move direction
		m_v3MoveDirectionNorm = v3DesDir;
		// Sets the desired speed
		var fDesSpeed = v3DesDir.magnitude;

		// Reset the gravity velocity
		m_v3PlayerVelocity.y = -1f;
		//m_v3PlayerVelocity.y = -m_fGravity * Time.deltaTime;

		// Changes the players stance from prone, crouch, sprint based on the input
		ChangeStance();

		// Added checks and moved accelerate here so it did not double accelerate in the stance function.
		if (m_bCrouched)
		{
			// Checks if the player is crouched and does the acceleration needed
			Accelerate(v3DesDir, fDesSpeed *= m_fCrouchSpeed, m_fGroundAcceleration);
		}
		if (m_bProned)
		{
			// Checks if the player is proned and does the acceleration needed
			Accelerate(v3DesDir, fDesSpeed *= m_fProneSpeed, m_fGroundAcceleration);
		}
		if (m_bWalk && !m_bCrouched && !m_bProned)
		{
		//TODO add float in animation for speed to set bool to false is desSpeed is nothing or...
			if (fDesSpeed == 0)
			{
				m_animator.SetBool("Walk", false);
			}
			else
			{
				m_animator.SetBool("Walk", true);
			}
			// Checks if the player is walking and does the acceleration needed
			Accelerate(v3DesDir, fDesSpeed *= m_fWalkSpeed, m_fGroundAcceleration);
		}
		if (m_bRun && !m_bCrouched && !m_bProned)
		{
			// Checks if the player is running and does the acceleration needed
			Accelerate(v3DesDir, fDesSpeed *= m_fRunSpeed, m_fGroundAcceleration);
		}

		m_fFriction = m_fOriginalFriction;

		// if the player can jump and it is not disabled then checks if it needs to be held or pressed and does jump code
		if(m_bCanJump && !m_bDisableJump)
		{
			if(m_bHoldToJump)
			{
				// Checks if the jump button is pressed and the player is not proned then jumps
				// Applies friction to the player to slow down their momentum 
				if (Input.GetKey(m_jump) && !m_bProned)
				{
					m_v3PlayerVelocity.y = m_fJumpSpeed;
					m_bIsJumping = true;
					//m_animator.SetBool("Jump", true);
					ApplyFriction(m_fJumpFriction);
				}
				else
					ApplyFriction(1.0f);
			}
			else if(!m_bHoldToJump)
			{
				// Checks if the jump button is pressed and the player is not proned then jumps
				// Applies friction to the player to slow down their momentum
				if (Input.GetKeyDown(m_jump) && !m_bProned)
				{
					m_v3PlayerVelocity.y = m_fJumpSpeed;
					m_bIsJumping = true;
					//m_animator.SetBool("Jump", true);
					ApplyFriction(m_fJumpFriction);
				}
				else
					ApplyFriction(1.0f);
			}
			
		}
		else
			ApplyFriction(1.0f);
		// HERE maybe change 1.0f to m_fFriction
	}

	//--------------------------------------------------------------------------------
	// Applies friction to the player so they can slow down
	//
	// Param:
	//		t: Used to determine the amount of friction to apply
	//
	//--------------------------------------------------------------------------------
	private void ApplyFriction(float t)
	{
		// Sets the vec to equal the player vel
		Vector3 vec = m_v3PlayerVelocity;
		// Speed which will be used to calulate the friction speed
		float speed;
		// The new speed with friction applied
		float newspeed;
		float control;
		// The amount the speed should drop with the friction
		float drop;

		vec.y = 0.0f;
		speed = vec.magnitude;
		drop = 0.0f;

		// Only if the player is on the ground then apply friction
		// HERE could add a check if they want air friction
		if (FPSCC.isGrounded)
		{
			control = speed < m_fGroundDeceleration ? m_fGroundDeceleration : speed;
			drop = control * m_fFriction * Time.deltaTime * t;
		}
		// Sets the new speed based on the speed and drop
		newspeed = speed - drop;

		if (newspeed < 0)
			newspeed = 0;
		if (speed > 0)
			newspeed /= speed;
		// Changes the player vel to the new speed with friction
		m_v3PlayerVelocity.x *= newspeed;
		m_v3PlayerVelocity.z *= newspeed;
	}

	//--------------------------------------------------------------------------------
	// Accelerates the players velocity based on the values passed in.
	//
	// Param:
	//		v3DesDir: The desired direction the player should move
	//		fDesSpeed: The desired speed the player should move at
	//		fAccel: The rate the player should accelerate
	//--------------------------------------------------------------------------------
	private void Accelerate(Vector3 v3DesDir, float fDesSpeed, float fAccel)
	{
		float addspeed;
		float accelspeed;
		float currentspeed;

		// Sets the currecnt speed to the dot product of the player vel and des dir
		currentspeed = Vector3.Dot(m_v3PlayerVelocity, v3DesDir);
		// Add speed equals the desired speed minus the current speed
		addspeed = fDesSpeed - currentspeed;
		// If there is no speed being added exit this function
		if (addspeed <= 0)
			return;
		// Calculates the accel speed
		accelspeed = fAccel * Time.deltaTime * fDesSpeed;
		if (accelspeed > addspeed)
			accelspeed = addspeed;
		// Accelerats the players velocity
		m_v3PlayerVelocity.x += accelspeed * v3DesDir.x;
		m_v3PlayerVelocity.z += accelspeed * v3DesDir.z;
	}

	//--------------------------------------------------------------------------------
	// Lerps the camera between the current camera pos and the new offset, used when 
	// the player changes stance
	//
	// Param:
	//		yOffset: Used to change the y position of the camera
	//
	//--------------------------------------------------------------------------------
	private void SetCameraYPos(float yOffset)
	{
		// Moves the camera into a FPS desired y offset postion in the player capsule
		Vector3 m_v3NextCameraPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
		m_playerCamera.transform.position = Vector3.Lerp(m_playerCamera.transform.position, m_v3NextCameraPos, m_fCameraChangeStanceSpeed);
	}

	//--------------------------------------------------------------------------------
	// Makes the player crouch, while checking if the player is able to stand by
	// doing a sphere cast above checking if it hits anything
	//--------------------------------------------------------------------------------
	private void Crouch()
	{
		// used to store hit data on the spherecast
		RaycastHit _hit;
		if(m_bHoldToCrouch)
		{
			// if the player is holding the crouch button and there is nothing above the player incase the player was prone
			if (Input.GetKey(m_crouch) && !Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_proneHeight))
			{
				// now the player should be crouching
				m_bCrouched = true;
				// and if the player was prone then they no longer are
				m_bProned = false;
			}
			// if the player lets go of crouch and there is nothing above them then they should stand
			else if (m_bCrouched && !Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_crouchHeight))
			{
				m_bCrouched = false;
			}
		}
		else if(!m_bHoldToCrouch)
		{
			// if the player is presses the crouch button and there is nothing above the player incase the player was prone
			if (Input.GetKeyDown(m_crouch) && !m_bCrouched && !Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_proneHeight))
			{
				// now the player should be crouching
				m_bCrouched = true;
				// and if the player was prone then they no longer are
				m_bProned = false;
			}
			// if the player is presses the crouch button again to uncrouch and there is nothing above them then uncrouch
			else if (Input.GetKeyDown(m_crouch) && m_bCrouched && !Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_crouchHeight))
			{
				m_bCrouched = false;
			}
		}
	}

	//--------------------------------------------------------------------------------
	// Makes the player prone, while checking if the player is able to stand by
	// doing a sphere cast above checking if it hits anything
	//--------------------------------------------------------------------------------
	private void Prone()
	{
		// checks if grounded as prone can only happen if on ground
		if(FPSCC.isGrounded)
		{
			// used to store hit data on the spherecast
			RaycastHit _hit;
			if (m_bHoldToProne)
			{
				// if the player is holding the prone button
				if (Input.GetKey(m_prone))
				{
					// the player should now prone
					m_bProned = true;
					// and if they were crouched it should now be false
					m_bCrouched = false;
				}
				// if the player releases the prone button and there is nothing above then they stand
				else if (m_bProned && !Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_crouchHeight))
				{
					m_bProned = false;
				}
			}
			else if(!m_bHoldToProne)
			{
				// if the player is presses the prone button
				if (Input.GetKeyDown(m_prone) && !m_bProned)
				{
					// the player should now prone
					m_bProned = true;
					// and if they were crouched it should now be false
					m_bCrouched = false;
				}
				// if the player is presses the prone button again to unprone and there is nothing above them then stand
				else if (Input.GetKeyDown(m_prone) && m_bProned && !Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_crouchHeight))
				{
					m_bProned = false;
				}
			}
		}
	}

	//--------------------------------------------------------------------------------
	// Makes the player sprint or walk, checks if there is anything above the player
	// if not makes them stand up and sprint.
	//--------------------------------------------------------------------------------
	private void Sprint()
	{
		// used to store hit data on the spherecast
		RaycastHit _hit;
		if (m_bHoldToSprint)
		{
			// checks if sprint is being held, and sprinting to walk is false, also the player is not sliding
			if (Input.GetKey(m_sprint) && !m_bSprintToWalk && !m_bSlide)
			{
				// checks if the player is in a different stance and if being blocked by something then they cant stand and sprint
				if (m_bProned && Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_crouchHeight) || m_bCrouched && Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_crouchHeight))
				{
					m_bRun = false;
					m_animator.SetBool("Run", false);
				}
				// if the player is in a different stance and theres nothing blocking them then they stand and sprint
				else
				{
					// the player should now stand and sprint 
					m_bRun = true;
					m_animator.SetBool("Run", true);
					// walking, prone and crouch should all now be false
					m_bWalk = false;
					m_animator.SetBool("Walk", false);
					m_bProned = false;
					m_bCrouched = false;
				}
			}
			// if the player is no longer holding sprint set the player to walking
			else if (!m_bSprintToWalk && m_bRun)
			{
				m_bWalk = true;
				m_animator.SetBool("Walk", true);
				m_bRun = false;
				m_animator.SetBool("Run", false);
			}
			// if sprint to walk is true then the player walks when the sprint button is held
			if (Input.GetKey(m_sprint) && m_bSprintToWalk)
			{
				m_bWalk = true;
				m_animator.SetBool("Walk", true);
				m_bRun = false;
				m_animator.SetBool("Run", false);
			}
			// if the player is not holding sprint to walk then they are running
			else if (m_bSprintToWalk && m_bWalk)
			{
				m_bWalk = false;
				m_animator.SetBool("Walk", false);
				m_bRun = true;
				m_animator.SetBool("Run", true);
			}
		}
		else if(!m_bHoldToSprint)
		{
			// checks if sprint is pressed, and sprinting to walk is false, also the player is not already running
			if (Input.GetKeyDown(m_sprint) && !m_bSprintToWalk && !m_bRun)
			{
				// checks if the player is in a different stance and if being blocked by something then they cant stand and sprint
				if (m_bProned && Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_crouchHeight) || m_bCrouched && Physics.SphereCast(transform.position, FPSCC.radius, Vector3.up, out _hit, m_crouchHeight))
				{
					m_bRun = false;
					m_animator.SetBool("Run", false);
				}
				// if the player is in a different stance and theres nothing blocking them then they stand and sprint
				else
				{
					// the player should now stand and sprint 
					m_bRun = true;
					m_animator.SetBool("Run", true);
					// walking, prone and crouch should all now be false
					m_bWalk = false;
					m_animator.SetBool("Walk", false);
					m_bProned = false;
					m_bCrouched = false;
				}
			}
			// if the player presses the sprint button again then and they are already sprinting then they walk
			else if (Input.GetKeyDown(m_sprint) && !m_bSprintToWalk && m_bRun)
			{
				m_bWalk = true;
				m_animator.SetBool("Walk", true);
				m_bRun = false;
				m_animator.SetBool("Run", false);
			}
			// if sprint to walk is true then the player walks when the sprint button is pressed
			if (Input.GetKeyDown(m_sprint) && m_bSprintToWalk && !m_bWalk)
			{
				m_bWalk = true;
				m_animator.SetBool("Walk", true);
				m_bRun = false;
				m_animator.SetBool("Run", false);
			}
			// if the player presses the sprint button again then and they are already walking then they run
			else if (Input.GetKeyDown(m_sprint) && m_bSprintToWalk && m_bWalk)
			{
				m_bWalk = false;
				m_animator.SetBool("Walk", false);
				m_bRun = true;
				m_animator.SetBool("Run", true);
			}
		}
	}

	//--------------------------------------------------------------------------------
	// Makes the player do a power slide, if the player is on a slope then they will 
	// slide down the slope or will attempt to slide up it and stop.
	//--------------------------------------------------------------------------------
	private void Slide()
	{
		if(FPSCC.isGrounded)
		{
			// if the player is sprinting and goes to crouch then a slide should happen, so slide is true
			if(m_bRun && m_bCrouched)
			{
				m_bSlide = true;
			}
			// if crouch is held and player is over a certain mementum or velocity
			if(m_bCrouched && Mathf.Abs(m_v3PlayerVelocity.x) > m_fSpeedToStartSlide || m_bCrouched && Mathf.Abs(m_v3PlayerVelocity.z) > m_fSpeedToStartSlide)
			{
				// if the player is on a slope and the angle is over the slope limit then the player should slide down the slope
				if (OnSlope() && Vector3.Angle(m_v3HitPointNormal, Vector3.up) > m_fSlideSlopeLimit)
				{
					// Stores the direction the player wants to move
					Vector3 v3DesDir;
					// Sets the desired direction based on the velocity
					v3DesDir = new Vector3(m_v3PlayerVelocity.x, 0, m_v3PlayerVelocity.z);
					// Sets the desired speed
					var fDesSpeed = v3DesDir.magnitude;

					// checks if it is the start of the slide
					if (m_bSlideStart)
					{
						// stores the original speed.
						m_fOriginalSlideSpeed = m_fSlideSpeed;
						// sets start of the slide to false
						m_bSlideStart = false;
					}
					// gets the hit point normal
					m_v3HitPointNormal = SlopeHit().normal;
					// makes the player slide in the down direction of the object using the hit normal
					m_v3PlayerVelocity += new Vector3(m_v3HitPointNormal.x, -m_v3HitPointNormal.y, m_v3HitPointNormal.z) * m_fSlideSlopeSpeed;
					// Checks if the player have moved up or down on the slope from last frame
					if (m_fOldYPos < transform.position.y)
					{
						// if we are goin up then slow the player down faster so they cant slide up hill
						m_fSlideSpeed -= m_fSlideDeceleration * Time.deltaTime;
					}
					else if (m_fOldYPos > transform.position.y)
					{
						// if the player is going down hill then make them slide down using the slope angle of the hill for the speed
						m_fSlideSpeed = Vector3.Angle(m_v3HitPointNormal, Vector3.up);
					}
					// if the speed reaches below 0 then set it to 0
					if (m_fSlideSpeed <= 0)
					{
						m_fSlideSpeed = 0;
					}
					// if the speed reaches above ogriginal speed then it capes the speed to that making it have a max speed
					if (m_fSlideSpeed >= m_fOriginalSlideSpeed)
					{
						m_fSlideSpeed = m_fOriginalSlideSpeed;
					}
					// Accelerate the player to start the slide and continue the slide
					Accelerate(v3DesDir, fDesSpeed * m_fSlideSpeed, m_fSlideAcceleration);
					// sets old y pos to the current y pos so next frame it can tell if it is going up or down
					m_fOldYPos = transform.position.y;
				}
				// the player does a slide which does not go down slopes
				else
				{
					// Stores the direction the player wants to move
					Vector3 v3DesDir;
					// Sets the desired direction based on the velocity
					v3DesDir = new Vector3(m_v3PlayerVelocity.x, 0, m_v3PlayerVelocity.z);
					// Sets the desired speed
					var fDesSpeed = v3DesDir.magnitude;
					// Accelerate the player to start the slide and continue the slide
					Accelerate(v3DesDir, fDesSpeed * m_fSlideSpeed, m_fSlideAcceleration);
					// checks if it is the start of the slide
					if (m_bSlideStart)
					{
						// stores the original speed.
						m_fOriginalSlideSpeed = m_fSlideSpeed;
						// sets start of the slide to false
						m_bSlideStart = false;
					}
					// slows the slide speed down making the player eventually stop sliding
					m_fSlideSpeed -= m_fSlideSpeed * Time.deltaTime;
					// if the speed reaches below 0 then set it to 0
					if (m_fSlideSpeed < 0)
					{
						m_fSlideSpeed = 0;
					}
				}
			}
			// if the player is no longer crouched or has fallen under a certain speed then the slide is finished
			else if(!m_bCrouched || m_bCrouched && Mathf.Abs(m_v3PlayerVelocity.x) <= m_fSpeedToStartSlide || m_bCrouched && Mathf.Abs(m_v3PlayerVelocity.z) <= m_fSpeedToStartSlide)
			{
				// if it is not the start of a slide and we have cancelled a slide reset the slide speed to original
				if(!m_bSlideStart)
				{
					m_fSlideSpeed = m_fOriginalSlideSpeed;
				}
				// sets slide start back to true as the next time a slide happens it should be a new slide
				m_bSlideStart = true;
				// sliding is false as the player has stopped sliding
				m_bSlide = false;
			}
		}
	}

	//--------------------------------------------------------------------------------
	// Checks if the player is on a slope by checking if the normal is equal to the 
	// objects up.
	//
	// Return:
	//		bool: returns true or false if the player is on a slope 
	//
	//--------------------------------------------------------------------------------
	private bool OnSlope()
	{
		RaycastHit _hit;
		// Raycast down from half the player height by the slope ray length to see if there is an object
		if (Physics.Raycast(transform.position, Vector3.down, out _hit, FPSCC.height / 2 * m_fSlopeRayLength))
		{
			// if there is an object and the normal is not equal to up then the player is on a slope
			if (_hit.normal != Vector3.up)
			{
				return true;
			}
		}
		// returns false if there is no object below the player that is angled
		return false;
	}

	//--------------------------------------------------------------------------------
	// Does a ray cast down and gets the hit information of the object
	//
	// Return:
	//		 _hit: Is the object being hit by raycast.
	//--------------------------------------------------------------------------------
	private RaycastHit SlopeHit()
	{
		RaycastHit _hit;
		// Raycast down from half the player height by the slope ray length to see if there is an object
		if (Physics.Raycast(transform.position, Vector3.down, out _hit, FPSCC.height / 2 * m_fSlopeRayLength))
		{
			// if there is an object and the normal is not equal to up then return the hit information for that object
			if (_hit.normal != Vector3.up)
			{
				return _hit;
			}
		}
		return _hit;
	}

	//--------------------------------------------------------------------------------
	// Changes the stance of the player from crouch to prone, standing, sprinting and
	// sliding if the player can do those functions.
	//--------------------------------------------------------------------------------
	private void ChangeStance()
	{
		if(m_bCanProne)
		{
			Prone();
		}
		if(m_bCanSprint)
		{
			Sprint();
		}
		if(m_bCanCrouch)
		{
			Crouch();
		}
		if(m_bCanSlide)
		{
			Slide();
		}
		if (m_bCanLean)
		{
			Lean();
		}
	}

	private void Lean()
	{

		if (m_bHoldToLeanLeft)
		{
			if (Input.GetKey(m_leanLeft))
			{

				RaycastHit _hit;
				if(!Physics.SphereCast(transform.position, FPSCC.radius, -transform.right, out _hit, m_standingHeight))
				{
					m_bLeaning = true;
					m_playerHitBox.transform.eulerAngles = new Vector3(transform.rotation.x, m_fRotY, transform.rotation.z /*+ m_fLeanAmount*/);
					m_playerCamera.transform.localPosition = new Vector3(-0.8f, m_playerCamera.transform.localPosition.y, 0);
					m_playerCamera.transform.eulerAngles = new Vector3(m_fRotX, m_fRotY, transform.rotation.z/* + m_fLeanAmount*/);
				}
				else
				{
					m_bLeaning = false;
					m_playerHitBox.transform.eulerAngles = new Vector3(transform.rotation.x, m_fRotY, transform.rotation.z);
					m_playerCamera.transform.localPosition = new Vector3(0, m_playerCamera.transform.localPosition.y, 0);
					m_playerCamera.transform.eulerAngles = new Vector3(m_fRotX, m_fRotY, transform.rotation.z);
				}

			}
			else if (m_bLeaning && !Input.GetKey(m_leanRight))
			{
				m_bLeaning = false;
			}
		}
		else if (!m_bHoldToLeanLeft)
		{
			if (Input.GetKeyDown(m_leanLeft) && !m_bLeaning)
			{
				RaycastHit _hit;
				if (!Physics.SphereCast(transform.position, FPSCC.radius, Vector3.left, out _hit, m_standingHeight))
				{
					m_bLeaning = true;
					m_playerHitBox.transform.eulerAngles = new Vector3(transform.rotation.x, m_fRotY, transform.rotation.z /*+ m_fLeanAmount*/);
					m_playerCamera.transform.localPosition = new Vector3(-1f, m_playerCamera.transform.localPosition.y, 0);
					m_playerCamera.transform.eulerAngles = new Vector3(m_fRotX, m_fRotY, transform.rotation.z /*+ m_fLeanAmount*/);
				}
				else
				{
					m_playerCamera.transform.localPosition = new Vector3(0, m_playerCamera.transform.localPosition.y, 0);
				}
			}
			else if (Input.GetKeyDown(m_leanLeft) && m_bLeaning)
			{
				m_bLeaning = false;
			}
		}
		if (m_bHoldToLeanRight)
		{
			if (Input.GetKey(m_leanRight))
			{
				RaycastHit _hit;
				if (!Physics.SphereCast(transform.position, FPSCC.radius, transform.right, out _hit, m_standingHeight))
				{
					m_bLeaning = true;
					m_playerHitBox.transform.eulerAngles = new Vector3(transform.rotation.x, m_fRotY, transform.rotation.z /*- m_fLeanAmount*/);
					m_playerCamera.transform.localPosition = new Vector3(0.8f, m_playerCamera.transform.localPosition.y, 0);
					m_playerCamera.transform.eulerAngles = new Vector3(m_fRotX, m_fRotY, transform.rotation.z /*- m_fLeanAmount*/);
				}
				else
				{
					m_bLeaning = false;
					m_playerHitBox.transform.eulerAngles = new Vector3(transform.rotation.x, m_fRotY, transform.rotation.z);
					m_playerCamera.transform.localPosition = new Vector3(0, m_playerCamera.transform.localPosition.y, 0);
					m_playerCamera.transform.eulerAngles = new Vector3(m_fRotX, m_fRotY, transform.rotation.z);
				}
			}
			else if (m_bLeaning && !Input.GetKey(m_leanLeft))
			{
				m_bLeaning = false;
			}
		}
		else if (!m_bHoldToLeanRight)
		{
			if (Input.GetKeyDown(m_leanRight) && !m_bLeaning)
			{
				RaycastHit _hit;
				if (!Physics.SphereCast(transform.position, FPSCC.radius, new Vector3(1, m_playerCamera.transform.localPosition.y, 0), out _hit, m_standingHeight))
				{
					m_bLeaning = true;
					m_playerHitBox.transform.eulerAngles = new Vector3(transform.rotation.x, m_fRotY, transform.rotation.z /*- m_fLeanAmount*/);
					m_playerCamera.transform.localPosition = new Vector3(1, m_playerCamera.transform.localPosition.y, 0);
					m_playerCamera.transform.eulerAngles = new Vector3(m_fRotX, m_fRotY, transform.rotation.z /*- m_fLeanAmount*/);
				}
			}
			else if (Input.GetKeyDown(m_leanRight) && m_bLeaning)
			{
				m_bLeaning = false;
			}
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.layer != LayerMask.GetMask("Ground") && collision.gameObject.layer != LayerMask.GetMask("Player"))
		{
			m_v3PlayerVelocity = new Vector3(0, 0, 0);
		}
	}

	//--------------------------------------------------------------------------------
	// Checks if the player controller is colliding and slows down if it is not the
	// ground layer.
	//
	// Param:
	//		hit: Is the object the collider is hitting used for info on the object
	//--------------------------------------------------------------------------------
	private void OnCollisionEnter(Collision collision)
	{
		// used to check if there is an object above using sphere cast below
		RaycastHit _hit;
		if(Physics.Raycast(transform.position, collision.transform.position, out  _hit))
		{
			// checks if the object being hit is on a certian layer
			if (collision.gameObject.layer != 6)
			{
			// slows velocity down based on the hit normal 
				Debug.Log(collision.gameObject.name);
				//m_v3PlayerVelocity -= _hit.normal * Vector3.Dot(m_v3PlayerVelocity, _hit.normal);
				m_v3PlayerVelocity /= 2;
			}
		}
	}
}
//HERE if player runs into somthing it should stop vel going that way
//HERE when sliding input to stop or slow down faster
//HERE speed to start slide may also need a speed to end slide, so the player can start at a fast speed and stop at a slow speed not just start and stop at a slow speed
//HERE player collides with player hit box for stance checks and the player doesnt notice objects above when checking sphere cast with layer