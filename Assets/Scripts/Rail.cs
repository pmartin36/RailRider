﻿using System;
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

	public RailNode LastNode {
		get {
			return nodes.Last();
		}
	}

	// Use this for initialization
	void Awake () {
		railManager = GetComponentInParent<RailManager>();
		lastRailSpawnPosition = transform.position;
		seed = UnityEngine.Random.Range(0, 1000000);
		nodes = new List<RailNode>();
		nodes.Add(new RailNode(new Vector2(35, transform.position.y), Vector3.right, Vector3.up));
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
			nodes.Add(RailNode.Invalid);
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
		var newNodes = r.CalculateNodes(size, lastRailSpawnPosition, this.transform.position);
		nodes.AddRange(newNodes);

		previousRailSegment = r;
		lastRailSpawnPosition = transform.position;
		return r;
	}

	// player just hopped on rail
	public int GetTargetIndex(List<RailSegment> rs, Vector3 collisionPosition) {
		var rnodes = rs.SelectMany( r => r.Nodes );
		var sortedNodes = rnodes.OrderBy( n => Vector2.Distance(n.Position, collisionPosition) ).ToList();
		RailNode selectedNode = sortedNodes.Count > 1 && sortedNodes[1].index > sortedNodes[0].index ?  sortedNodes[1] : sortedNodes[0];

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