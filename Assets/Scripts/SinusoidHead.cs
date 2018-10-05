using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidHead : MonoBehaviour {
	public float Value;

	public void ActivatedAction() {
		transform.parent.GetComponent<SinusoidEnemy>().HeadCollected();
		Destroy(this.gameObject);
	}
}
