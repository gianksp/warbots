using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frame : BaseObject {

	public float maxHeat = 10f;
	public float currentHeat;

	public float maxHp = 10f;
	public float currentHp ;

	// Use this for initialization
	void Start () {
		currentHeat = maxHeat;
		currentHp = maxHp;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
