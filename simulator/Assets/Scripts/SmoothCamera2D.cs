using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour {

	public float interpVelocity;
	public float minDistance;
	public float followDistance;
	public Transform target;
	public Vector3 offset;
	Vector3 targetPos;
	// Use this for initialization


	private GameObject[] _players;

	void Start () {
//		targetPos = transform.position;


		_players = GameObject.FindGameObjectsWithTag ("Player");
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (target)
		{
			targetPos = new Vector3 (target.position.x, 100, target.position.z);
			transform.position = Vector3.Lerp( transform.position, targetPos, 0.05f);



		// // How many units should we keep from the players
		// float zoomFactor = 1.5f;
		// float followTimeDelta = 0.8f;

		// // Midpoint we're after
		// Vector3 midpoint = (_players[0].transform.position + _players[1].transform.position) / 2f;

		// // Distance between objects
		// float distance = (_players[0].transform.position - _players[1].transform.position).magnitude;

		// // Move camera a certain distance
		// Vector3 cameraDestination = midpoint - Camera.main.transform.forward * distance * zoomFactor;

		// // Adjust ortho size if we're using one of those
		// if (Camera.main.orthographic)
		// {
		// 	// The camera's forward vector is irrelevant, only this size will matter
		// 	Camera.main.orthographicSize = distance;
		// }
		// // You specified to use MoveTowards instead of Slerp
		// Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, cameraDestination, followTimeDelta);

		// // Snap when close enough to prevent annoying slerp behavior
		// if ((cameraDestination - Camera.main.transform.position).magnitude <= 0.05f)
		// 	Camera.main.transform.position = cameraDestination;




		}
	}
}
