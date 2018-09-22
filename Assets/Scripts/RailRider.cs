using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailRider : MonoBehaviour {

	public Rail AttachedRail;
	public RailNode target = RailNode.Invalid;
	protected float distanceToTarget;
	protected float distanceToCenter;

	public float Speed;
	protected float targetSpeed;
	protected float speedx;

	protected float speedChangeDelta;

	protected float RailSpeed;
	protected float GravitySpeed;
	protected float Gravity;
	protected Coroutine centerOnRailRoutine;

	public int RailIndex;

	protected MainCamera mainCamera;

	protected CircleCollider2D cc;

	public virtual void Start () {
		RailIndex = 0;
		RailSpeed = Speed;
		Gravity = -15f;
		StartCoroutine(DelayedInit());

		mainCamera = Camera.main.GetComponent<MainCamera>();
		cc = GetComponent<CircleCollider2D>();

		targetSpeed = Speed;
		speedChangeDelta = 0.2f;
	}

	public virtual void Update() {
		Vector3 startPosition = mainCamera.Camera.WorldToViewportPoint(transform.position);
		if(AttachedRail == null) {
			FreeMovement();
		}
		else {
			RailMovement();
		}

		RailSpeed = Mathf.SmoothDamp(RailSpeed, targetSpeed, ref speedx, speedChangeDelta);

		var viewportPosition = mainCamera.Camera.WorldToViewportPoint(transform.position);
		var diff = viewportPosition - startPosition;
		if ((viewportPosition.y > 1 && diff.y > 0) ||
			(viewportPosition.y < 0 && diff.y < 0)) {
			if(AttachedRail != null) {
				DisconnectFromRail();
			}
			viewportPosition.y = 1 - viewportPosition.y;
			transform.position = mainCamera.Camera.ViewportToWorldPoint(viewportPosition);
		}	
	}

	public virtual void FreeMovement() {
		GravitySpeed = GravitySpeed + Gravity * Time.deltaTime;
		if( Mathf.Abs(GravitySpeed) > 12f ) {
			GravitySpeed = 12f * Mathf.Sign(Gravity);
		}

		//RailSpeed = 5 * Time.deltaTime;
		//var diff = target.Direction * RailSpeed + GravityDirection * GravitySpeed;
		RailNode camTarget = mainCamera.Anchor.target;
		var diff = camTarget.Direction * RailSpeed + camTarget.Normal * GravitySpeed;
		var newPosition = transform.position + diff * Time.deltaTime;

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
			Gravity = -1f * Mathf.Abs(Gravity);
			DisconnectFromRail();
		}
	}

	public void ConnectToRail(Rail r) {
		AttachedRail = r;
		GravitySpeed = 0;
		AttachedRail.NodesRemoved += RailRemovedNodes;
	}

	public virtual void ConnectToRail(List<RailSegment> railSegments) {
		Rail r = railSegments.FirstOrDefault()?.parentRail;
		if (r != null) {
			ConnectToRail(r);
			RailIndex = AttachedRail.GetTargetIndex(railSegments, transform.position);
			SetTarget();

			if(centerOnRailRoutine != null) StopCoroutine(centerOnRailRoutine);
			centerOnRailRoutine = StartCoroutine(CenterOnRail());
		}
	}

	protected void ConnectToRailByOverlap(float radius = 0.25f) {
		List<RailSegment> rails = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer("Rail"))
								.Select(r => r?.gameObject.GetComponent<RailSegment>())
								.Where(r => r != null && r.SegmentIndex >= target.SegmentIndex).ToList();
		if (rails.Count > 0) {
			ConnectToRail(rails);
		}
	}

	public virtual void DisconnectFromRail() {
		AttachedRail.NodesRemoved -= RailRemovedNodes;
		AttachedRail = null;
		//RailSpeed = Mathf.Clamp(RailSpeed, Speed * 0.5f, Speed * 1.5f);
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

	protected IEnumerator DelayedInit() {
		yield return new WaitForEndOfFrame();
		ConnectToRailByOverlap();
	}
}
