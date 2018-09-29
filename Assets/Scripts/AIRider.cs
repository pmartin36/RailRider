using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRider : RailRider {

	public float Lifetime;
	protected float timeOnRail; 
	protected int checksOnRail;

	public override void Start() {		
		base.Start();
		targetSpeed = Speed * 2f;
		checksOnRail = 1;

		ConnectToRail(GameManager.Instance.RailManager.GetAnotherRail(null));
		SetNextTarget();
		transform.position = target.Position;
	}

	public override void Update() {
		base.Update();
		if(AttachedRail != null) {
			if(timeOnRail * 2f > checksOnRail) {
				if (Random.value > 1f / (1 + checksOnRail)) {
					var proposedDirection = AttachedRail.RailIndex -1;
					float direction = Mathf.Sign(((proposedDirection + 3) % 3) - AttachedRail.RailIndex);
					Gravity = direction * Mathf.Abs(Gravity);
					GravitySpeed = 10f * direction;
					DisconnectFromRail();
				}
				checksOnRail++;
			}
			timeOnRail += Time.deltaTime;
		}

		Lifetime -= Time.deltaTime;
		if(Lifetime < 0) {
			DestroySelf();
		}
	}

	public virtual void DestroySelf() {
		Destroy(this.gameObject);
	}

	public override void FreeMovement() {
		GravitySpeed = GravitySpeed + Gravity * Time.deltaTime;
		if (Mathf.Abs(GravitySpeed) > 30f) {
			GravitySpeed = 30f * Mathf.Sign(Gravity);
		}

		Vector3 diff = target.Direction * RailSpeed + target.Normal * GravitySpeed;
		transform.position = transform.position + diff * Time.deltaTime;
	}

	public override void OnTriggerEnter2D(Collider2D collision) {
		if (AttachedRail == null && CanAttachToRail) {
			if (collision.gameObject.tag == "Rail") {
				checksOnRail = 1;
				timeOnRail = 0;
				ConnectToRail(new List<RailSegment>() { collision.GetComponent<RailSegment>() });
			}
		}
	}
}
