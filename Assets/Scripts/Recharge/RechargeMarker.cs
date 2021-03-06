﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RechargeMarker : MonoBehaviour {
	public static Transform MarkerContainer;
	public float Value;

	public abstract bool IsConditionMet(bool jumping, bool attachedToRail, float direction);
	public virtual void Init() { }
	public virtual void Awake() {
		
	}
	public virtual void Start() { }
	public virtual void ActivatedAction() { }
}
