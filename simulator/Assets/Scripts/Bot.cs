using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bot : MonoBehaviour
{

	public enum Team {RED, BLUE};

	float torque = 0f;
	float power = 100f;
	float hp = 10;

	public Team team = Team.RED;


	public string name;
	public string username;

	private Transform target;

	public Weapon primaryWeapon;
	public Weapon secondaryWeapon;

	public Radar radar;
	public Frame frame;

	//State
	private Vector3 __currentPos;
	private Quaternion __currentRot;
	private bool __currentFire;
	private float __currentHeat;
	private float __currentHp;

	//Controllers
	private Vector3 __applyForce = Vector3.zero;
	private Vector3 __applyTorque = Vector3.zero;
	private bool __isFiring = false;

	private Rigidbody __rb;

	private BasicMessage.MatchEvent __state = BasicMessage.MatchEvent.PREPARING;

	/// <summary>
	/// Bootstrap this bot by username/botname, search in Warbots database and load all basic configuration including
	/// - Radar information
	/// - Primary weapon information
	/// - Secondary weapon information
	/// - Frame information
	/// </summary>
	void Awake ()
	{
		__rb = gameObject.GetComponent<Rigidbody> ();
	}

	/// <summary>
	/// Initialise
	/// </summary>
	void Start ()
	{
		__currentPos = transform.position;
		__currentRot = transform.rotation;
		__currentFire = false;
	}

	public void OnScan(GameObject obj)
	{
		//Build the radar scan info object
		ScanMessage msg = new ScanMessage();

		try {
			//3 tags are eval here? player = bot, bullets, obstacles
			BasicMessage.Scannable action = (BasicMessage.Scannable)Enum.Parse(typeof(BasicMessage.Scannable),obj.transform.tag.ToUpper());
			msg.position = obj.transform.position;
			msg.rotation = obj.transform.rotation;
			msg.name = obj.transform.tag;
			switch (action) {
			case BasicMessage.Scannable.PLAYER:
				msg.type = BaseObject.BaseObjectType.BOT.ToString();
				Bot ebot = obj.GetComponent<Bot>();
				msg.team = ebot.team.ToString();
				msg.heat = ebot.__currentHeat;
				msg.isFiring = ebot.__isFiring;
				msg.maxHeat = ebot.frame.maxHeat;
				msg.maxHp = ebot.frame.maxHp;
				Link.SendScanMessage(msg);
				break;
			case BasicMessage.Scannable.BULLET:
				msg.type = BaseObject.BaseObjectType.PROJECTILE.ToString();
				Link.SendScanMessage(msg);
				break;
			case BasicMessage.Scannable.OBSTACLE:
				msg.type = BaseObject.BaseObjectType.OBSTACLE.ToString();
				Link.SendScanMessage(msg);
				break;
			case BasicMessage.Scannable.BOUNDARY:
				msg.type = BaseObject.BaseObjectType.BOUNDARY.ToString();
				Link.SendScanMessage(msg);
				break;
			}
		} catch(Exception ex) {
		}
	}

	public void OnEnemyHit(Bot source) {

	}

	public void OnEnemyKill(Bot source) {

	}

	public void OnHit (Bot source, Bullet projectile)
	{
		Debug.Log (this.name+" on team "+this.team.ToString()+" has been hit by "+source.name+" by "+projectile.damage.ToString()+" from team ");
		this.hp -= projectile.damage;
		if (this.hp <= 0) {
			Destroy (this);
		}
	}
		
	public void OnDeath() {
	
	}

	public void OnTempChange(float old, float actual) {
		
	}

	/// <summary>
	/// Handle physics
	/// </summary>
	void LateUpdate ()
	{
		/************************* DOWNSTREAM SIMULATOR -> CLIENT ***********************/

		//Game state changed
		// if (__state == BasicMessage.MatchEvent.PREPARING) {
		// 	__state = BasicMessage.MatchEvent.START;
		// 	MatchStateMessage msg = new MatchStateMessage ();
		// 	msg.state = __state;
		// 	Link.SendMatchUpdateMessage(msg);
		// }

		//Position downstream
		if (__currentPos != transform.position) {
			__currentPos = transform.position;
			Link.SendPositionUpdateMessage (__currentPos);
		}

		//Rotation downstream
		if (__currentRot != transform.rotation) {
			__currentRot = transform.rotation;
			Link.SendRotationUpdateMessage (__currentRot);
		}

		/************************* UPSTREAM CLIENT -> SIMULATOR ***********************/

		//Fire upstream
		if (__isFiring) {
			primaryWeapon.Fire ();
		}

		//Force upstream
		if (__applyForce != Vector3.zero) {
			__rb.AddRelativeForce (__applyForce, ForceMode.Force);
			__applyForce = Vector3.zero;
		}

		//Torque upstream
		if (__applyTorque != Vector3.zero) {
			__rb.AddRelativeTorque (__applyTorque, ForceMode.Force);
			__applyTorque = Vector3.zero;
		}
	}
		
	public void ApplyForce(Vector3 vector) {
		__applyForce = vector;
	}

	public void ApplyTorque(Vector3 vector) {
		__applyTorque = vector;
	}

	public void SetFiring(bool firing) {
		__isFiring = firing;
	}
		
}
