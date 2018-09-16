using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RailSegment : PoolObject {

	public static GameObject RailSegmentContainer;
	private bool Corrupted;

	private MeshRenderer meshRenderer;
	private MeshFilter mf;
	public float Width;
	public Rail parentRail;

	[Range(2,100)]
	public int NumNodes;
	public int ModifiedNumNodes;
	public bool InitialRailSegment;
	private float killTime;

	private PolygonCollider2D pc;

	public RailNode[] Nodes;
	public int SegmentIndex;

	public void Awake() {
		RailSegmentContainer = RailSegmentContainer ?? GameObject.FindGameObjectWithTag("SegmentContainer");
		transform.parent = RailSegmentContainer.transform;
		meshRenderer = GetComponent<MeshRenderer>();
		mf = GetComponent<MeshFilter>();
		pc = GetComponent<PolygonCollider2D>();	
	}

	public void OnEnable() {
		pc.enabled = true;
	}

	public void Init(float ktime) {
		pc.enabled = true;
		killTime = Time.time + ktime;
	}

	public void Update() {
		float time = Time.time;
		if ( pc.enabled && Time.time > (killTime - 4f) ) {
			pc.enabled = false;
		}
		else if ( Time.time > killTime ) {
			Recycle();
		}
	}

	public void SetCorrupted(bool corrupted) {
		Corrupted = corrupted;
		if (corrupted) {
			meshRenderer.material.color = Color.red;
		}
		else {
			meshRenderer.material.color = Color.white;
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


	public RailNode[] CalculateNodes(float size, float spawnAngleDiff, Vector3 lastRailSpawnPosition, Vector3 currentPosition, int segIndex) {
		SegmentIndex = segIndex;
		float totalDistance = Vector3.Distance(lastRailSpawnPosition, currentPosition);
		float distFactor = (totalDistance / size);
		ModifiedNumNodes = (int)(NumNodes * distFactor);

		Nodes = new RailNode[ModifiedNumNodes - 1];
		Vector2[] pcPoints = new Vector2[ModifiedNumNodes * 2 ];
		Vector3[] positions = new Vector3[ModifiedNumNodes];
		Vector3[] vertices = new Vector3[ModifiedNumNodes * 2];

		RailNode lastNode = parentRail?.LastNode;
		bool lastNodeValid = lastNode != null && lastNode.Valid;
		Vector2 lastPosition = lastRailSpawnPosition;

		Vector2 pivot;
		if (lastNodeValid) {
			pivot = lastRailSpawnPosition + lastNode.Direction * totalDistance;
		}
		else {
			Vector2 center = (currentPosition + lastRailSpawnPosition) / 2f;
			Vector2 overallNormal = ((Vector2)(currentPosition - lastRailSpawnPosition).normalized).Rotate(90);
			pivot = center + overallNormal * distFactor * -Mathf.Pow(spawnAngleDiff / 25f, 3);
		}	

		for (int i = 0; i < ModifiedNumNodes; i++) {	
			Vector2 rd, normal, newPosition;
			float pct = i / ((float)(ModifiedNumNodes - 1));	

			if (lastNodeValid) {
				newPosition = Vector2.Lerp(lastRailSpawnPosition, currentPosition, pct) * pct +
								Vector2.Lerp(lastRailSpawnPosition, pivot, pct) * (1-pct) ;
			}
			else {
				newPosition = Utils.QuadraticBezier(lastRailSpawnPosition, currentPosition, pivot, pct);
			}		
			positions[i] = newPosition;

			if (i > 0) {
				rd = (newPosition - lastPosition).normalized;
				normal = rd.Rotate(90);
				Nodes[i - 1] = new RailNode(segIndex, i, newPosition, rd, normal, true);
			} else {
				if (lastNodeValid) {
					rd = lastNode.Direction;
					normal = lastNode.Normal;
				}
				else {
					var nextPosition = Utils.QuadraticBezier(lastRailSpawnPosition, currentPosition, pivot, pct + (1 / ((float)ModifiedNumNodes - 1)));
					rd = (nextPosition - newPosition).normalized;
					normal = rd.Rotate(90);
				}
			}

			vertices[i] = newPosition + normal * Width;
			vertices[i + ModifiedNumNodes] = newPosition - normal * Width;

			pcPoints[i] = newPosition + normal * Width * 1.2f;
			pcPoints[pcPoints.Length - (i + 1)] = newPosition - normal * Width * 1.2f;

			//Debug.DrawRay(newPosition, rd/2f, Color.green, 2);
			//Debug.DrawRay(newPosition, normal, Color.cyan, 2);

			lastPosition = newPosition;
		}

		pc.points = pcPoints;

		Mesh m = new Mesh();
		m.vertices = vertices;
		//m.uv = 
		List <int> tris = new List<int>();
		for(int i = 0; i < ModifiedNumNodes - 1; i++) {
			tris.AddRange(new int [] {
				i, i+1, i+ModifiedNumNodes,
				i+ModifiedNumNodes+1, i+ModifiedNumNodes, i+1
			});
		}
		m.triangles = tris.ToArray();
		
		m.RecalculateNormals();
		mf.sharedMesh = m;

		return Nodes;
	}

	public override void Recycle() {
		parentRail.RemoveNodes(ModifiedNumNodes-1);
		base.Recycle();
	}
}
