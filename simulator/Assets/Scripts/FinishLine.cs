using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour {

  private bool _over = false;
  private bool _win = true;

  private Bot __bot;

  private bool _isNotified = false;

  public GUIStyle style;
  void Start() {
    Time.timeScale = 1;
    _over = false;
    __bot = GameObject.FindWithTag("Player").GetComponent<Bot>();
  }

  void Update() {
      if (__bot.GetHeat() >= 10) {
        _win = false;
        _over = true;
      }


      if (_over && !_isNotified) {
        MatchStateMessage msg = new MatchStateMessage ();
        msg.state = BasicMessage.MatchEvent.END;
        Link.SendMatchUpdateMessage(msg);
        _isNotified = true;
        Time.timeScale = 0;
      }
  }

	// Update is called once per frame
	void OnCollisionEnter (Collision collision) {
    if (collision.gameObject.tag == "Player") {
		    Debug.Log(collision.gameObject.transform.tag.ToString());
        _over = true;
        _win = true;
    }
	}

  void OnGUI() {

      if (_over) {
        if (_win) {
          if (GUI.Button(new Rect(10, 70, 500, 60), "Mission Success!!!!")) {
             Application.LoadLevel("Main");
           }
        } else {
          if (GUI.Button(new Rect(10, 70, 500, 80), "Mission Failed, you overheated!!! \n every action overheats the bot 10%, bot coolsdown at a rate of 10% per second. \nLimit the actions per second you issue to your bot")) {
              Application.LoadLevel("Main");
          }
        }
     } else {
       GUI.Label(new Rect(50, 100, 300, 40), "Overheat "+__bot.GetHeat()*10.0f+"%", style);
     }

     GUI.Label(new Rect(50, Screen.height-40, 300, 20), "0.0.15", style);
 }
}
