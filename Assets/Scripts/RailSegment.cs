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
		if (InitialRailSegment) {
			Nodes = new RailNode[1];
			killTime = Time.time + 10f;	
			Nodes[0] = new RailNode(new Vector3(35, parentRail.transform.position.y), Vector3.right, Vector3.up);
			InitialRailSegment = false;

			Mesh m = new Mesh();
			m.vertices = new Vector3[] {
				new Vector3(-35, parentRail.transform.position.y + Width),
				new Vector3(35, parentRail.transform.position.y + Width),
				new Vector3(35, parentRail.transform.position.y - Width),
				new Vector3(-35, parentRail.transform.position.y - Width)			
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
			killTime = Time.time + 5f;
		}
		NumNodes = 30;
	}

	public void Update() {
		if( Time.time > killTime ) {
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

		Vector2 pivot = (currentPosition - lastRailSpawnPosition) / 2f;
		Vector2 overallNormal = pivot.normalized.Rotate(90);
		pivot += overallNormal * Mathf.Sign(spawnAngleDiff) * distFactor * Mathf.Pow( spawnAngleDiff / 30f, 4);
		

		Nodes = new RailNode[NumNodes - 1];
		Vector2[] pcPoints = new Vector2[NumNodes * 2 ];
		Vector2 lastPosition = lastRailSpawnPosition;
		Vector3[] positions = new Vector3[ NumNodes ];
		Vector3[] vertices = new Vector3[NumNodes * 2];

		for (int i = 0; i < NumNodes; i++) {
			//Vector2 newPosition = Vector3.Lerp(lastRailSpawnPosition, currentPosition, i / ((float)(NumNodes - 1)));
			Vector2 newPosition = Utils.QuadraticBezier(lastRailSpawnPosition, currentPosition, pivot, i / ((float)(NumNodes - 1)));
			positions[i] = newPosition;
		
			Vector2 rd, normal;
			if( i > 0 ) {
				rd = (newPosition - lastPosition).normalized;
				normal = rd.Rotate(90);			
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
			}

			vertices[i] = newPosition + normal * Width;
			vertices[i + NumNodes] = newPosition - normal * Width;

			pcPoints[i] = newPosition + normal * Width * 1.2f;
			pcPoints[pcPoints.Length - (i + 1)] = newPosition - normal * Width * 1.2f;

			Debug.DrawRay(newPosition, rd, Color.green, 1);
			Debug.DrawRay(newPosition, normal, Color.cyan, 1);

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
