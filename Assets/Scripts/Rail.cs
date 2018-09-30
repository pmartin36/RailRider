using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SegmentType {
	None,
	Even,
	Odd,
	Corrupted
}

public class Rail : MonoBehaviour {

	public event EventHandler<int> NodesRemoved;

	private Vector3 nextSpawnLocation;
	private float lastSpawnTime;

	RailManager railManager;
	int seed;

	float corruptedRailOverrideChance;
	private Vector3 lastRailSpawnPosition;
	private RailSegment previousRailSegment;

	private List<RailNode> nodes;
	public int RailIndex;

	public RailNode LastNode {
		get {
			return nodes.LastOrDefault();
		}
	}

	public RailNode FirstNode {
		get
		{
			return nodes.FirstOrDefault();
		}
	}

	public int NodeCount {
		get {
			return nodes.Count;
		}
	}

	// Use this for initialization
	void Awake () {
		lastRailSpawnPosition = transform.position;
		railManager = GetComponentInParent<RailManager>();
		seed = Mathf.RoundToInt(UnityEngine.Random.value * 1000000);
		nodes = new List<RailNode>();
	}

	private void Start() {

	}

	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnRail(float density, float size, float spawnAngleDiff, int segIndex, float killTime = 20f) {
		var isCorrupted = UnityEngine.Random.value <= corruptedRailOverrideChance;
		if (isCorrupted || Perlin.Noise01(Time.time * 5 + seed) <= density) {
			RailSegment r = CreateRailSegment(size, spawnAngleDiff, segIndex, killTime: killTime);

			if( isCorrupted ) {
				r.SetCorrupted(true);
				corruptedRailOverrideChance *= 0.5f;
			}
			else {
				r.SetCorrupted(false);
				corruptedRailOverrideChance = 0;
			}
		}
		else {
			previousRailSegment = null;
			Vector2 rd =  (transform.position - lastRailSpawnPosition).normalized;
			Vector2 normal = rd.Rotate(90);
			Debug.DrawRay(transform.position, normal, Color.blue, 3f);
			nodes.Add(new RailNode(segIndex, 0, transform.position, rd, normal, isCorrupted, false));
			lastRailSpawnPosition = transform.position;
		}
	}

	public void SpawnCorruptedRail(float size, float spawnAngleDiff, int segIndex) {
		RailSegment r = CreateRailSegment(size, spawnAngleDiff, segIndex, true);
		r.SetCorrupted(true);
		corruptedRailOverrideChance = 0.8f;

		previousRailSegment = r;
	}

	public void SpawnRailWithRechargeMarker(float size, float spawnAngleDiff, int segIndex) {
		RailSegment r = CreateRailSegment(size, spawnAngleDiff, segIndex);
		r.SetCorrupted(false);	
		RechargeMarker marker = RechargeMarker.Create();
		marker.Init();

		RailNode n = r.Nodes[ r.Nodes.Length / 2 ];
		marker.transform.position = new Vector3(n.Position.x, n.Position.y, -1f);
		marker.transform.localRotation = Quaternion.Euler(0, 0, Utils.VectorToAngle(n.Direction));
		marker.AttachedRailSegment = r;
		r.RechargeMarker = marker;

		previousRailSegment = r;
	}

	private RailSegment CreateRailSegment(float size, float spawnAngleDiff, int segIndex, bool corrupted = false, float killTime = 20f) {
		RailSegment r = RailSegment.Create();
		r.parentRail = this;
		r.Init(killTime);
		var newNodes = r.CalculateNodes(size, spawnAngleDiff, lastRailSpawnPosition, this.transform.position, corrupted, segIndex);		
		nodes.AddRange(newNodes);

		previousRailSegment = r;
		lastRailSpawnPosition = transform.position;
		return r;
	}

	// player just hopped on rail
	public int GetTargetIndex(List<RailSegment> rs, Vector3 collisionPosition) {
		var rnodes = rs.SelectMany( r => r.Nodes ).ToArray();
		return GetTargetFromNodes(rnodes, collisionPosition);
	}

	public int GetTargetIndex(RailSegment rs, Vector3 collisionPosition) {
		return GetTargetFromNodes(rs.Nodes, collisionPosition);
	}

	private int GetTargetFromNodes(RailNode[] rnodes, Vector3 collisionPosition) {
		var sortedNodes = rnodes.OrderBy(n => Vector2.Distance(n.Position, collisionPosition)).ToList();
		RailNode selectedNode = sortedNodes.Count > 1 && sortedNodes[1].index > sortedNodes[0].index ? sortedNodes[1] : sortedNodes[0];

		var nodeIndex = nodes.LastIndexOf(selectedNode);
		return nodeIndex < 0 ? 0 : nodeIndex;
	}

	public RailNode GetTargetRailNode(int index) {
		return index < nodes.Count ? nodes[index] : RailNode.Invalid;
	}
	
	public void RemoveNodes(int numNodes) {
		nodes.RemoveRange(0,numNodes);
		NodesRemoved?.Invoke(this, numNodes);
	}
}
