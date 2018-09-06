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

	public RailNode[] Nodes;

	public void Awake() {
		RailSegmentContainer = RailSegmentContainer ?? GameObject.FindGameObjectWithTag("SegmentContainer");
		transform.parent = RailSegmentContainer.transform;
		lr = GetComponent<LineRenderer>();
		pc = GetComponent<PolygonCollider2D>();	
	}

	public void OnEnable() {
		Nodes = new RailNode[NumNodes-1];
		if ( NumNodes == 2 ) {
			killTime = Time.time + 10f;
			NumNodes = 30;
			Nodes[0] = new RailNode(new Vector3(35, parentRail.transform.position.y), Vector3.right, Vector3.up);
		}
		else {
			killTime = Time.time + 5f;
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


	public RailNode[] CalculateNodes(float size, Vector3 lastRailSpawnPosition, Vector3 currentPosition) {
		Vector2[] pcPoints = new Vector2[ NumNodes * 2 ];
		Vector2 lastPosition = lastRailSpawnPosition;
		Vector3[] positions = new Vector3[ NumNodes ];

		for (int i = 0; i < NumNodes; i++) {
			Vector2 newPosition = Vector3.Lerp(lastRailSpawnPosition, currentPosition, i / ((float)(NumNodes - 1)));
			positions[i] = newPosition;
		
			Vector2 rd, normal;
			if( i > 0 ) {
				rd = (newPosition - lastPosition).normalized;
				normal = rd.Rotate(90);
				pcPoints[i] = newPosition + normal * lr.startWidth;
				pcPoints[pcPoints.Length - (i + 1)] = newPosition - normal * lr.startWidth * 1.2f;	
					
				Nodes[i-1] = new RailNode(i, newPosition, rd, normal );
			}
			else {
				if( parentRail.LastNode != RailNode.Invalid ) {
					rd = parentRail.LastNode.Direction;
					normal = parentRail.LastNode.Normal;
				}
				else {
					Vector2 nextPosition = Vector3.Lerp(lastRailSpawnPosition, currentPosition, (i+1) / ((float)(NumNodes - 1)));
					rd = (nextPosition - newPosition).normalized;
					normal = rd.Rotate(90);
				}	
				pcPoints[0] = newPosition + normal * lr.startWidth;
				pcPoints[pcPoints.Length - 1] = newPosition - normal * lr.startWidth * 1.2f;
			}

			Debug.DrawRay(newPosition, rd, Color.green, 1);
			Debug.DrawRay(newPosition, normal, Color.cyan, 1);

			lastPosition = newPosition;
		}

		lr.positionCount = NumNodes;
		lr.SetPositions(positions);
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
		parentRail.RemoveNodes(NumNodes-1);
		base.Recycle();
	}
}
