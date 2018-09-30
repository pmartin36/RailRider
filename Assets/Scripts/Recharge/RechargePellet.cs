using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargePellet : RechargeMarker, PoolObject {

	[Header("Recharge Pellet Settings")]
	public float Lifetime;
	private float aliveTime;

	public new static RechargePellet Create() {
		var name = "Recharge-Pellet";
		PoolManager pm = PoolManager.Instance;
		if (!pm.ContainsKey(name)) {
			RechargePellet prefab = Resources.Load<RechargePellet>($"Prefabs/{name}");
			prefab.Key = name;
			pm.CreatePool(prefab);
		}
		RechargePellet seg = pm.Next<RechargePellet>(name);
		return seg;
	}

	public override bool IsConditionMet(bool jumping, bool attachedToRail, float direction) {
		return true;
	}

	public void Update() {
		aliveTime += Time.deltaTime;
		if(Lifetime < aliveTime){
			Recycle();
		}
	}

	public override void Recycle() {
		aliveTime = 0;
		base.Recycle();
	}
}
