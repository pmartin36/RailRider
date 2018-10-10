using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRider : RailRider {

	public float Lifetime;
	protected float timeOnRail; 
	protected int checksOnRail;
	private Rail lastRail;

	public override void Start() {		
		base.Start();
		targetSpeed = Speed * 2f;
		RailSpeed = targetSpeed;
		checksOnRail = 1;
	}

	public override void Update() {
		base.Update();
		if(checksOnRail > 0 && AttachedRail != null) {
			// check every 0.5s 
			if(timeOnRail * 2f > checksOnRail + 1) {
				if (Random.value > 1f / (1 + checksOnRail)) {
					StartFreeMovement();
				}
				else {
					checksOnRail++;
				}
			}
			timeOnRail += Time.deltaTime;
		}

		Lifetime -= Time.deltaTime;
		if(Lifetime < 0) {
			DestroySelf();
		}
	}

	public override void OverranRail() {
		StartFreeMovement();
	}

	protected void StartFreeMovement() {
		var proposedDirection = AttachedRail.RailIndex - 1;
		float direction = Mathf.Sign(((proposedDirection + RailManager.NumRails) % RailManager.NumRails) - AttachedRail.RailIndex);
		Gravity = direction * Mathf.Abs(Gravity);
		GravitySpeed = 30f * direction;
		lastRail = AttachedRail;

		DisconnectFromRail();
	}

	public override void FreeMovement() {
		GravitySpeed = GravitySpeed + Gravity * Time.deltaTime;
		Vector3 diff = target.Direction * RailSpeed + target.Normal * GravitySpeed;
		transform.position = transform.position + diff * Time.deltaTime;
	}

	public virtual void DestroySelf() {
		if(AttachedRail != null){
			AttachedRail.NodesRemoved -= RailRemovedNodes;
		}
		Destroy(this.gameObject);
	}

	public override void OnTriggerEnter2D(Collider2D collision) {
		if (AttachedRail == null && CanAttachToRail) {
			if (collision.gameObject.tag == "Rail") {
				timeOnRail = 0;
				checksOnRail = 1;
				ConnectToRail(new List<RailSegment>() { collision.GetComponent<RailSegment>() });	
			}
		}
	}

	protected override IEnumerator CenterOnRail() {
		float movement = Mathf.Abs(Gravity) / 5f * Time.deltaTime * Mathf.Sign(distanceToCenter);
		while (AttachedRail != null && target != null) {
			if (Mathf.Abs(movement) > Mathf.Abs(distanceToCenter)) {
				transform.position += distanceToCenter * target.Normal;
				yield break;
			}
			else {
				transform.position += movement * target.Normal;
				distanceToCenter -= movement;
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
