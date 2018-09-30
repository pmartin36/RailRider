using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RechargeMarker : MonoBehaviour, PoolObject {
	public static Transform MarkerContainer;
	public RailSegment AttachedRailSegment;

	[Header("Pool Object Settings")]
	[SerializeField]
	protected string _key;
	public string Key { get { return _key; } set { _key = value; } }
	[SerializeField]
	protected int _startingCount;
	public int StartingCount { get { return _startingCount;} set { _startingCount = value;} }

	public virtual bool IsConditionMet(bool jumping, bool attachedToRail, float direction) {
		return attachedToRail && jumping;
	}
	public virtual void Init() { }
	public virtual void Awake() {
		MarkerContainer = MarkerContainer ?? GameObject.FindGameObjectWithTag("MarkerContainer").transform;
		transform.parent = MarkerContainer;
	}
	public virtual void Start() { }
	public virtual void ActivatedAction() {
		Recycle();
	}

	public static RechargeMarker Create() {
		float random = UnityEngine.Random.value;
		if( random > 0.0f) {
			return RechargeRiderMarker.Create();
		}
		else {
			return SingleMarker.Create();
		}
	}

	public virtual void Recycle() {
		this.gameObject.SetActive(false);
		PoolManager.Instance.Recycle(this);
	}
}
