using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BaseObject {

	public Bot origin;
	public float damage = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision) 
	{
		if (collision.transform.tag == "Player") {
			Bot destination = collision.gameObject.GetComponent<Bot> ();
			destination.OnHit (origin, this);
		}
		Destroy (this);
	}
}
