using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraAnchor : RailRider {

	float targetRotation;
	float currentRotation;

	// Use this for initialization
	public override void Start () {
		base.Start();
		currentRotation = 0f;
	}

	public override void DisconnectFromRail() {
		var nextRailIndex = RailIndex;
		var nextTarget = AttachedRail.GetTargetRailNode(nextRailIndex);
		Rail nextRail = AttachedRail;
		float rayDist = 20;

		base.DisconnectFromRail();

		do {
			List<RaycastHit2D> hits = Physics2D.RaycastAll(nextTarget.Position - nextTarget.Normal * rayDist/2f, nextTarget.Normal, rayDist, 1 << LayerMask.NameToLayer("Rail"))
										.Where(r => r.collider != null)
										.OrderBy(r => Vector2.Distance(r.point, transform.position)).ToList();
			Debug.DrawRay(nextTarget.Position - nextTarget.Normal * rayDist/2f, nextTarget.Normal * rayDist, Color.yellow, 3f);
			if(hits.Count > 0) {
				RaycastHit2D hit = hits[0];
				RailSegment seg = hits[0].collider.GetComponent<RailSegment>();
				nextRail = seg.parentRail;			
				nextRailIndex = nextRail.GetTargetIndex(seg, hit.point);
			}
			else {
				nextRailIndex++;
			}

			nextTarget = nextRail.GetTargetRailNode(nextRailIndex);
		}  while (nextRailIndex < nextRail.NodeCount && !nextTarget.Valid);

		if(nextRailIndex >= nextRail.NodeCount) {
			Debug.Log("We didn't find any rails???");
			return;
		}
		else {
			Debug.Log(nextTarget.Position);
			Debug.Log(nextRail.gameObject.name);
		}

		ConnectToRail(nextRail);
		RailIndex = nextRailIndex;
		SetTarget();
		if (centerOnRailRoutine != null) StopCoroutine(centerOnRailRoutine);
		centerOnRailRoutine = StartCoroutine(CenterOnRail());
	}

	public override void Update() {
		base.Update();
	}
}
