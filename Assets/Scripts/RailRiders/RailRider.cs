using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailRider : MonoBehaviour {

	public Rail AttachedRail;
	public RailNode target = RailNode.Invalid;
	protected float distanceToTarget;
	protected float distanceToCenter;
	protected bool CanAttachToRail;

	public float Speed;
	protected float targetSpeed;
	protected float speedx;
	protected Vector2 speedy;

	protected float speedChangeDelta;

	protected float RailSpeed;
	protected float LastFrameRailSpeed;
	protected float GravitySpeed;
	protected float Gravity;
	protected Coroutine centerOnRailRoutine;

	public int RailIndex;

	protected MainCamera mainCamera;

	protected CircleCollider2D cc;

	public virtual void Start () {
		RailIndex = 0;
		RailSpeed = Speed;
		Gravity = -30f;

		mainCamera = Camera.main.GetComponent<MainCamera>();
		cc = GetComponent<CircleCollider2D>();

		targetSpeed = Speed;
		speedChangeDelta = 0.2f;
		CanAttachToRail = true;
	}

	public virtual void Update() {
		RailSpeed = Mathf.SmoothDamp(RailSpeed, targetSpeed, ref speedx, speedChangeDelta);
		if (AttachedRail == null) {
			FreeMovement();
		}
		else {
			RailMovement();
		}
		LastFrameRailSpeed = RailSpeed;
	}

	public void HandleScreenWrap(Vector3 start, Vector3 end) {
		var viewportPosition = mainCamera.Camera.WorldToViewportPoint(end);
		var diff = viewportPosition - start;
		if ((viewportPosition.y > 1 && diff.y > 0) ||
			(viewportPosition.y < 0 && diff.y < 0)) {
			if (AttachedRail != null) {
				DisconnectFromRail();
			}
			viewportPosition.y = 1 - viewportPosition.y;
			transform.position = mainCamera.Camera.ViewportToWorldPoint(viewportPosition);
		}
	}

	public virtual void FreeMovement() {
		GravitySpeed = GravitySpeed + Gravity * Time.deltaTime;
		if( Mathf.Abs(GravitySpeed) > 40f ) {
			GravitySpeed = 40f * Mathf.Sign(Gravity);
		}

		RailNode camTarget = mainCamera.Anchor.target;
		Vector2 direction = Utils.AngleToVector(mainCamera.Anchor.transform.localRotation.eulerAngles.z);
		Vector2 normal = direction.Rotate(90);

		Vector3 diff = direction * RailSpeed + normal * GravitySpeed + mainCamera.Anchor.LastDiff;

		var newPosition = transform.position + diff * Time.deltaTime;

		transform.position = newPosition;
	}

	public virtual void RailMovement() {
		float movement = RailSpeed * Time.deltaTime;
		if( Mathf.Sign(LastFrameRailSpeed * RailSpeed) < 0f ){
			// we changed direction
			SetNextTarget();
		}
		while (AttachedRail != null) {
			if (Mathf.Abs(distanceToTarget) > Mathf.Abs(movement)) {
				transform.position += movement * target.Direction;
				distanceToTarget -= movement;
				break;
			}
			else {
				transform.position += distanceToTarget * target.Direction;
				movement -= distanceToTarget;
				SetNextTarget();
			}
		}

	}

	public virtual void SetNextTarget() {
		SetNextIndex();
		SetTarget();
	}

	public virtual void OverranRail() {
		Gravity = -Mathf.Abs(Gravity);
		GravitySpeed = Mathf.Abs(RailSpeed) / 20f;
		DisconnectFromRail();
	}

	public virtual void SetTarget() {
		RailNode nextPosition = AttachedRail.GetTargetRailNode(RailIndex);
		if (nextPosition.Valid) {
			target = nextPosition;

			var dist = Vector2.Distance(transform.position, target.Position);
			var angle = Vector2.SignedAngle( transform.position - target.Position, target.Direction ) * Mathf.Deg2Rad;

			distanceToTarget = dist * -Mathf.Cos(angle);
			distanceToCenter = dist * Mathf.Sin(angle);
		}
		else {
			OverranRail();
		}
	}

	public void ConnectToRail(Rail r) {
		if(CanAttachToRail) {
			AttachedRail = r;
			GravitySpeed = 0;
			AttachedRail.NodesRemoved += RailRemovedNodes;
		}
	}

	public virtual void ConnectToRail(List<RailSegment> railSegments) {
		Rail r = railSegments.FirstOrDefault()?.parentRail;
		if (r != null) {
			ConnectToRail(r);
			if(AttachedRail != null) {
				RailIndex = AttachedRail.GetTargetIndex(railSegments, transform.position);
				SetTarget();

				if(centerOnRailRoutine != null) StopCoroutine(centerOnRailRoutine);
				centerOnRailRoutine = StartCoroutine(CenterOnRail());
			}
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
		StartCoroutine(DisableAttachToRail());
		//RailSpeed = Mathf.Clamp(RailSpeed, Speed * 0.5f, Speed * 1.5f);
	}

	protected void SetNextIndex() {	
		if (RailSpeed >= 0) {
			RailIndex++;
		}
		else {
			RailIndex--;
		}
	}

	public void RailRemovedNodes(object sender, int numRemoved) {
		// Debug.Log(gameObject.name + " " + AttachedRail.name);
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
		float movement = Mathf.Abs(Gravity) / 5f * Time.deltaTime * Mathf.Sign(distanceToCenter);
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

	protected IEnumerator DisableAttachToRail() {
		CanAttachToRail = false;
		yield return new WaitForSeconds(0.25f);
		CanAttachToRail = true;
	}
}
