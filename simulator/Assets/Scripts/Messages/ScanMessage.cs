using System;
using UnityEngine;

[Serializable]
public class ScanMessage {
	
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;
	public string type;
	public string team;

	public bool isFiring;
	public string name;
	public float maxHp;
	public float hp;
	public float maxHeat;
	public float heat;
}