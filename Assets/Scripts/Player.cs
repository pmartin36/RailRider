using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : RailRider {

	public List<RailSegment> TouchedRailSegments;

	public override void Start () {
		GameManager.Instance.Player = this;
		TouchedRailSegments = new List<RailSegment>();
		base.Start();
	}

	public override void Update () {
		base.Update();
	}

	public void ProcessInputs(InputPackage p) {
		if (p.Jump && AttachedRail != null) {
			if(p.Vertical > 0.5f) {
				Gravity = -Mathf.Abs(Gravity);
				DisconnectFromRail();
			}
			else if(p.Vertical < -0.5f) {
				Gravity = Mathf.Abs(Gravity);
				DisconnectFromRail();
			}
			else {
				// spin thing
			}
		}

		if (TouchedRailSegments != null && AttachedRail == null) {
			if (p.Right || p.Left) {
				//Collider2D[] rails = Physics2D.OverlapCircleAll(transform.position, cc.radius, 1 << LayerMask.NameToLayer("Rail"));
				ConnectToRail(TouchedRailSegments);
				TouchedRailSegments.Clear();
			}
		}
	}

	public override void OnTriggerEnter2D(Collider2D collision) {
		if (AttachedRail == null && collision.gameObject.tag == "Rail") {
			RailSegment seg = collision.gameObject.GetComponent<RailSegment>();
			TouchedRailSegments.Add(seg);
		}
	}

	public override void OnTriggerExit2D(Collider2D collision) {
		if (AttachedRail == null && collision.gameObject.tag == "Rail") {
			RailSegment seg = collision.gameObject.GetComponent<RailSegment>();
			TouchedRailSegments.Remove(seg);
		}
	}
}
