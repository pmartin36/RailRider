using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RailSegment : PoolObject {

	public static GameObject RailSegmentContainer;
	private bool Corrupted;

	private LineRenderer lr;
	public Rail parentRail;

	[Range(2,30)]
	public int NumNodes;
	private float killTime;

	private PolygonCollider2D pc;

	public Vector2[] RailDirection;
	public Vector3[] Nodes;

	public void Awake() {
		RailSegmentContainer = RailSegmentContainer ?? GameObject.FindGameObjectWithTag("SegmentContainer");
		transform.parent = RailSegmentContainer.transform;
		lr = GetComponent<LineRenderer>();
		pc = GetComponent<PolygonCollider2D>();

		RailDirection = new Vector2[NumNodes];
	}

	public void OnEnable() {
		RailDirection = new Vector2[NumNodes];
		if( NumNodes == 2 ) {
			killTime = Time.time + 30f;
			Nodes = new Vector3[2];
			Nodes[0] = new Vector3( -35f, transform.position.y);
			Nodes[1] = new Vector3( 34f, transform.position.y);
			NumNodes = 15;
		}
		else {
			killTime = Time.time + 20f;
		}	
	}

	public void Update() {
		if( Time.time > killTime ) {
			Recycle();
		}
	}

	public void SetCorrupted(bool corrupted) {
		Corrupted = corrupted;
		if (corrupted) {
			lr.material.color = Color.red;
		}
		else {
			lr.material.color = Color.white;
		}
	}

	public static RailSegment Create() {
		var name = "Rail-Segment";
		PoolManager pm = PoolManager.Instance;
		if (!pm.ContainsKey(name)) {
			RailSegment prefab = Resources.Load<RailSegment>($"Prefabs/{name}");
			prefab.Key = name;
			pm.CreatePool(prefab);
		}
		RailSegment seg = pm.Next(name) as RailSegment;
		return seg;
	}


	public Vector3[] CalculateNodes(float size, Vector3 lastRailSpawnPosition, Vector3 currentPosition) {
		Vector2[] pcPoints = new Vector2[ NumNodes * 2 ];
		Vector2 lastPosition = lastRailSpawnPosition;
		Nodes = new Vector3[NumNodes];

		for (int i = 0; i < NumNodes; i++) {
			Vector2 newPosition = Vector3.Lerp(lastRailSpawnPosition, currentPosition, i / ((float)(NumNodes - 1)));
			Nodes[i] = newPosition;
		
			if( i > 0 ) {
				Vector2 rd = (newPosition - lastPosition).normalized;
				Vector2 normal = rd.Rotate(90);
				pcPoints[i] = newPosition + normal * lr.startWidth;
				pcPoints[pcPoints.Length - (i + 1)] = newPosition - normal * lr.startWidth * 1.2f;		
				RailDirection[i] = rd;		

				if(i == 1) {
					pcPoints[0] = lastPosition + normal * lr.startWidth;
					pcPoints[pcPoints.Length - 1] = lastPosition - normal * lr.startWidth * 1.2f;
					RailDirection[0] = rd;
				}
			}

			lastPosition = newPosition;
		}

		lr.positionCount = NumNodes;
		lr.SetPositions(Nodes);

		pc.points = pcPoints;

		return Nodes;
	}

	public Vector3 GetNearestNode(Vector3 collisionPoint) {
		var positions = new Vector3[lr.positionCount];
		lr.GetPositions(positions);

		var pd = positions.OrderBy(p => Vector2.Distance(p, collisionPoint)).ToList();

		return pd.First();
	}

	public override void Recycle() {
		parentRail.RemoveNodes(NumNodes);
		base.Recycle();
	}
}
