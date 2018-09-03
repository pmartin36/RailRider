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

	private List<Vector3> nodes;

	// Use this for initialization
	void Awake () {
		railManager = GetComponentInParent<RailManager>();
		lastRailSpawnPosition = transform.position;
		seed = UnityEngine.Random.Range(0, 1000000);
		nodes = new List<Vector3>();
		nodes.Add(new Vector2(-35, transform.position.y));
		nodes.Add(new Vector2(34, transform.position.y));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnRail(float density, float size) {
		var isCorrupted = UnityEngine.Random.value <= corruptedRailOverrideChance;
		if (isCorrupted || Perlin.Noise01(Time.time * 5 + seed) <= density) {
			RailSegment r = CreateRailSegment(size);

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
			nodes.Add(Vector3.back);
			lastRailSpawnPosition = transform.position;
		}
	}

	public void SpawnCorruptedRail(float size) {
		RailSegment r = CreateRailSegment(size);
		r.SetCorrupted(true);
		corruptedRailOverrideChance = 0.8f;
		previousRailSegment = r;
	}

	private RailSegment CreateRailSegment(float size) {
		RailSegment r = RailSegment.Create();
		r.parentRail = this;
		var positions = r.CalculateNodes(size, lastRailSpawnPosition, this.transform.position);
		nodes.AddRange(positions);

		previousRailSegment = r;
		lastRailSpawnPosition = transform.position;
		return r;
	}

	// player just hopped on rail
	public int GetTargetIndex(List<RailSegment> rs, Vector3 collisionPosition) {
		var rnodes = rs.SelectMany( r => r.Nodes );
		var sortedNodes = rnodes.Select( (n,i) => new { position = n, index = i }).OrderBy( n => Vector2.Distance(n.position, collisionPosition) ).ToList();
		Vector3 selectedNode = sortedNodes[1].index > sortedNodes[0].index ?  sortedNodes[1].position : sortedNodes[0].position;

		var nodeIndex = nodes.IndexOf(selectedNode);
		return nodeIndex < 0 ? 0 : nodeIndex;
	}

	public Vector3 GetTargetPosition(int index) {
		return index < nodes.Count ? nodes[index] : Vector3.back;
	}
	
	public void RemoveNodes(int numNodes) {
		nodes.RemoveRange(0,numNodes);
		NodesRemoved?.Invoke(this, numNodes);
	}
}
