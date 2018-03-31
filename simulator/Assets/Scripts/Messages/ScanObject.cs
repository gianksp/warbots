using System;
using UnityEngine;

[Serializable]
public class ScanObject {
	
	public Vector3 position;
	public Vector3 rotation;
	public Vector3 velocity;
	public Vector3 angularVelocity;
	public int id;
	public float hp;
	public float heat;
	public string eventType;
}