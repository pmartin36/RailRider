using UnityEngine;
using System.Collections;

public class OnRailRechargeMarker : RechargeMarker, PoolObject {

	public RailSegment AttachedRailSegment;

	[Header("Pool Object Settings")]
	[SerializeField]
	protected string _key;
	public string Key { get { return _key; } set { _key = value; } }
	[SerializeField]
	protected int _startingCount;
	public int StartingCount { get { return _startingCount; } set { _startingCount = value; } }

	public override void Awake() {
		base.Awake();
		MarkerContainer = MarkerContainer ?? GameObject.FindGameObjectWithTag("MarkerContainer").transform;
		transform.parent = MarkerContainer;
	}

	public override void ActivatedAction() {
		Recycle();
	}

	public virtual void Recycle() {
		this.gameObject.SetActive(false);
		PoolManager.Instance.Recycle(this);
	}

	public override bool IsConditionMet(bool jumping, bool attachedToRail, float direction) {
		return attachedToRail && jumping;
	}

	public static OnRailRechargeMarker Create() {
		float random = UnityEngine.Random.value;
		if (random > 0.8f) {
			return RechargeRiderMarker.Create();
		}
		else {
			return SingleMarker.Create();
		}
	}
}
