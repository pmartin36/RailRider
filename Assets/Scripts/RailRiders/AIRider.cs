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
					float diff = Mathf.Sign(Random.value - 0.5f);
					if (!AttemptDisconnectInDirectionWithCheck(diff)) {
						diff *= -1f;
						if (!AttemptDisconnectInDirectionWithCheck(diff)) {
							checksOnRail++;
						}
					}
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

	public bool AttemptDisconnectInDirectionWithCheck(float diff) {
		var proposedRailIndex = AttachedRail.RailIndex + diff;
		float direction = ((proposedRailIndex + RailManager.NumRails) % RailManager.NumRails) - AttachedRail.RailIndex;
		float directionSign = Mathf.Sign(direction);
		float proposedGravity = directionSign * Mathf.Abs(Gravity);
		float proposedGravitySpeed = 30f * direction;

		// hole avoidance
		Vector2 proposedDirection = (proposedGravitySpeed * target.Normal + target.Direction * RailSpeed * 0.8f).normalized;
		Vector2 startPosition = (Vector2)transform.position + proposedDirection * cc.radius * 2f * transform.localScale.x;
		RaycastHit2D hit = Physics2D.Raycast(startPosition, proposedDirection, 35 * Mathf.Abs(direction), 1 << LayerMask.NameToLayer("Rail"));
		Debug.DrawRay(startPosition, proposedDirection * 35 * Mathf.Abs(direction), Color.green, 5f);
		if (hit.collider != null) {
			Disconnect(proposedGravity, proposedGravitySpeed);
			return true;
		}
		return false;
	}

	public void DisconnectNoCheck(float diff) {
		var proposedRailIndex = AttachedRail.RailIndex + diff;
		float direction = Mathf.Sign(((proposedRailIndex + RailManager.NumRails) % RailManager.NumRails) - AttachedRail.RailIndex);
		Disconnect(direction * Mathf.Abs(Gravity), direction * 30f);
	}

	private void Disconnect(float proposedGravity, float speed) {
		Gravity = proposedGravity;
		GravitySpeed = speed;
		lastRail = AttachedRail;
		DisconnectFromRail();
	}

	public override void OverranRail() {
		StartFreeMovement();
	}

	protected void StartFreeMovement() {
		float diff = Mathf.Sign(Random.value - 0.5f);	
		if (!AttemptDisconnectInDirectionWithCheck(diff)) {
			diff *= -1f;
			DisconnectNoCheck(diff);
		}	
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
}
