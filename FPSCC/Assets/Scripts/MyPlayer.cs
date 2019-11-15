using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour
{

	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	private bool grounded = false;

	// Transform of the camera
	public Transform m_cameraTransform;
	// The height at which the camera is bound to
	public float m_fStandCameraYOffset = 0.6f;
	// Mouse X Sensitivity
	public float m_fXMouseSensitivity = 30.0f;
	// Mouse Y Sensitivity
	public float m_fYMouseSensitivity = 30.0f;
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

	// Movement stuff
	private float m_fVerticalMovement;
	private float m_fHorizontalMovement;
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

	private bool m_bCantStand = false;

	private bool m_bCantCrouch = false;

    private bool m_bSlide = false;

	// will be used for slopes
	private bool m_bCantProne;

	// Q3: players can queue the next jump just before he hits the ground
	private bool m_bWishJump = false;

    [Tooltip("If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down.")]
    [SerializeField]
    private bool m_SlideWhenOverSlopeLimit = false;

    [Tooltip("If checked and the player is on an object tagged \"Slide\", he will slide down it regardless of the slope limit.")]
    [SerializeField]
    private bool m_SlideOnTaggedObjects = false;

    [Tooltip("How fast the player slides when on slopes as defined above.")]
    [SerializeField]
    private float m_SlideSpeed = 12.0f;


    private RaycastHit m_Hit;
    private float m_FallStartLevel;
    private bool m_Falling;
    private float m_SlideLimit;
    private float m_RayDistance;
    private Vector3 m_ContactPoint;
    private bool m_PlayerControl = false;

    private Vector3 m_v3MoveDirectionNorm = Vector3.zero;
	private Vector3 m_v3PlayerVelocity = Vector3.zero;
	private float m_fPlayerTopVelocity = 0.0f;

	private CharacterController FPSCC;

	void Awake()
	{
		// Hide and lock the cursor
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

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

		FPSCC = GetComponent<CharacterController>();

        m_RayDistance = FPSCC.height * .5f + FPSCC.radius;
        m_SlideLimit = FPSCC.slopeLimit - .1f;

    }

	private void Update()
	{
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

		/* Calculate top velocity IDK if i can use this for locking the max velocity I THINK IT WAS USED FOR UI*/
		Vector3 udp = m_v3PlayerVelocity;
		udp.y = 0.0f;
		//if (udp.magnitude > m_fPlayerTopVelocity)
            m_fPlayerTopVelocity = udp.magnitude;

		/* Movement, here's the important part */
		// que jump checks if space is held down after each jump which ques the jump as the player is wishing to jump it should not que just jump when they are grounded and space is pressed.
		QueueJump();
		if (FPSCC.isGrounded)
			GroundMove();
		else if (!FPSCC.isGrounded)
			AirMove();

		// Move the controller
		FPSCC.Move(m_v3PlayerVelocity * Time.deltaTime);

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

	}


	void FixedUpdate()
	{

	}

	private void SetMoveDir()
	{
		m_fHorizontalMovement = Input.GetAxisRaw("Vertical");
		m_fVerticalMovement = Input.GetAxisRaw("Horizontal");
	}

	private void QueueJump()
	{
		if (Input.GetButton("Jump") && !m_bWishJump)
			m_bWishJump = true;
		if (Input.GetButtonUp("Jump"))
			m_bWishJump = false;
	}
	// Wierd diagnol movement from desDir need to maybe disable forward movement while strafing or some how stop the fast speed increase.
	private void AirMove()
	{
		Vector3 v3DesDir;
		float fDesVel = m_fAirAcceleration;
		float fAccel;

		SetMoveDir();

		v3DesDir = new Vector3(m_fVerticalMovement, 0, m_fHorizontalMovement);

		v3DesDir = transform.TransformDirection(v3DesDir);

		float fDesSpeed = v3DesDir.magnitude;

		ChangeStance(fDesSpeed, v3DesDir);

		v3DesDir.Normalize();
		m_v3MoveDirectionNorm = v3DesDir;

		// CPM: Aircontrol
		float fDesSpeed2 = fDesSpeed;
		if (Vector3.Dot(m_v3PlayerVelocity, v3DesDir) < 0)
			fAccel = m_fAirDecceleration;
		else
			fAccel = m_fAirAcceleration;
		// If the player is ONLY strafing left or right
		if (m_fVerticalMovement == 0 && m_fHorizontalMovement != 0)
		{
			if (fDesSpeed > m_fSideStrafeSpeed)
				fDesSpeed = m_fSideStrafeSpeed;
			fAccel = m_fSideStrafeAcceleration;
			Debug.Log(m_v3PlayerVelocity);
		}

		// If the player is ONLY strafing forward or backward
		if (m_fVerticalMovement != 0 && m_fHorizontalMovement == 0)
		{
			if (fDesSpeed > m_fSideStrafeSpeed)
				fDesSpeed = m_fSideStrafeSpeed;
			fAccel = m_fSideStrafeAcceleration;
			Debug.Log(m_v3PlayerVelocity);
		}

		Accelerate(v3DesDir, fDesSpeed, fAccel);
		if (m_fAirControl > 0)
			AirControl(v3DesDir, fDesSpeed2);
		// !CPM: Aircontrol

		// Apply gravity
		m_v3PlayerVelocity.y -= m_fGravity * Time.deltaTime;

	}


	private void AirControl(Vector3 v3DesDir, float fDesSpeed)
	{
		float zspeed;
		float speed;
		float dot;
		float k;

		// Can't control movement if not moving forward or backward
		if (Mathf.Abs(m_fVerticalMovement) < 0.001 || Mathf.Abs(fDesSpeed) < 0.001)
			return;
		zspeed = m_v3PlayerVelocity.y;
		m_v3PlayerVelocity.y = 0;
		/* Next two lines are equivalent to idTech's VectorNormalize() */
		speed = m_v3PlayerVelocity.magnitude;
		m_v3PlayerVelocity.Normalize();

		dot = Vector3.Dot(m_v3PlayerVelocity, v3DesDir);
		k = 32;
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

		m_v3PlayerVelocity.x *= speed;
		m_v3PlayerVelocity.y = zspeed; // Note this line
		m_v3PlayerVelocity.z *= speed;

		// Stops the player moving extremly fast by limiting the speed
		if (m_fVerticalMovement != 0 && m_fHorizontalMovement != 0)
		{
			if (m_v3PlayerVelocity.x >= 10)
			{
				m_v3PlayerVelocity.x = 10;
			}
			if (m_v3PlayerVelocity.x <= -10)
			{
				m_v3PlayerVelocity.x = -10;
			}
			if (m_v3PlayerVelocity.z >= 10)
			{
				m_v3PlayerVelocity.z = 10;
			}
			if (m_v3PlayerVelocity.z <= -10)
			{
				m_v3PlayerVelocity.z = -10;
			}
		}

	}

	private void GroundMove()
	{
		Vector3 v3DesDir;

		SetMoveDir();

		v3DesDir = new Vector3(m_fVerticalMovement, 0, m_fHorizontalMovement);
		v3DesDir = transform.TransformDirection(v3DesDir);
		v3DesDir.Normalize();
		m_v3MoveDirectionNorm = v3DesDir;

		var fDesSpeed = v3DesDir.magnitude;

		

        // Reset the gravity velocity
        m_v3PlayerVelocity.y = -m_fGravity * Time.deltaTime;

        SlideDownSlope();

        ChangeStance(fDesSpeed, v3DesDir);

        if (Input.GetButton("Jump") && !m_bProned)
		{
			m_v3PlayerVelocity.y = m_fJumpSpeed;
			// m_bWishJump = false;
		}

		// Do not apply friction if the player is queueing up the next jump
		if (!m_bWishJump)
		{
			ApplyFriction(1.0f);
		}
		else
		{
			ApplyFriction(0);
		}

	}

	private void ApplyFriction(float t)
	{
		Vector3 vec = m_v3PlayerVelocity;
		float speed;
		float newspeed;
		float control;
		float drop;

		vec.y = 0.0f;
		speed = vec.magnitude;
		drop = 0.0f;

		/* Only if the player is on the ground then apply friction */
		if (FPSCC.isGrounded)
		{
			control = speed < m_fRunDeacceleration ? m_fRunDeacceleration : speed;
			drop = control * m_fFriction * Time.deltaTime * t;
		}

		newspeed = speed - drop;
		m_fPlayerFriction = newspeed;
		if (newspeed < 0)
			newspeed = 0;
		if (speed > 0)
			newspeed /= speed;

		m_v3PlayerVelocity.x *= newspeed;
		m_v3PlayerVelocity.z *= newspeed;
	}

	private void Accelerate(Vector3 v3DesDir, float fDesSpeed, float fAccel)
	{
		float addspeed;
		float accelspeed;
		float currentspeed;

		currentspeed = Vector3.Dot(m_v3PlayerVelocity, v3DesDir);
		addspeed = fDesSpeed - currentspeed;
		if (addspeed <= 0)
			return;
		accelspeed = fAccel * Time.deltaTime * fDesSpeed;
		if (accelspeed > addspeed)
			accelspeed = addspeed;

		m_v3PlayerVelocity.x += accelspeed * v3DesDir.x;
		m_v3PlayerVelocity.z += accelspeed * v3DesDir.z;
	}

	private void SetCameraYPos(float yOffset)
	{
		// Moves the camera into a FPS desired y offset postion in the player capsule
		m_cameraTransform.position = new Vector3(transform.position.x,
												 transform.position.y + yOffset,
												 transform.position.z);
	}

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
        else
        {
            m_bCrouched = false;
            m_bWalk = true;
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

    private void SlideDownSlope()
    {
        bool sliding = false;
        // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
        // because that interferes with step climbing amongst other annoyances
        if (Physics.Raycast(transform.position, Vector3.down, out m_Hit, m_RayDistance))
        {
            if (Vector3.Angle(m_Hit.normal, Vector3.up) > m_SlideLimit)
            {
                sliding = true;
            }
        }
        // However, just raycasting straight down from the center can fail when on steep slopes
        // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
        else
        {
            Physics.Raycast(m_ContactPoint + Vector3.up, -Vector3.up, out m_Hit);
            if (Vector3.Angle(m_Hit.normal, Vector3.up) > m_SlideLimit)
            {
                sliding = true;
            }
        }

        // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
        if ((sliding && m_SlideWhenOverSlopeLimit) || (m_SlideOnTaggedObjects && m_Hit.collider.tag == "Slide"))
        {
            Vector3 hitNormal = m_Hit.normal;
            m_v3PlayerVelocity = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
            Vector3.OrthoNormalize(ref hitNormal, ref m_v3PlayerVelocity);
            m_v3PlayerVelocity *= m_SlideSpeed;
            m_PlayerControl = false;
        }
        // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
        //else if(m_bRun)
        //{
        //    m_v3PlayerVelocity = new Vector3(m_fVerticalMovement, -0.75f, m_fHorizontalMovement);
        //    m_v3PlayerVelocity = transform.TransformDirection(m_v3PlayerVelocity) * m_fRunSpeed;
        //    m_PlayerControl = true;
        //}
        //else
        //{
        //    m_v3PlayerVelocity = new Vector3(m_fVerticalMovement, -0.75f, m_fHorizontalMovement);
        //    m_v3PlayerVelocity = transform.TransformDirection(m_v3PlayerVelocity) * m_fWalkSpeed;
        //    m_PlayerControl = true;
        //}
    }

    IEnumerator Slide()
    {
        m_fFriction = 0.5f;
        yield return new WaitForSeconds(2.0f);
        m_bSlide = false;
        m_bCrouched = true;
    }

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
			}
			else
			{
				m_bCantStand = false;
				m_bCantCrouch = false;
				m_bCantProne = false;
			}
		}
	}
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
