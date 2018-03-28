using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour {

	public enum BaseObjectType {BOT,OBSTACLE,PROJECTILE,GROUND,BOUNDARY}
	public enum TeamColor {RED,BLUE}

	public string nameLabel;
	public float weight = 1f;
	public float armor = 0f;
}
