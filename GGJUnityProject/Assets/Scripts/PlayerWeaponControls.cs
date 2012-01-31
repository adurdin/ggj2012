using UnityEngine;
using System.Collections;

public class PlayerWeaponControls : MonoBehaviour 
{
	public GameObject currentPlayerWeapon;

	void Start () 
	{
	}
	
	void Update () 
	{
		PlayerWeapon playerWeapon = ((PlayerWeapon)currentPlayerWeapon.GetComponent("PlayerWeapon"));
		if((Input.GetKey("space") == true || VirtualButtonRegion.VBpressed) && playerWeapon.CanFire())
		{
			playerWeapon.Fire(transform.position, Vector3.right, transform.rotation);
			
		}
	}
}
