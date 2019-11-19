using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
{
	// Transform of the camera
	public Transform m_cameraTransform;
	// The height at which the camera is bound to
	public float m_fStandCameraYOffset = 0.6f;
	// Mouse X Sensitivity
	public float m_fXMouseSensitivity = 30.0f;
	// Mouse Y Sensitivity
	public float m_fYMouseSensitivity = 30.0f;
	// Used to change how fast the camera moves between stances
	public float m_fCameraChangeStanceSpeed = 0.1f;
	// Used to set the FOV for the camera
	[Range(20.0f, 150.0f)]
	public float m_fFieldOfView;
	// Camera rotation X
	private float m_fRotX = 0.0f;
	// Camera rotation Y
	private float m_fRotY = 0.0f;
	// Frame occuring factors
	public float m_fGravity = 20.0f;
	// Ground friction
	public float m_fFriction = 3;
	// Used to display real time fricton values
	private float m_fPlayerFriction = 0.0f;
	// Stores the original friction
	private float m_fOriginalFriction = 0.0f;
	// Vertical Movement
	private float m_fVerticalMovement;
	// Horizontal Movement
	private float m_fHorizontalMovement;
	// Used to limit how fast the player can go
	public float m_fMaxVelocitySpeed;
	// Ground run speed
	public float m_fRunSpeed = 7.0f;
	// Ground walk speed
	public float m_fWalkSpeed = 3.0f;
	// Ground crouch speed
	public float m_fCrouchSpeed = 3.0f;
	// Ground prone speed
	public float m_fProneSpeed = 1.5f;
	// Ground accel
	public float m_fRunAcceleration = 14.0f;
	// Deacceleration that occurs when running on the ground
	public float m_fRunDeacceleration = 10.0f;
	// Air accel
	public float m_fAirAcceleration = 2.0f;
	// Deacceleration experienced when ooposite strafing
	public float m_fAirDecceleration = 2.0f;
	// How precise air control is
	public float m_fAirControl = 0.3f;
	// How fast acceleration occurs to get up to sideStrafeSpeed when
	public float m_fSideStrafeAcceleration = 50.0f;
	// What the max speed to generate when side strafing
	public float m_fSideStrafeSpeed = 1.0f;
	// The speed at which the character's up axis gains when hitting jump
	public float m_fJumpSpeed = 8.0f;
	// Used for sprinting
	private bool m_bRun = false;
	// Used to change the hitbox
	public CapsuleCollider m_standingHitBox;
	// Used to crouch
	private bool m_bCrouched = false;
	// Used to change the hitbox
	public CapsuleCollider m_crouchHitBox;
	// Used to set the camera up for crouch pos
	public float m_fCrouchCameraYOffset;
	// Used to prone
	private bool m_bProned = false;
	// Used to change the hitbox
	public CapsuleCollider m_proneHitBox;
	// Used to set the camera up for prone pos
	public float m_fProneCameraYOffset;
	// Used to check if no crouch, prone or sprint action is happening
	private bool m_bWalk = true;
	// Used to Check if the player cant stand
	private bool m_bCantStand = false;
	// Used to Check if the player cant crouch
	private bool m_bCantCrouch = false;
	// Used to check when the player is sprint sliding
	private bool m_bSlide = false;
	// Will be used for slopes
	private bool m_bCantProne;
    [Tooltip("If the player ends up on a slope which is greater then the Slope Limit as set on the character controller, then he will slide down.")]
    [SerializeField]
	// Used to make the player slide on slopes that are above the slope limit
    private bool m_bSlideWhenOverSlopeLimit = false;
    [Tooltip("If checked and the player is on an object tagged \"Slide\", he will slide down it regardless of the slope limit.")]
    [SerializeField]
	// Used to make the player slide on slopes with a tag
	private bool m_bSlideOnTaggedObjects = false;
    [Tooltip("How fast the player slides when on slopes as defined above.")]
    [SerializeField]
	// Used to set how fast the slide speed is
    private float m_fSlideSpeed = 12.0f;
	[Tooltip("How long a sprint slide wait goes for.")]
	[SerializeField]
	// Used to set how long a sprint slide wait goes for
	private float m_fSlideTime = 2.0f;
	// Used to get data from the object being hit in the raycast
	private RaycastHit m_Hit;
	// Used to set the slope limit the player can walk up
    private float m_fSlideLimit;
	// Used to check the max distance of the raycast for slopes
    private float m_fRayDistance;
	// Used for slide calculations and the contact point hit
	private Vector3 m_v3ContactPoint;
	// Used to store the move direction normalised for the movment
    private Vector3 m_v3MoveDirectionNorm = Vector3.zero;
	// Stores the player velocity allowing for the player to have momentum
	private Vector3 m_v3PlayerVelocity = Vector3.zero;
	// Stores the players max velocity
	private float m_fPlayerTopVelocity = 0.0f;
	// Used to get the character controller values
	private CharacterController FPSCC;

	//--------------------------------------------------------------------------------
	// Awake used for initialization.
	//--------------------------------------------------------------------------------
	void Awake()
	{
		// Hide and lock the cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		// Stores the original friction
		m_fOriginalFriction = m_fFriction;

		// Checks if the camera transform has been set
		if (m_cameraTransform == null)
		{
			// If the camera transform has not been set then use the main camera
			Camera mainCamera = Camera.main;
			if (mainCamera != null)
				m_cameraTransform = mainCamera.gameObject.transform;
		}

		// Moves the camera into a FPS postion in the player capsule
		m_cameraTransform.position = new Vector3(transform.position.x,
												 transform.position.y + m_fStandCameraYOffset,
												 transform.position.z);

		// Gets access to the player character controller
		FPSCC = GetComponent<CharacterController>();
		// Sets the ray distance based on the CC values
        m_fRayDistance = FPSCC.height * .5f + FPSCC.radius;
		// Sets the slide limit from the plater CC slope limit
        m_fSlideLimit = FPSCC.slopeLimit - .1f;
    }

	//--------------------------------------------------------------------------------
	// Update is called once every frame. It allows the player to move as well as
	// changes the FOV, locks the camera and hides the cursor, moves the camera, 
	// and limits the max speed the player can go.
	//--------------------------------------------------------------------------------
	private void Update()
	{
		// Sets the FOV of the camera
		Camera.main.fieldOfView = m_fFieldOfView;

		/* Ensure that the cursor is locked into the screen */
		if (Cursor.lockState != CursorLockMode.Locked)
		{
			if (Input.GetButtonDown("Fire1"))
				Cursor.lockState = CursorLockMode.Locked;
		}

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
		m_cameraTransform.rotation = Quaternion.Euler(m_fRotX, m_fRotY, 0);

		/* Calculate current velocity IDK if i can use this for locking the max velocity I THINK IT WAS USED FOR UI*/
		Vector3 udp = m_v3PlayerVelocity;
		udp.y = 0.0f;
        m_fPlayerTopVelocity = udp.magnitude;

		// If the player is grounded then do ground move otherwise use airmove as they are in the air
		if (FPSCC.isGrounded)
			GroundMove();
		else if (!FPSCC.isGrounded)
			AirMove();

		// Move the controller
		FPSCC.Move(m_v3PlayerVelocity * Time.deltaTime);

		// Checks what stance the player is in and moves the camera to fit it
		if (m_bCrouched)
		{
			// Set the camera's position to the transform
			SetCameraYPos(m_fCrouchCameraYOffset);
		}
		else if (m_bProned)
		{
			// Set the camera's position to the transform
			SetCameraYPos(m_fProneCameraYOffset);
		}
		else
		{
			// Set the camera's position to the transform
			SetCameraYPos(m_fStandCameraYOffset);
		}

		// Checks if the player is strafing left or right and limits the speed
		if (m_fVerticalMovement != 0 || m_fHorizontalMovement != 0)
		{
			if (m_v3PlayerVelocity.x >= m_fMaxVelocitySpeed)
			{
				m_v3PlayerVelocity.x = m_fMaxVelocitySpeed;
			}
			if (m_v3PlayerVelocity.x <= -m_fMaxVelocitySpeed)
			{
				m_v3PlayerVelocity.x = -m_fMaxVelocitySpeed;
			}
			if (m_v3PlayerVelocity.z >= m_fMaxVelocitySpeed)
			{
				m_v3PlayerVelocity.z = m_fMaxVelocitySpeed;
			}
			if (m_v3PlayerVelocity.z <= -m_fMaxVelocitySpeed)
			{
				m_v3PlayerVelocity.z = -m_fMaxVelocitySpeed;
			}
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
		// Used to calculation the acceleration of the vel in air
		float fDesVel = m_fAirAcceleration;
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
		ChangeStance(fDesSpeed, v3DesDir);

		v3DesDir.Normalize();
		// Sets the normalized move direction
		m_v3MoveDirectionNorm = v3DesDir;

		// Sets the a second desired speed to be used when not strafing
		float fDesSpeed2 = fDesSpeed;

		// Checks if the dot product of player vel and the des direction is less then zero 
		// And accelerates or deccelerates based on the result
		if (Vector3.Dot(m_v3PlayerVelocity, v3DesDir) < 0)
			fAccel = m_fAirDecceleration;
		else
			fAccel = m_fAirAcceleration;

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

		// Accelerates the player based on the values calculated above
		Accelerate(v3DesDir, fDesSpeed, fAccel);
		// If the air control is greater then zero then use the player air control movement
		if (m_fAirControl > 0)
			AirControl(v3DesDir, fDesSpeed2);

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
        m_v3PlayerVelocity.y = -m_fGravity * Time.deltaTime;

		// If the player is on a slope then makes it slide
        SlideDownSlope();
		// Changes the players stance from prone, crouch, sprint based on the input
		ChangeStance(fDesSpeed, v3DesDir);

		// Checks if the jump button is pressed and the player is not proned then jumps
        if (Input.GetButton("Jump") && !m_bProned)
		{
			m_v3PlayerVelocity.y = m_fJumpSpeed;
		}
		// Applies friction to the player to slow down their momentum
		ApplyFriction(1.0f);
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
		if (FPSCC.isGrounded)
		{
			control = speed < m_fRunDeacceleration ? m_fRunDeacceleration : speed;
			drop = control * m_fFriction * Time.deltaTime * t;
		}
		// Sets the new speed based on the speed and drop
		newspeed = speed - drop;
		// Sets the player friction the new speed
		m_fPlayerFriction = newspeed;
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
		m_cameraTransform.position = Vector3.Lerp(m_cameraTransform.position, m_v3NextCameraPos, m_fCameraChangeStanceSpeed);
	}

	//--------------------------------------------------------------------------------
	// Changes the stance of the player from crouch to prone, standing, sprinting and
	// sliding.
	//
	// Param:
	//		v3DesDir: The desired direction the player should move
	//		fDesSpeed: The desired speed the player should move at
	//--------------------------------------------------------------------------------
	private void ChangeStance(float fDesSpeed, Vector3 v3DesDir)
	{
		if (Input.GetButton("Crouch"))
		{
			if(!m_bCantCrouch)
			{
				m_bCrouched = true;
				m_bProned = false;
				m_bRun = false;
				m_bWalk = false;
			}
		}
		else if(m_bCantStand && !m_bCantCrouch && !m_bProned)
		{
			m_bCrouched = true;
		}
		else
		{
			m_bCrouched = false;
			m_bWalk = true;
		}
	
		if (Input.GetButtonDown("Prone"))
		{
			if (m_bCantStand || m_bCantCrouch)
			{
				m_bCrouched = false;
				m_bProned = true;
				m_bRun = false;
				m_bWalk = false;
			}
			else if (!m_bCantCrouch && m_bProned)
			{
				m_bProned = !m_bProned;
				m_bCrouched = true;
				m_bRun = false;
				m_bWalk = false;
			}
			else
			{
				m_bProned = !m_bProned;
				m_bCrouched = false;
				m_bRun = false;
				m_bWalk = !m_bWalk;
			}
		}

		if (Input.GetButton("Sprint"))
		{
			if(!m_bCantStand && !m_bProned)
			{
				m_bRun = true;
				m_bCrouched = false;
				m_bProned = false;
				m_bWalk = false;
			}
		}
		else
		{
			if (!m_bCantStand && !m_bProned)
			{
				m_bRun = false;
				m_bWalk = true;
			}
			else if(!m_bCantStand && !m_bCrouched)
			{
				m_bRun = false;
				m_bWalk = true;
			}
		}
        if (Input.GetButton("Sprint") && Input.GetButtonDown("Crouch"))
        {
            if (!m_bCantCrouch)
            {
                m_bRun = false;
                m_bCrouched = true;
                m_bSlide = true;
                m_bProned = false;
                m_bWalk = false;
            }
        }
        else if (m_bCantStand && !m_bCantCrouch && !m_bProned)
        {
            m_bCrouched = true;
        }
        else if (m_bSlide)
        {
            m_bCrouched = true;
            m_bWalk = false;
        }

        if (m_bCrouched)
		{
			m_standingHitBox.enabled = true;
			m_crouchHitBox.enabled = false;
			m_proneHitBox.enabled = false;

			// Need to make the capsule colliders and CC same hieght and center then detect if they can un crouch.
			FPSCC.center = m_crouchHitBox.center;
			FPSCC.height = m_crouchHitBox.height;

            if(m_bSlide)
            {
                fDesSpeed *= m_fCrouchSpeed;

                Accelerate(v3DesDir, fDesSpeed, m_fRunAcceleration);

                StartCoroutine("Slide");
            }
            else
            {
                fDesSpeed *= m_fCrouchSpeed;

                Accelerate(v3DesDir, fDesSpeed, m_fRunAcceleration);

                m_fFriction = m_fOriginalFriction;
            }
		}
		else if (m_bProned)
		{
			m_standingHitBox.enabled = false;
			m_crouchHitBox.enabled = true;
			m_proneHitBox.enabled = false;

			m_bCantStand = false;

			FPSCC.center = m_proneHitBox.center;
			FPSCC.height = m_proneHitBox.height;

			fDesSpeed *= m_fProneSpeed;

			Accelerate(v3DesDir, fDesSpeed, m_fRunAcceleration);

			m_fFriction = 1.0f;
		}
		else if (m_bRun)
		{
			m_standingHitBox.enabled = true;
			m_crouchHitBox.enabled = false;
			m_proneHitBox.enabled = false;

			FPSCC.center = m_standingHitBox.center;
			FPSCC.height = m_standingHitBox.height;

			fDesSpeed *= m_fRunSpeed;

			Accelerate(v3DesDir, fDesSpeed, m_fRunAcceleration);

			m_fFriction = m_fOriginalFriction;
		}
		else
		{
			m_standingHitBox.enabled = true;
			m_crouchHitBox.enabled = false;
			m_proneHitBox.enabled = false;

			FPSCC.center = m_standingHitBox.center;
			FPSCC.height = m_standingHitBox.height;

			fDesSpeed *= m_fWalkSpeed;

			Accelerate(v3DesDir, fDesSpeed, m_fRunAcceleration);

			m_fFriction = m_fOriginalFriction;
		}
	}

	//--------------------------------------------------------------------------------
	// Checks if the player is on a slope above the slope limit or if the player is
	// on an object tagged "Slide" and makes the player slide down the object.
	//--------------------------------------------------------------------------------
	private void SlideDownSlope()
    {
        bool sliding = false;
        // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
        // because that interferes with step climbing amongst other annoyances
        if (Physics.Raycast(transform.position, Vector3.down, out m_Hit, m_fRayDistance))
        {
            if (Vector3.Angle(m_Hit.normal, Vector3.up) > m_fSlideLimit)
            {
                sliding = true;
            }
        }
        // However, just raycasting straight down from the center can fail when on steep slopes
        // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
        else
        {
            Physics.Raycast(m_v3ContactPoint + Vector3.up, -Vector3.up, out m_Hit);
            if (Vector3.Angle(m_Hit.normal, Vector3.up) > m_fSlideLimit)
            {
                sliding = true;
            }
        }
		
		// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
		if ((sliding && m_bSlideWhenOverSlopeLimit) || (m_bSlideOnTaggedObjects && m_Hit.collider.tag == "Slide"))
        {
            Vector3 hitNormal = m_Hit.normal;
            m_v3PlayerVelocity = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
            Vector3.OrthoNormalize(ref hitNormal, ref m_v3PlayerVelocity);
            m_v3PlayerVelocity *= m_fSlideSpeed;
        }
	}

	//--------------------------------------------------------------------------------
	// Store point that we're in contact with for use in FixedUpdate if needed
	//
	// Param:
	//		hit: used to find the hit point of the colliding objects.
	//
	//--------------------------------------------------------------------------------
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		m_v3ContactPoint = hit.point;
	}

	//--------------------------------------------------------------------------------
	// Changes the players friction when sliding so the player can move further and
	// not lose as much momentum, waits a certain amount of time before stopping the
	// slide.
	//--------------------------------------------------------------------------------
	IEnumerator Slide()
    {
        m_fFriction = 0.5f;
        yield return new WaitForSeconds(m_fSlideTime);
        m_bSlide = false;
        m_bCrouched = true;
    }

	//--------------------------------------------------------------------------------
	// OnTriggerStay checks if anything but the player is being triggered and prevents
	// the ability to crouch or stand based on the which bools are active and if there
	// is a rigid body, if there is a rigid body and it is not kinematic then it 
	// should be able to get pushed of the player.
	//
	// Param:
	//		other: used to find the other colliding object.
	//
	//--------------------------------------------------------------------------------
	private void OnTriggerStay(Collider other)
	{
		if (other.tag != "Player")
		{
			Rigidbody RB = other.gameObject.GetComponent<Rigidbody>();
			if(RB != null)
			{
				if (RB.isKinematic)
				{
					if (m_bCrouched)
					{
						m_bCantCrouch = false;
						m_bCantProne = false;
						m_bCantStand = true;
					}
					else if (m_bProned)
					{
						m_bCantCrouch = true;
						m_bCantProne = false;
						m_bCantStand = true;
					}
					else if (m_bWalk)
					{
						m_bCantStand = true;
						m_bCantCrouch = false;
						m_bCantProne = false;
					}
				}
				else
				{
					m_bCantStand = false;
					m_bCantCrouch = false;
					m_bCantProne = false;
				}
			}
			else
			{// Needs fixing does not crouch under objects with no rigid body
				if (m_bCrouched)
				{
					m_bCantCrouch = false;
					m_bCantProne = false;
					m_bCantStand = true;
				}
				else if (m_bProned)
				{
					m_bCantCrouch = true;
					m_bCantProne = false;
					m_bCantStand = true;
				}
				else if (m_bWalk)
				{
					m_bCantStand = true;
					m_bCantCrouch = false;
					m_bCantProne = false;
				}
			}
		}
	}

	//--------------------------------------------------------------------------------
	// OnTriggerExit checks if the other object is not the player and sets the stance
	// bools back to flase as there is nothing preventing them from changing stance.
	//
	// Param:
	//		other: used to find the other colliding object.
	//
	//--------------------------------------------------------------------------------
	private void OnTriggerExit(Collider other)
	{
		if (other.tag != "Player")
		{
			m_bCantCrouch = false;
			m_bCantProne = false;
			m_bCantStand = false;
		}
	}
}
