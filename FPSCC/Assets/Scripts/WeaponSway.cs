using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

	public float m_fAmount;
	public float m_fMaxAmount;
	public float m_fSmoothAmount;

	private Vector3 m_v3InitialPosition;


	// Start is called before the first frame update
	void Start()
    {
		m_v3InitialPosition = transform.localPosition;
	}

	// Update is called once per frame
	void Update()
	{
		float fMovementX = -Input.GetAxis("Mouse X") * m_fAmount;
		float fMovementY = -Input.GetAxis("Mouse Y") * m_fAmount;
		fMovementX = Mathf.Clamp(fMovementX, -m_fMaxAmount, m_fMaxAmount);
		fMovementY = Mathf.Clamp(fMovementY, -m_fMaxAmount, m_fMaxAmount);

		Vector3 v3FinalPosition = new Vector3(fMovementX, fMovementY, 0);
		transform.localPosition = Vector3.Lerp(transform.localPosition, v3FinalPosition + m_v3InitialPosition, Time.deltaTime * m_fSmoothAmount);
	}
}
