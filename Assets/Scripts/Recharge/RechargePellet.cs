using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargePellet : RechargeMarker, PoolObject {

	[Header("Recharge Pellet Settings")]
	public float Lifetime;
	private float aliveTime;

	[Header("Pool Object Settings")]
	[SerializeField]
	protected string _key;
	public string Key { get { return _key; } set { _key = value; } }
	[SerializeField]
	protected int _startingCount;
	public int StartingCount { get { return _startingCount; } set { _startingCount = value; } }

	public static RechargePellet Create() {
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

	public override void ActivatedAction() {
		Recycle();
	}

	public void Update() {
		aliveTime += Time.deltaTime;
		if(Lifetime < aliveTime){
			Recycle();
		}
	}

	public void Recycle() {
		aliveTime = 0;
		this.gameObject.SetActive(false);
		PoolManager.Instance.Recycle(this);
	}
}
