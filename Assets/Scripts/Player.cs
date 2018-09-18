using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : RailRider {

	public override void Start () {
		GameManager.Instance.Player = this;
		base.Start();
	}

	public override void Update () {
		base.Update();
	}

	public void ProcessInputs(InputPackage p) {
		Vector2 inputDirection = new Vector2(p.Horizontal, p.Vertical);
		if (AttachedRail != null) {			
			float angleBtwDirection = Vector2.SignedAngle( target.Direction, inputDirection );
			float absAngle = Mathf.Abs(angleBtwDirection);
			float tolerance = 45f;

			if ( p.Jump && absAngle > 90 - tolerance && absAngle < 90 + tolerance) {
				// jump
				GravityDirection = Mathf.Sign(angleBtwDirection) * target.Normal;
				GravitySpeed = 2f;
				DisconnectFromRail();
				
			}
			else if( inputDirection.sqrMagnitude > 0.4f ) {
				if (absAngle > 180 - tolerance) {
					// slow down
					RailSpeed = Speed * 0.4f;
				}
				else if (absAngle < tolerance) {
					// speed up
					RailSpeed = Speed * 1.75f;
				}
			}	
			else {
				RailSpeed = Speed;
			}
		}
		else  {
			if (p.Jump) {
				//should probably use nonAlloc version - fix later for performance
				ConnectToRailByOverlap(cc.radius);
			}
		}
	}

	public override void OnTriggerEnter2D(Collider2D collision) {

	}

	public override void OnTriggerExit2D(Collider2D collision) {

	}
}
