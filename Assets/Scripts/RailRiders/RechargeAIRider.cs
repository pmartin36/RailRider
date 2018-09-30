using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeAIRider : AIRider {

	private TrailRenderer tr;
	private static GameObject pelletPrefab;
	private float timeSinceLastPellet;

	public override void Start () {
		base.Start();
		tr = GetComponentInChildren<TrailRenderer>();
		pelletPrefab = pelletPrefab ?? Resources.Load<GameObject>("Prefabs/Recharge-Pellet");
	}

	public override void Update() {
		base.Update();
		timeSinceLastPellet += Time.deltaTime;
		if (timeSinceLastPellet > 0.25f) {
			RechargePellet p = RechargePellet.Create();
			p.transform.position = this.transform.position;
			timeSinceLastPellet = 0f;
		}
	}

	public override void DestroySelf() {
		tr.transform.parent = null;
		base.DestroySelf();
	}
}
