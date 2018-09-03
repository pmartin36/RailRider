using System;
using UnityEngine;

public class RailNode {
	public Vector3 Direction;
	public Vector3 Position;
	public Vector3 Normal;
	public int index;

	public static RailNode Invalid = new RailNode(Vector3.back);

	public RailNode(int i, Vector3 p, Vector3 d, Vector3 n) {
		index = i;
		Direction = d;
		Position = p;
		Normal = n;
	}
	public RailNode(Vector3 p): this(0 ,p, Vector3.back, Vector3.back) { }
	public RailNode(Vector3 p, Vector3 d, Vector3 n): this(0, p, d, n) { }

	}

