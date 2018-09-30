using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorruptedTrail : MonoBehaviour {
	
	private TrailRenderer tr;
	private int knownPositions;
	private EdgeCollider2D collider;

	private void Start() {
		tr = GetComponent<TrailRenderer>();
		collider = GetComponentInChildren<EdgeCollider2D>();

		collider.transform.parent = null;
		collider.transform.localScale = Vector3.one;
		collider.transform.position = Vector3.zero;
	}

	void Update () {
		if ( knownPositions > tr.positionCount ) {
			int diff = knownPositions - tr.positionCount;
			collider.points = collider.points.Skip(diff).Take(tr.positionCount).ToArray();
			knownPositions -= diff;
		}
		else if ( knownPositions < tr.positionCount) {
			List<Vector2> pos = new List<Vector2>();
			pos.AddRange(collider.points);
			while(knownPositions < tr.positionCount) {
				pos.Add(tr.GetPosition(knownPositions));
				knownPositions++;
			}
			collider.points = pos.ToArray();
		}
	}

	private void OnDestroy() {
		Destroy(collider.gameObject);
	}
}
