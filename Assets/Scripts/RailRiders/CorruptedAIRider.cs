using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorruptedAIRider : AIRider, EnemyType {

	Transform child;

	public override void Start() {	
		base.Start();

		RailManager rm = GameManager.Instance.RailManager;
		int index = Random.Range(0, RailManager.NumRails);
		// List<int> availableRails = Enumerable.Range(0, 3).ToList();

		ConnectToRail(rm.GetRail(index));
		for (int i = 0; i < AttachedRail.RailSegmentPositions.Count - 1; i++) {
			var screenPos = mainCamera.Camera.WorldToViewportPoint(AttachedRail.RailSegmentPositions[i+1]);
			if( screenPos.x > 0 && screenPos.y > 0 && screenPos.x < 1 && screenPos.y < 1 ) {
				RailIndex = AttachedRail.FindIndex(AttachedRail.RailSegmentPositions[i]);
				break;
			}
		}
		RailNode nextPosition = AttachedRail.GetTargetRailNode(RailIndex);
		transform.position = nextPosition.Position;	

		if (!nextPosition.Valid) {
			StartFreeMovement();
		}
		else {
			target = nextPosition;
		}

		child = transform.GetChild(0);
	}

	public override void DestroySelf() {
		child.parent = null;
		base.DestroySelf();
	}

	public void Init() {
	}
}
