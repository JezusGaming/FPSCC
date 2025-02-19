using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwap : MonoBehaviour
{
    public int m_selectedWeapon = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        SwapWeapon();
    }

    void SwapWeapon()
    {
        int _previousWeapon = m_selectedWeapon;

  //      if(Input.GetAxis("Mouse ScrollWheel") > 0.0f)
  //      {
  //          if(m_selectedWeapon >= transform.childCount - 1)
  //          {
  //              m_selectedWeapon = 0;
		//    }
  //          else
  //          {
  //              m_selectedWeapon++;
  //          }
		//}
  //      if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
  //      {
  //          if (m_selectedWeapon <= 0)
  //          {
  //              m_selectedWeapon = transform.childCount - 1;
  //          }
  //          else
  //          {
  //              m_selectedWeapon--;
  //          }
  //      }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_selectedWeapon = 0;
		}
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            m_selectedWeapon = 1;
        }

        if (_previousWeapon != m_selectedWeapon)
        {
            SelectWeapon();
		}
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach(Transform weapon in transform)
        {

            if(i == m_selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
		    }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
	}

}
