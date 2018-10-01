using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedAIRider : AIRider, EnemyType {

	Transform child;

	public override void Start() {	
		base.Start();
		ConnectToRail(GameManager.Instance.RailManager.GetAnotherRail(null));

		for (int i = 0; i < AttachedRail.RailSegmentPositions.Count - 1; i++) {
			var screenPos = mainCamera.Camera.WorldToViewportPoint(AttachedRail.RailSegmentPositions[i+1]);
			if( screenPos.x > 0 && screenPos.y > 0 && screenPos.x < 1 && screenPos.y < 1 ) {
				RailIndex = AttachedRail.FindIndex(AttachedRail.RailSegmentPositions[i]);
				break;
			}
		}

		SetTarget();
		transform.position = target.Position;
		child = transform.GetChild(0);
	}

	public override void DestroySelf() {
		child.parent = null;
		base.DestroySelf();
	}

	public void Init() {
	}
}
