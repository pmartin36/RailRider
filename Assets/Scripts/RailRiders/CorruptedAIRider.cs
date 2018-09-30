using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedAIRider : AIRider {

	Transform child;

	public override void Start() {	
		base.Start();
		ConnectToRail(GameManager.Instance.RailManager.GetAnotherRail(null));
		SetNextTarget();
		transform.position = target.Position;
		child = transform.GetChild(0);
	}

	public override void DestroySelf() {
		child.parent = null;
		base.DestroySelf();
	}
}
