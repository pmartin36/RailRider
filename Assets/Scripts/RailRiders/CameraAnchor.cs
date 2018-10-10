using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraAnchor : RailRider {

	Vector2 targetRotation;
	Vector2 currentRotation;

	private float segmentIndex = 0;
	public Vector2 LastDiff;

	// Use this for initialization
	public override void Start () {
		base.Start();
		currentRotation = Vector2.right;
		StartCoroutine(DelayedInit());
	}

	public override void SetTarget() {
		RailNode nextPosition = AttachedRail.GetTargetRailNode(RailIndex);
		target = nextPosition;

		var dist = Vector2.Distance(transform.position, target.Position);
		var angle = Vector2.SignedAngle(transform.position - target.Position, target.Direction) * Mathf.Deg2Rad;

		distanceToTarget = dist * Mathf.Abs(Mathf.Cos(angle));
		distanceToCenter = dist * Mathf.Sin(angle);
		targetRotation = target.Direction;

		while (target.SegmentIndex > this.segmentIndex) {
			segmentIndex++;
			GameManager.Instance.RailManager.AddRail();
		}
	}

	public override void Update() {
		var diff = Vector2.SignedAngle(currentRotation, targetRotation) * 1f * Time.deltaTime;
		currentRotation = currentRotation.Rotate(diff);
		transform.localRotation = Quaternion.Euler(0,0,Utils.VectorToAngle(currentRotation));
		Vector3 lastPosition = transform.position;
		base.Update();
		LastDiff = transform.position - lastPosition;
	}
}
