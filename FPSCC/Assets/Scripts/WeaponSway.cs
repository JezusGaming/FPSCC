using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
	// Used for the amount of sway
	public float m_fAmount;
	// Used for the max amount of sway that can occur
	public float m_fMaxAmount;
	// Used for how smooth the sway will happen
	public float m_fSmoothAmount;
	// Stores the initial position
	private Vector3 m_v3InitialPosition;
	// Stores the initial rotation
	private Quaternion m_qInitialRotation;
	// Used to detect objects with rigidbodies and shoot/push them
	private RaycastHit Hit;
	// Used to find lerp the sprint sway
	private Vector3 LerpSway;
	// Used to change the time of the sprint lerp sway
	public float m_fLerpTime;
	// Sets where the sprints lerp sway ends
	public float m_fSprintTilt = 20.0f;
	// Used to get access to the player script
	public GameObject m_player;
	private MyPlayer m_playerScript;

	//--------------------------------------------------------------------------------
	// start used for initialization.
	//--------------------------------------------------------------------------------
	void Start()
    {
		m_playerScript = m_player.GetComponent<MyPlayer>();
		m_v3InitialPosition = transform.localPosition;
		m_qInitialRotation = transform.localRotation;
	}

	//--------------------------------------------------------------------------------
	// Update is called once every frame. It makes the weapon sway depending on the 
	// values it can sway a lot or little.
	//--------------------------------------------------------------------------------
	void Update()
	{
		
		// Sets fMovementX and fMovementY based on the mouse input
		float fMovementX = -Input.GetAxis("Mouse X") * m_fAmount;
		float fMovementY = -Input.GetAxis("Mouse Y") * m_fAmount;
		// Clampes the movement based on the max amount
		fMovementX = Mathf.Clamp(fMovementX, -m_fMaxAmount, m_fMaxAmount);
		fMovementY = Mathf.Clamp(fMovementY, -m_fMaxAmount, m_fMaxAmount);
		// Lerps between the final pos and inital pos by the smooth amount
		Vector3 v3FinalPosition = new Vector3(fMovementX, fMovementY, 0);
		transform.localPosition = Vector3.Lerp(transform.localPosition, v3FinalPosition + m_v3InitialPosition, Time.deltaTime * m_fSmoothAmount);

			Vector3 fwd = transform.TransformDirection(Vector3.left);
		// Shoots a rigid body object pushing them over
		if (Input.GetMouseButtonDown(0))
		{

			if (Physics.Raycast(transform.position, fwd, out Hit, 50))
			{
				//print("There is something in front of the object!");
				if (Hit.rigidbody)
					Hit.rigidbody.velocity = fwd * 20;
			}
		}
		if(!m_playerScript.m_bSprintToWalk)
		{
			if (Input.GetButton("Sprint"))
			{
				LerpSway = new Vector3(transform.localRotation.x, 90.0f, m_fSprintTilt);
				transform.localEulerAngles = Vector3.Lerp(transform.localRotation.eulerAngles, LerpSway, m_fLerpTime * Time.deltaTime);
			}
			else
			{
				//transform.localRotation = m_qInitialRotation;
				transform.localEulerAngles = Vector3.Lerp(transform.localRotation.eulerAngles, m_qInitialRotation.eulerAngles, Time.deltaTime);
			}
		}
	}
}
