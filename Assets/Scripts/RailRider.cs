using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailRider : MonoBehaviour {

	public Rail AttachedRail;
	protected Vector3 targetPosition = Vector3.forward;
	public float MaxSpeed;

	private float RailSpeed;
	protected float GravitySpeed;
	protected float Gravity;

	protected int RailIndex;
	protected Camera mainCamera;

	protected CircleCollider2D cc;

	public virtual void Start () {
		RailIndex = 1;
		ConnectToRail(AttachedRail);
		targetPosition = AttachedRail.GetTargetPosition(RailIndex);

		RailSpeed = MaxSpeed;
		Gravity = -10f;

		mainCamera = Camera.main;
		cc = GetComponent<CircleCollider2D>();
	}

	public virtual void Update() {
		if(AttachedRail == null) {
			GravitySpeed = Mathf.Min( Mathf.Abs(GravitySpeed + Gravity * Time.deltaTime), MaxSpeed) * Mathf.Sign(Gravity);

			var diff = new Vector3(RailSpeed, GravitySpeed) * Time.deltaTime;
			var newPosition = transform.position + diff;

			var viewportPosition = mainCamera.WorldToViewportPoint(newPosition);
			//if( (viewportPosition.x > 1 && diff.x > 0) ||
			//	(viewportPosition.x < 0 && diff.x < 0)) {
			//	newPosition.x *= -1;
			//}
			if( (viewportPosition.y > 1 && diff.y > 0) || 
				(viewportPosition.y < 0 && diff.y < 0)) {
				newPosition.y *= -1;
			}

			transform.position = newPosition;
		}
		else {
			float movement = RailSpeed * Time.deltaTime;
			while (AttachedRail != null && movement > 0) {
				float dist = Vector3.Distance(targetPosition, this.transform.position);
				if (dist > movement) {
					transform.position = Vector3.Lerp(transform.position, targetPosition, movement / dist);
					movement = 0;
				}
				else {
					movement -= dist;
					transform.position = targetPosition;
					RailIndex++;
					Vector3 nextPosition = AttachedRail.GetTargetPosition(RailIndex);
					if(nextPosition != Vector3.back) {
						targetPosition = nextPosition;
					}
					else {
						DisconnectFromRail();
					}
				}
			}
		}
	}

	public void ConnectToRail(Rail r) {
		AttachedRail = r;
		GravitySpeed = 0;
		Gravity = -10f;
		AttachedRail.NodesRemoved += RailRemovedNodes;
		StartCoroutine(CenterOnRail());
	}

	public void ConnectToRail(List<RailSegment> railSegments) {
		Rail r = railSegments.FirstOrDefault()?.parentRail;
		if (r != null) {
			ConnectToRail(r);
			RailIndex = AttachedRail.GetTargetIndex(railSegments, transform.position);
			targetPosition = AttachedRail.GetTargetPosition(RailIndex);
		}
	}

	public void DisconnectFromRail() {
		AttachedRail.NodesRemoved -= RailRemovedNodes;
		AttachedRail = null;
	}

	public void RailRemovedNodes(object sender, int numRemoved) {
		RailIndex = Mathf.Max(RailIndex - numRemoved, 0);
		Debug.Log(Time.time);
		Debug.Log(transform.position);
		Debug.Log(AttachedRail.GetTargetPosition(RailIndex));
	}

	public virtual void OnTriggerEnter2D(Collider2D collision) {
		if (AttachedRail == null && collision.gameObject.tag == "Rail") {
			
		}
	}

	public virtual void OnTriggerExit2D(Collider2D collision) {
		if (AttachedRail == null && collision.gameObject.tag == "Rail") {
			
		}
	}

	IEnumerator CenterOnRail() {
		//float startTime = Time.time;
		//float startY = transform.position.y;
		//while( AttachedRail != null && Mathf.Abs(AttachedRail.transform.position.y - transform.position.y) > 0.01f ) {
		//	transform.position = new Vector2( transform.position.x, Mathf.Lerp( startY, AttachedRail.transform.position.y, (Time.time - startTime) / 0.5f ));
		//	yield return new WaitForEndOfFrame();
		//}
		yield return null;
	}
}
