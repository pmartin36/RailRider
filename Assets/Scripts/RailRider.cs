using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailRider : MonoBehaviour {

	public Rail AttachedRail;
	public RailNode target = RailNode.Invalid;
	protected float distanceToTarget;
	protected float distanceToCenter;
	public float MaxSpeed;

	private float RailSpeed;
	protected float GravitySpeed;
	protected float Gravity;
	protected Vector3 GravityDirection;
	protected Coroutine centerOnRailRoutine;

	public int RailIndex;

	protected Camera mainCamera;

	protected CircleCollider2D cc;

	public virtual void Start () {
		RailIndex = 0;
		ConnectToRail(AttachedRail);
		SetTarget();

		RailSpeed = MaxSpeed;
		Gravity = 15f;

		mainCamera = Camera.main;
		cc = GetComponent<CircleCollider2D>();
	}

	public virtual void Update() {
		if(AttachedRail == null) {
			FreeMovement();
		}
		else {
			RailMovement();
		}
	}

	public virtual void FreeMovement() {
		GravitySpeed = Mathf.Min(GravitySpeed + Gravity * Time.deltaTime, MaxSpeed);

		var diff = target.Direction * RailSpeed + GravityDirection * GravitySpeed;
		var newPosition = transform.position + diff * Time.deltaTime;

		var viewportPosition = mainCamera.WorldToViewportPoint(newPosition);
		//if( (viewportPosition.x > 1 && diff.x > 0) ||
		//	(viewportPosition.x < 0 && diff.x < 0)) {
		//	newPosition.x *= -1;
		//}
		if ((viewportPosition.y > 1 && diff.y > 0) ||
			(viewportPosition.y < 0 && diff.y < 0)) {
			newPosition.y *= -1;
		}

		transform.position = newPosition;
	}

	public virtual void RailMovement() {
		float movement = RailSpeed * Time.deltaTime;
		while (AttachedRail != null && movement > 0) {
			if (distanceToTarget > movement) {
				transform.position += target.Direction * movement;
				distanceToTarget -= movement;
				movement = 0;
			}
			else {
				movement -= distanceToTarget;
				transform.position += target.Direction * distanceToTarget;
				RailIndex++;
				SetTarget();
			}
		}
	}

	public virtual void SetTarget() {
		RailNode nextPosition = AttachedRail.GetTargetRailNode(RailIndex);
		if (nextPosition.Valid) {
			target = nextPosition;

			var dist = Vector2.Distance(transform.position, target.Position);
			var angle = Vector2.SignedAngle( transform.position - target.Position, target.Direction ) * Mathf.Deg2Rad;

			distanceToTarget = dist * Mathf.Abs( Mathf.Cos(angle) );
			distanceToCenter = dist * Mathf.Sin(angle);
		}
		else {
			GravityDirection = -target.Normal;
			DisconnectFromRail();
		}
	}

	public void ConnectToRail(Rail r) {
		AttachedRail = r;
		GravitySpeed = 0;
		AttachedRail.NodesRemoved += RailRemovedNodes;
	}

	public void ConnectToRail(List<RailSegment> railSegments) {
		Rail r = railSegments.FirstOrDefault()?.parentRail;
		if (r != null) {
			ConnectToRail(r);
			RailIndex = AttachedRail.GetTargetIndex(railSegments, transform.position);
			SetTarget();

			if(centerOnRailRoutine != null) StopCoroutine(centerOnRailRoutine);
			centerOnRailRoutine = StartCoroutine(CenterOnRail());
		}
	}

	public virtual void DisconnectFromRail() {
		AttachedRail.NodesRemoved -= RailRemovedNodes;
		AttachedRail = null;
	}

	public void RailRemovedNodes(object sender, int numRemoved) {
		RailIndex = Mathf.Max(RailIndex - numRemoved, 0);
	}

	public virtual void OnTriggerEnter2D(Collider2D collision) {
		if (AttachedRail == null && collision.gameObject.tag == "Rail") {
			
		}
	}

	public virtual void OnTriggerExit2D(Collider2D collision) {
		if (AttachedRail == null && collision.gameObject.tag == "Rail") {
			
		}
	}

	protected virtual IEnumerator CenterOnRail() {
		float movement = 2f * Time.deltaTime * Mathf.Sign(distanceToCenter);
		while (AttachedRail != null && target != null) {		
			if( Mathf.Abs(movement) > Mathf.Abs(distanceToCenter) ) {
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
