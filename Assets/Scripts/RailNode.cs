using System;
using UnityEngine;

public class RailNode {
	public Vector3 Direction;
	public Vector3 Position;
	public Vector3 Normal;
	public int index;
	public bool Valid;

	public static RailNode Invalid = new RailNode(0, Vector3.zero, Vector3.zero, Vector3.zero, false);

	public RailNode(int i, Vector3 p, Vector3 d, Vector3 n, bool valid) {
		index = i;
		Direction = d;
		Position = p;
		Normal = n;
		Valid = valid;
	}
	public RailNode(int i, Vector3 p, Vector3 d, Vector3 n): this(i, p, d, n, true) { }
	public RailNode(Vector3 p): this(0 ,p, Vector3.back, Vector3.back, true) { }
	public RailNode(Vector3 p, Vector3 d, Vector3 n): this(0, p, d, n, true) { }

	}

