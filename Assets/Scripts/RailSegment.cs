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
	public bool InitialRailSegment;
	private float killTime;

	private PolygonCollider2D pc;

	public RailNode[] Nodes;

	public void Awake() {
		RailSegmentContainer = RailSegmentContainer ?? GameObject.FindGameObjectWithTag("SegmentContainer");
		transform.parent = RailSegmentContainer.transform;
		meshRenderer = GetComponent<MeshRenderer>();
		mf = GetComponent<MeshFilter>();
		pc = GetComponent<PolygonCollider2D>();	
	}

	public void OnEnable() {
		pc.enabled = true;
		if (InitialRailSegment) {
			Nodes = new RailNode[1];
			killTime = Time.time + 15f;	
			Nodes[0] = new RailNode(new Vector3(35, parentRail.transform.position.y), Vector3.right, Vector3.up);
			InitialRailSegment = false;

			Mesh m = new Mesh();
			m.vertices = new Vector3[] {
				new Vector3(-60, parentRail.transform.position.y + Width),
				new Vector3(35, parentRail.transform.position.y + Width),
				new Vector3(35, parentRail.transform.position.y - Width),
				new Vector3(-60, parentRail.transform.position.y - Width)			
			};
			m.uv = new Vector2[] {
				new Vector2(0, 1),
				new Vector2(1, 1),
				new Vector2(1, 0),
				new Vector2(0, 0)
			};
			m.triangles = new int[] {
				0, 1, 3,
				2, 3, 1
			};
			m.RecalculateNormals();
			mf.sharedMesh = m;
		}
		else {
			killTime = Time.time + 10f;
		}
		NumNodes = 30;
	}

	public void Update() {
		float time = Time.time;
		if ( pc.enabled && Time.time > (killTime - 5f) ) {
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


	public RailNode[] CalculateNodes(float size, float spawnAngleDiff, Vector3 lastRailSpawnPosition, Vector3 currentPosition) {
		float totalDistance = Vector3.Distance(lastRailSpawnPosition, currentPosition);
		float distFactor = (totalDistance / size);
		NumNodes = (int)(NumNodes * distFactor);

		Nodes = new RailNode[NumNodes - 1];
		Vector2[] pcPoints = new Vector2[NumNodes * 2 ];
		Vector3[] positions = new Vector3[ NumNodes ];
		Vector3[] vertices = new Vector3[NumNodes * 2];

		RailNode lastNode = parentRail.LastNode;
		Vector2 lastPosition = lastRailSpawnPosition;

		Vector2 pivot;
		if (lastNode.Valid) {
			pivot = lastRailSpawnPosition + lastNode.Direction * totalDistance;
		}
		else {
			Vector2 center = (currentPosition + lastRailSpawnPosition) / 2f;
			Vector2 overallNormal = ((Vector2)(currentPosition - lastRailSpawnPosition).normalized).Rotate(90);
			pivot = center + overallNormal * distFactor * -Mathf.Pow(spawnAngleDiff / 25f, 3);
		}	

		for (int i = 0; i < NumNodes; i++) {	
			Vector2 rd, normal, newPosition;
			float pct = i / ((float)(NumNodes - 1));	

			if (lastNode.Valid) {
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
				Nodes[i - 1] = new RailNode(i, newPosition, rd, normal);
			} else {
				if (lastNode.Valid) {
					rd = lastNode.Direction;
					normal = lastNode.Normal;
				}
				else {
					var nextPosition = Utils.QuadraticBezier(lastRailSpawnPosition, currentPosition, pivot, pct + (1 / ((float)NumNodes - 1)));
					rd = (nextPosition - newPosition).normalized;
					normal = rd.Rotate(90);
				}
			}

			vertices[i] = newPosition + normal * Width;
			vertices[i + NumNodes] = newPosition - normal * Width;

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
		for(int i = 0; i < NumNodes - 1; i++) {
			tris.AddRange(new int [] {
				i, i+1, i+NumNodes,
				i+NumNodes+1, i+NumNodes, i+1
			});
		}
		m.triangles = tris.ToArray();
		
		m.RecalculateNormals();
		mf.sharedMesh = m;

		return Nodes;
	}

	public override void Recycle() {
		parentRail.RemoveNodes(NumNodes-1);
		base.Recycle();
	}
}
