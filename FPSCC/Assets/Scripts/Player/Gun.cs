using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public float m_damage = 10.0f;
    public float m_range = 100.0f;
    public float m_hitForce = 30.0f;
    public float m_fireRate = 15.0f;

    public int m_selectedMode = 0;

    public Camera m_fpsCamera;
    public ParticleSystem m_muzzleFlash;
    public GameObject m_impactEffect;

    private float m_nextTimeToFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetButton("Fire1") && Time.time >= m_nextTimeToFire)
        {
            m_nextTimeToFire = Time.time + 1.0f / m_fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        m_muzzleFlash.Play();
        RaycastHit _hit;
        if(Physics.Raycast(m_fpsCamera.transform.position, m_fpsCamera.transform.forward, out _hit, m_range))
        {
			if (_hit.rigidbody != null)
			{
				_hit.rigidbody.AddForce(-_hit.normal * m_hitForce);

			}

			GameObject _impactGO = Instantiate(m_impactEffect, _hit.point, Quaternion.LookRotation(_hit.normal));
			Destroy(_impactGO, 2.0f);
		}
	}
}
