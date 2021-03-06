using UnityEngine;
using System.Collections;

public class ShootingController : MonoBehaviour {
	
	// Shoot a bullet every N seconds
	public float reloadInterval = 3.0f;
	
	public float safetyOnDistance = 20;
	
	// Bullet to shoot
	public GameObject bulletPrefab = null;
	
	// Movement speed of bullet
	public float speed = 500.0f;
	
	// How long until we next shoot
	protected float intervalRemaining;
	
	/// <summary>
	/// Backing field for Active property
	/// </summary>
	public bool isActive = false;
	
	/// <summary>
	/// Is this shooter active (shooting)?
	/// </summary>
	public bool Active {
		get { return isActive; }
		set {
			isActive = value;
			intervalRemaining = Random.Range(0.0f, reloadInterval);
		}
	}

	// Use this for initialization
	public virtual void Start () {
		intervalRemaining = Random.Range(0.0f, reloadInterval);
			
		if (bulletPrefab == null) {
			bulletPrefab = ((PrefabHack)(Camera.mainCamera.GetComponent("PrefabHack"))).Bullet;
		}
	}
	
	// Update is called once per frame, and activates ourself if we've reached the right side of the screen
	// or, if we're already active, considers shooting. 
	//
	// Don't shoot! I'm innocent.
	void Update () {
		if (isActive) {
			if (intervalRemaining > 0.0f) 
			{
				intervalRemaining -= Time.deltaTime;
			} else if (transform.position.x < safetyOnDistance)
			{
				// Shoot
				Shoot();
				intervalRemaining = this.reloadInterval;
			}
		} else {
			if (transform.position.x < 19.9 && !isActive) {
				this.Active = true;
			}
		}
	}
	
	public virtual void Shoot() {
		GameObject bullet = (GameObject)Instantiate(bulletPrefab,this.transform.position,Quaternion.identity);
		bullet.rigidbody.AddForce(new Vector3(-1.0f, 0.0f, 0.0f)*speed);
	}
}
