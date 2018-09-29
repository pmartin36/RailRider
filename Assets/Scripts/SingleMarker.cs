using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkerType {
	In,
	Up,
	Down,
	Out
}

public class SingleMarker : RechargeMarker {

	MarkerType MarkerType;
	public float value;
	private SpriteRenderer spriteRenderer;
	private static Sprite InMarkerSprite;
	private static Sprite DownMarkerSprite;
	private static Sprite OutMarkerSprite;

	// Use this for initialization
	public override void Awake () {
		MarkerContainer = MarkerContainer ?? GameObject.FindGameObjectWithTag("MarkerContainer").transform;
		transform.parent = MarkerContainer;
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Init() {
		MarkerType = (MarkerType)Random.Range(0, 4);
		switch (MarkerType) {
			case MarkerType.In:
				InMarkerSprite = InMarkerSprite ?? Resources.Load<Sprite>("Sprites/arrow_in");
				spriteRenderer.sprite = InMarkerSprite;
				value = 10f;
				break;
			case MarkerType.Up:
				DownMarkerSprite = DownMarkerSprite ?? Resources.Load<Sprite>("Sprites/down_arrow_out");
				spriteRenderer.sprite = DownMarkerSprite;
				spriteRenderer.flipY = true;
				value = 20f;
				break;
			case MarkerType.Down:
				DownMarkerSprite = DownMarkerSprite ?? Resources.Load<Sprite>("Sprites/down_arrow_out");
				spriteRenderer.sprite = DownMarkerSprite;
				spriteRenderer.flipY = false;
				value = 20f;
				break;
			default:
			case MarkerType.Out:
				OutMarkerSprite = OutMarkerSprite ?? Resources.Load<Sprite>("Sprites/both_arrow_out");
				spriteRenderer.sprite = OutMarkerSprite;
				value = 15f;
				break;
		}
	}

	public new static SingleMarker Create() {
		var name = "Single-Marker";
		PoolManager pm = PoolManager.Instance;
		if (!pm.ContainsKey(name)) {
			SingleMarker prefab = Resources.Load<SingleMarker>($"Prefabs/{name}");
			prefab.Key = name;
			pm.CreatePool(prefab);
		}
		SingleMarker seg = pm.Next(name) as SingleMarker;
		return seg;
	}

	public override bool IsConditionMet(float f) {
		switch (MarkerType) {
			case MarkerType.In: 
				return Mathf.Abs(f) < 0.3f;
			case MarkerType.Up:
				return f > 0.5f;
			case MarkerType.Down:
				return f < -0.5f;
			default:
			case MarkerType.Out:
				return Mathf.Abs(f) > 0.5f;
		}
	}
}
