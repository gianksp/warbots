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


	public string nameLabel;
	public string username;

	private Transform target;

	public Weapon primaryWeapon;
	public Weapon secondaryWeapon;

	public Radar radar;
	public Frame frame;

	private List<ScanObject> __scanned;

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

	private FinishLine _finishLine;

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

	public void Heat() {
		__currentHeat+=1;
	}

	public float GetHeat() {
		return __currentHeat;
	}

	public void Cooldown() {
		if (__currentHeat > 0) {
			__currentHeat-=1;
		}
	}

	/// <summary>
	/// Initialise
	/// </summary>
	void Start ()
	{
		__currentPos = transform.position;
		__currentRot = transform.rotation;
		__currentFire = false;
		__scanned = new List<ScanObject>();

		InvokeRepeating("UpdateBot",0f,0.5f);
    	InvokeRepeating("Cooldown", 1.0f, 1.0f);
    	InvokeRepeating("UpdateRadar", 0.5f, 1.0f);
	}

	void UpdateBot() {

		//Build the radar scan info object
		ScanObject thisBot = new ScanObject();

		try {
			//3 tags are eval here? player = bot, bullets, obstacles
			thisBot.position = transform.position;
			thisBot.rotation = transform.rotation.eulerAngles;

			thisBot.velocity = __rb.velocity;
			thisBot.angularVelocity = __rb.angularVelocity;
			
			thisBot.id = Mathf.Abs(GetInstanceID());
			thisBot.eventType = BasicMessage.BotEvent.STATUS.ToString();
			Link.SendStatusMessage(thisBot);

		} catch(Exception ex) {
			Debug.Log(ex.ToString());
		}
	}

	void UpdateRadar() {
		Link.SendScanMessage(__scanned.ToArray());
		__scanned.Clear();
	}

	public void OnScan(GameObject obj)
	{
		//Build the radar scan info object
		ScanObject scan = new ScanObject();

		try {
			//3 tags are eval here? player = bot, bullets, obstacles
			scan.position = obj.transform.position;
			scan.rotation = obj.transform.rotation.eulerAngles;

			Rigidbody rb = obj.GetComponent<Rigidbody>();

			scan.velocity = rb.velocity;
			scan.angularVelocity = rb.angularVelocity;
			
			scan.id = Mathf.Abs(obj.GetInstanceID());
			scan.eventType = BasicMessage.RadarEvent.SCAN.ToString();
			__scanned.Add(scan);

		} catch(Exception ex) {
			Debug.Log(ex.ToString());
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

	// void UpdatePosition() {
	// 	// Game state changed
	// 	if (__state == BasicMessage.MatchEvent.PREPARING) {
	// 		__state = BasicMessage.MatchEvent.START;
	// 		MatchStateMessage msg = new MatchStateMessage ();
	// 		msg.state = __state;
	// 		Link.SendMatchUpdateMessage(msg);
	// 	}

	// 	//Position downstream
	// 	if (__currentPos != transform.position) {
	// 		__currentPos = transform.position;
	// 		Link.SendPositionUpdateMessage (__currentPos);
	// 	}

	// 	//Rotation downstream
	// 	if (__currentRot != transform.rotation) {
	// 		__currentRot = transform.rotation;
	// 		Link.SendRotationUpdateMessage (__currentRot);
	// 	}
	// }

	/// <summary>
	/// Handle physics
	/// </summary>
	void LateUpdate ()
	{
		/************************* DOWNSTREAM SIMULATOR -> CLIENT ***********************/



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
		Heat();
	}

	public void ApplyTorque(Vector3 vector) {
		__applyTorque = vector;
		Heat();
	}

	public void SetFiring(bool firing) {
		__isFiring = firing;
		Heat();
	}

}
