using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RechargeMarker : PoolObject {
	public static Transform MarkerContainer;
	public abstract bool IsConditionMet(float f);
	public abstract void Init();

	public virtual void Awake() {
		MarkerContainer = MarkerContainer ?? GameObject.FindGameObjectWithTag("MarkerContainer").transform;
		transform.parent = MarkerContainer;
	}
	public virtual void Start() { }

	public static RechargeMarker Create() {
		return SingleMarker.Create();
	}
	
}
