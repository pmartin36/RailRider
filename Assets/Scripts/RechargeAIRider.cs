using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeAIRider : AIRider {

	private TrailRenderer tr;

	public override void Start () {
		base.Start();
		tr = GetComponentInChildren<TrailRenderer>();
	}

	public override void DestroySelf() {
		tr.transform.parent = null;
		base.DestroySelf();
	}
}
