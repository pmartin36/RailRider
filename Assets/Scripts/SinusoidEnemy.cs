using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidEnemy : MonoBehaviour, EnemyType, ICorruption {

	Transform child;
	public float Speed;
	public float Amplitude;
	private Vector2 lastPositionClipSpace;

	private Vector2 conversion;

	public float InitialDamage {
		get {
			return 2;
		}
	}

	public float SustainDamage {
		get {
			return 2;
		}
	}

	void Start () {
		Init();
		conversion = new Vector2(Camera.main.orthographicSize * 2 * Camera.main.aspect, Camera.main.orthographicSize * 2);
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 clipSpacePosition = lastPositionClipSpace + new Vector2(Speed * Time.deltaTime, 0);
		transform.position = Camera.main.ViewportToWorldPoint(clipSpacePosition);

		if (clipSpacePosition.x < -1f) {
			DestroySelf();
		}
		else {
			Vector2 nextClipSpace = clipSpacePosition;
			Vector2 nextWorldSpace = new Vector2( clipSpacePosition.x, Amplitude * Mathf.Sin(nextClipSpace.x * 2 * Mathf.PI));

			int count = transform.childCount;

			for (int i = 0; i < count; i++) {
				Transform child = transform.GetChild(i);
				float newX = nextClipSpace.x + i * child.lossyScale.x / conversion.x;
				float newY = nextClipSpace.y + Amplitude * Mathf.Sin(newX * 2 * Mathf.PI);
				child.position = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(newX, newY));
			}

			lastPositionClipSpace = clipSpacePosition;
		}
	}

	public void HeadCollected() {
		foreach(Transform child in transform) {
			Destroy(child.gameObject);
		}
		DestroySelf();
	}

	public void Init() {
		lastPositionClipSpace = new Vector3(1.1f, Random.Range(0.2f, 0.8f));
		transform.position = Camera.main.ViewportToWorldPoint(lastPositionClipSpace);
	}

	public void DestroySelf() {
		Destroy(this.gameObject);
	}
}
