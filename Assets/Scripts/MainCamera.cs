using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainCamera : MonoBehaviour {

	public Player player;
	public RailManager rm;
	float targetRotation;
	float currentRotation;

	// Use this for initialization
	void Start () {
		currentRotation = 0f;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//transform.position = new Vector3(player.transform.position.x+12, 0) + Vector3.back * 10;
		
		//var nodes = rm.Rails.Select( r => r.GetTargetRailNode( player.RailIndex ));
		if (player.target != RailNode.Invalid) {
			transform.position += player.MaxSpeed * player.target.Direction * Time.deltaTime;
		}

		targetRotation = Vector2.SignedAngle(Vector3.right, player.target.Direction); //Utils.VectorToAngle(player.target.Direction);
		var diff = targetRotation - currentRotation;
		if( diff < 1 ) {
			currentRotation += diff * 2f * Time.deltaTime;
			transform.rotation = Quaternion.Euler(0, 0, currentRotation);
		}
	}
}
