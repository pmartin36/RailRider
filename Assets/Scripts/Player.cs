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
		if (p.Jump && AttachedRail != null) {
			if(p.Vertical > 0.5f) {
				Gravity = -Mathf.Abs(Gravity);
				GravitySpeed += Gravity * 30f * Time.deltaTime;
				DisconnectFromRail();
			}
			else if(p.Vertical < -0.5f) {
				Gravity = Mathf.Abs(Gravity);
				GravitySpeed += Gravity * 30f * Time.deltaTime;
				DisconnectFromRail();
			}
			else {
				// spin thing
			}
		}

		if (AttachedRail == null) {
			if (p.Right || p.Left) {
				//should probably use nonAlloc version - fix later for performance
				Collider2D[] rails = Physics2D.OverlapCircleAll(transform.position, cc.radius, 1 << LayerMask.NameToLayer("Rail"));
				if(rails.Length > 0) {
					ConnectToRail( rails.Select( r => r.gameObject.GetComponent<RailSegment>()).ToList() );
				}
			}
		}
	}

	public override void OnTriggerEnter2D(Collider2D collision) {

	}

	public override void OnTriggerExit2D(Collider2D collision) {

	}
}
