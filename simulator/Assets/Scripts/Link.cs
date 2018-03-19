using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Link : MonoBehaviour {

	public string latestaction = "gg";

	/*!
	 * Relay json event to JS for sending it via WS to client
	 */
	[DllImport("__Internal")]
  	public static extern void TransmitMessage (string message);

  	private Bot __bot;

  	void Start() {
  		__bot = GameObject.FindWithTag("Player").GetComponent<Bot>();
  	}

  	/*!
  	 * Receive message from JS application via IoT WS
  	 * @param {[type]} string message JSON action
  	 */
	void MessageReceived (string message) {
		Debug.Log("------>message received...");
		Debug.Log(message);
		BotAction action = JsonUtility.FromJson<BotAction>(message);

		this.latestaction = JsonUtility.ToJson(action);
		if (action.fire != null)
			__bot.SetFiring(action.fire);
		if (action.force != null)
			__bot.ApplyForce(action.force);
		if (action.torque != null)
			__bot.ApplyTorque(action.torque);
		// Call set firing, add torque and add force
	}

	public static void SendScanMessage(ScanMessage msg) {
		try {
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
        if (GUI.Button(new Rect(10, 10, 450, 100), this.latestaction))
            print("You clicked the button!");
        
    }
}