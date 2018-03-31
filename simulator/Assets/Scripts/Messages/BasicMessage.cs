using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMessage : MonoBehaviour {

	public enum MatchEvent { PREPARING, START, END, INPROGRESS };  
	public enum RadarEvent { SCAN, HIT, KILL }
	public enum BotEvent { STATUS, POSITION, ROTATION, HP, HEAT, COLLIDE, FORCE, TORQUE };  
	public enum BotAction { SHOOT, FORCE, TORQUE }; 

	public enum Scannable { PLAYER, OBSTACLE, BULLET, GROUND, BOUNDARY }; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
