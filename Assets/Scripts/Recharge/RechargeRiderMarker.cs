using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeRiderMarker : OnRailRechargeMarker {

	public static RechargeAIRider riderPrefab;

	public new static RechargeRiderMarker Create() {
		var name = "Recharge-Rider-Marker";
		PoolManager pm = PoolManager.Instance;
		if (!pm.ContainsKey(name)) {
			RechargeRiderMarker prefab = Resources.Load<RechargeRiderMarker>($"Prefabs/{name}");
			prefab.Key = name;
			pm.CreatePool(prefab);
		}
		RechargeRiderMarker seg = pm.Next<RechargeRiderMarker>(name);
		return seg;
	}

	public override void ActivatedAction() {
		// spawn Recharge Rider
		riderPrefab = riderPrefab ?? Resources.Load<RechargeAIRider>("Prefabs/Recharge-Rider");
		RechargeAIRider rider = GameObject.Instantiate(riderPrefab);
		rider.transform.position = transform.position;
		rider.ConnectToRail( new List<RailSegment>() { this.AttachedRailSegment } );
		base.ActivatedAction();
	}
}
