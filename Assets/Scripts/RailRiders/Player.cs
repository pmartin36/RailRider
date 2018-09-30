using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : RailRider {

	public static event EventHandler<float> PlayerPowerChanged;
	private Coroutine boundaryBump;
	private float charge;
	private RechargeMarker overlappingMarker;

	public override void Start () {
		GameManager.Instance.Player = this;
		charge = 100;
		base.Start();
	}

	public override void Update () {
		Vector3 startPosition = mainCamera.Camera.WorldToViewportPoint(transform.position);
		base.Update();
		if( AttachedRail == null ) {
			ModifyCharge(-3 * Time.deltaTime);
		} 
		else if( target.Corrupted ) {
			ModifyCharge(-8 * Time.deltaTime);
		}
		else {
			ModifyCharge(-2 * Time.deltaTime);
		}

		HandleScreenWrap(startPosition, transform.position);
	}

	private void ModifyCharge(float diff){
		charge += diff;
		if (charge <= 0) {

		}
		else {
			PlayerPowerChanged?.Invoke(this, charge);
		}
	}

	public void ProcessInputs(InputPackage p) {
		Vector2 inputDirection = new Vector2(p.Horizontal, p.Vertical);

		if (overlappingMarker != null) {		
			if (overlappingMarker.IsConditionMet(p.Jump, AttachedRail != null, p.Vertical)) {
				if( overlappingMarker is SingleMarker ) {
					SingleMarker m = overlappingMarker as SingleMarker;
					ModifyCharge(m.Value);
				}
				
				overlappingMarker.ActivatedAction();
				overlappingMarker = null;
			}
		}

		if (AttachedRail != null) {
			if ( p.Jump ) {
				if (Mathf.Abs(p.Vertical) > 0.2f) {
					float direction = Mathf.Sign(p.Vertical);
					Gravity = direction * Mathf.Abs(Gravity);
					GravitySpeed = 10f * direction;
					DisconnectFromRail();
				}
				
			}
			else if( Mathf.Abs(p.Horizontal) > 0.2f ) {
				if (p.Horizontal < 0) {
					// slow down
					targetSpeed = -Speed;
				}
				else {
					// speed up
					targetSpeed = Speed * 3f;
				}
			}	
			else {
				targetSpeed = Speed;
			}
		}
		else  {
			if (p.Jump) {
				//should probably use nonAlloc version - fix later for performance
				ConnectToRailByOverlap(cc.radius * transform.localScale.x);
			}
		}
	}

	public override void OnTriggerEnter2D(Collider2D collision) {
		if(collision.gameObject.tag == "ScreenBoundary") {
			float dir = mainCamera.Camera.WorldToViewportPoint(transform.position).x;
			RailSpeed = Speed * (dir > 0.5f ? -5f : 5f);
			targetSpeed = Speed;

			StopCoroutine(RecoverFromBoundaryBump());
			StartCoroutine(RecoverFromBoundaryBump());
		}
		else if (collision.gameObject.tag == "RechargeMarker") {
			overlappingMarker = collision.GetComponent<RechargeMarker>();
		}
		else if (collision.gameObject.tag == "RechargePellet") {
			ModifyCharge(4);
			collision.GetComponent<RechargePellet>().ActivatedAction();
		}
		else if (collision.tag == "CorruptedTrail") {
			ModifyCharge(-4);
		}
	}

	public void OnTriggerStay2D(Collider2D collision) {
		if (collision.tag == "CorruptedTrail") {
			ModifyCharge(-6 * Time.deltaTime);
		}
	}

	public override void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject.tag == "RechargeMarker") {
			overlappingMarker = null;
		}
	}

	private IEnumerator RecoverFromBoundaryBump() {
		speedChangeDelta = 0.8f;
		yield return new WaitForSeconds(0.5f);
		speedChangeDelta = 0.2f;
	}
}
