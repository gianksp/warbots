using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : BaseObject {


	public GameObject bulletPrefab;
	private bool _canShoot = true;

	public Bot source;
	public float damage = 2;
	public float range =100;

	// Use this for initialization
	void Start () {
		source = transform.parent.gameObject.GetComponent<Bot> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Fire() {
		StartCoroutine ("Shoot");
	}

	IEnumerator Shoot()
	{

		if (_canShoot) {
			_canShoot = false;
			// Create the Bullet from the Bullet Prefab
			var bullet = (GameObject)Instantiate (
				            bulletPrefab,
				            transform.position,
				            transform.rotation);

			//Tag source and modifier for damage
			Bullet meta = (Bullet)bullet.GetComponent<Bullet>();
			meta.origin = source;
			meta.damage = damage;

			// Add velocity to the bullet
			var rb = bullet.GetComponent<Rigidbody> ();
			rb.AddForce (transform.forward * 4, ForceMode.Impulse);

			// Destroy the bullet after 2 seconds
			Destroy (bullet, 4.0f); 
			yield return new WaitForSeconds (0.25f);
			_canShoot = true;
		}
	}
}
