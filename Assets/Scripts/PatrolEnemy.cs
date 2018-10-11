using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour, ICorruption {

	private RailNode targetNode;
	private Vector3 target1, target2;
	private float moveSpeed;
	private float maxDistance = 20f;

	private float lifetime;

	public float InitialDamage {
		get {
			return 10;
		}
	}
	public float SustainDamage {
		get {
			return 2;
		}
	}

	public void Start() {
		Init();
		transform.position = targetNode.Position;
		transform.localRotation = Quaternion.Euler(0, 0, Utils.VectorToAngle(targetNode.Direction));
		lifetime = 25f;
	}

	// Update is called once per frame
	void Update () {
		if ( Vector2.Distance(targetNode.Position, transform.position) > maxDistance ) {
			moveSpeed *= -1f;
		}
		transform.position += targetNode.Normal * moveSpeed * Time.deltaTime;

		lifetime -= Time.deltaTime;
		if (lifetime < 0) {
			Destroy(this.gameObject);
		}
	}

	public void Init() {
		Rail middleRail = GameManager.Instance.RailManager.GetRail(1);
		targetNode = middleRail.LastNode;

		target1 = targetNode.Position + targetNode.Normal * maxDistance;
		target2 = targetNode.Position - targetNode.Normal * maxDistance;

		moveSpeed = Random.Range(18, 25);
		moveSpeed *= Mathf.Sign(Random.value - 0.5f);
	}
}
