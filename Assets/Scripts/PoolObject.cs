using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PoolObject : MonoBehaviour{
	[HideInInspector]
	public string Key;
	public int StartingCount;

	public virtual void Recycle() {
		gameObject.SetActive(false);
		PoolManager.Instance.Recycle(this);
	}
}

