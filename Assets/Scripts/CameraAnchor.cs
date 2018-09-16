using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraAnchor : RailRider {

	float targetRotation;
	float currentRotation;
	private float segmentIndex = 0;

	// Use this for initialization
	public override void Start () {
		base.Start();
		currentRotation = 0f;
	}

	public override void SetTarget() {
		RailNode nextPosition = AttachedRail.GetTargetRailNode(RailIndex);
		target = nextPosition;

		var dist = Vector2.Distance(transform.position, target.Position);
		var angle = Vector2.SignedAngle(transform.position - target.Position, target.Direction) * Mathf.Deg2Rad;

		distanceToTarget = dist * Mathf.Abs(Mathf.Cos(angle));
		distanceToCenter = dist * Mathf.Sin(angle);
		//transform.localRotation = Quaternion.Euler(0,0,Utils.VectorToAngle(target.Direction));

		while (target.SegmentIndex > this.segmentIndex) {
			segmentIndex++;
			GameManager.Instance.RailManager.AddRail();
		}
	}

	public override void Update() {
		base.Update();
	}
}
