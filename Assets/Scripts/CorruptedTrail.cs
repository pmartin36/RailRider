using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorruptedTrail : MonoBehaviour, ICorruption {
	
	private TrailRenderer tr;

	private new EdgeCollider2D collider;
	private List<Vector2> points;

	public float InitialDamage {
		get {
			return 4;
		}
	}
	public float SustainDamage {
		get {
			return 6;
		}
	}

	private void Start() {
		tr = GetComponentInParent<TrailRenderer>();
		collider = GetComponent<EdgeCollider2D>();
		points = new List<Vector2>();

		transform.parent = null;
		transform.localScale = Vector3.one;
		transform.position = Vector3.zero;
	}

	void Update () {
		if(tr != null) {
			if ( points.Count > tr.positionCount ) {
				int diff = points.Count - tr.positionCount;
				points = points.Skip(diff).Take(tr.positionCount).ToList();
				collider.points = points.ToArray();
			}
			else if (points.Count < tr.positionCount) {
				while(points.Count < tr.positionCount) {
					points.Add(tr.GetPosition(points.Count));
				}
				collider.points = points.ToArray();
			}
		}
		else {
			Destroy(this.gameObject);
		}
	}

	private void OnDestroy() {
		if(collider?.gameObject != null) {
			Destroy(collider.gameObject);
		}
	}
}
