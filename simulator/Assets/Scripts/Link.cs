using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Link : MonoBehaviour {

	public string latestaction = "gg";
	public float msg = 0;
public GUIStyle style;

	/*!
	 * Relay json event to JS for sending it via WS to client
	 */
	[DllImport("__Internal")]
  	public static extern void TransmitMessage (string message);

  	private Bot __bot;
		private FinishLine __finishLine;

  	void Start() {
  		__bot = GameObject.FindWithTag("Player").GetComponent<Bot>();
			__finishLine = GameObject.FindWithTag("FinishLine").GetComponent<FinishLine>();
			// InvokeRepeating("Overheat",0f,1f);
  	}

		// void Overheat() {
		// 	MessageReceived("{\"fire\": true}");
		// }

  	/*!
  	 * Receive message from JS application via IoT WS
  	 * @param {[type]} string message JSON action
  	 */
	void MessageReceived (string message) {
		Debug.Log("------>message received...");
		Debug.Log(message);
		BotAction action = JsonUtility.FromJson<BotAction>(message);
		msg++;
		this.latestaction = JsonUtility.ToJson(action);
		if (action.fire == true || action.fire == false) {
			__bot.SetFiring(action.fire);
		}
		if (action.force != null && action.force != Vector3.zero) {
			__bot.ApplyForce(action.force);
		}
		if (action.torque != null && action.torque != Vector3.zero) {
			__bot.ApplyTorque(action.torque);
		}
	}

	public static void SendScanMessage(ScanObject[] msg) {
		try {
			Debug.Log("TOTAL->"+msg.Length);
			foreach(ScanObject o in msg) {
				Debug.Log(JsonUtility.ToJson(o))
				;
			}
			// Debug.Log(JsonUtility.ToJson(msg));
			TransmitMessage(JsonUtility.ToJson(msg));
		} catch {

		}
	}

	public static void SendStatusMessage(ScanObject msg) {
		try {
			Debug.Log(JsonUtility.ToJson(msg));
			TransmitMessage(JsonUtility.ToJson(msg));
		} catch {

		}
	}

	public static void SendMatchUpdateMessage(MatchStateMessage msg) {
		try {
			TransmitMessage(JsonUtility.ToJson(msg));
		} catch {

		}
	}

	public static void SendPositionUpdateMessage(Vector3 position) {
		try {
			TransmitMessage(JsonUtility.ToJson(position));
		} catch {

		}
	}

	public static void SendRotationUpdateMessage(Quaternion rotation) {
		try {
			TransmitMessage(JsonUtility.ToJson(rotation));
		} catch {

		}
	}


	void OnGUI() {
		 GUI.Label(new Rect(50, Screen.height-100, 300, 20), "Incoming messages "+msg.ToString(), style);
 }
}
