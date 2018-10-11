using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidHead : RechargeMarker {
	public override bool IsConditionMet(bool jumping, bool attachedToRail, float direction) {
		return jumping && Mathf.Abs(direction) < 0.2f;
	}

	public override void ActivatedAction() {
		transform.parent.GetComponent<SinusoidEnemy>().HeadCollected();
		Destroy(this.gameObject);
	}
}
