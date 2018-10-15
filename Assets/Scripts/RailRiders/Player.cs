using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : RailRider {

	public static event EventHandler<float> PlayerPowerChanged;
	private Coroutine boundaryBump;
	private float charge;

	private List<RechargeMarker> overlappingMarkers;
	private List<ICorruption> collidedCorruptions;

	private float jumpFudge;

	public override void Start () {
		GameManager.Instance.Player = this;
		charge = 100;
		collidedCorruptions = new List<ICorruption>();
		overlappingMarkers = new List<RechargeMarker>();
		StartCoroutine(DelayedInit());
		base.Start();
	}

	public override void Update () {
		Vector3 startPosition = mainCamera.Camera.WorldToViewportPoint(transform.position);
		base.Update();

		float chargeDiff = 0;
		foreach (ICorruption c in collidedCorruptions) {
			chargeDiff -= c.SustainDamage;
		}
		if ( AttachedRail == null ) {
			chargeDiff -= 3;
		} 
		else if( target.Corrupted ) {
			chargeDiff -= 8;
		}
		else {
			chargeDiff -= 2;
		}

		ModifyCharge(chargeDiff * Time.deltaTime);

		HandleScreenWrap(startPosition, transform.position);
		jumpFudge -= Time.deltaTime;
	}

	private void ModifyCharge(float diff){
		charge = Mathf.Clamp(charge+diff, 0, 100);
		if (charge <= 0) {
			//ded
		}
		else {
			PlayerPowerChanged?.Invoke(this, charge);
		}
	}

	public void ProcessInputs(InputPackage p) {
		Vector2 inputDirection = new Vector2(p.Horizontal, p.Vertical);

		for(int i = 0; i < overlappingMarkers.Count; i++) {
			RechargeMarker m = overlappingMarkers[i];
			if (m.IsConditionMet(p.Jump, AttachedRail != null, p.Vertical)) {
				if( m is SingleMarker || m is SinusoidHead ) {
					ModifyCharge(m.Value);
				}
				
				m.ActivatedAction();

				overlappingMarkers.RemoveAt(i);
				i--;
			}
		}

		float pVertAbs = Mathf.Abs(p.Vertical);
		float pHoriAbs = Mathf.Abs(p.Horizontal);
		if (AttachedRail != null) {
			if ( p.Jump ) {
				if (pVertAbs > 0.2f) {
					float direction = Mathf.Sign(p.Vertical);
					Gravity = direction * Mathf.Abs(Gravity);
					GravitySpeed = 20f * direction;
					if(targetSpeed > RailSpeed|| targetSpeed < 0) {
						targetSpeed *= 0.5f;
					}
					DisconnectFromRail();
				}
				
			}
			else if( pHoriAbs > 0.2f ) {
				if (p.Horizontal < 0) {
					// slow down
					targetSpeed = Speed * 0.8f * p.Horizontal;
				}
				else {
					// speed up
					targetSpeed = Speed * 2.25f * p.Horizontal;
				}
			}	
			else {
				targetSpeed = Speed;
			}
		}
		else  {
			if (p.Jump) {
				//should probably use nonAlloc version - fix later for performance
				// 1.25 because we want some fudge factor if the player overshoots the rail
				ConnectToRailByOverlap(cc.radius * 1.25f * transform.localScale.x);

				if (AttachedRail == null && jumpFudge < 0) {
					jumpFudge = 0.10f;
				}
			}
		}
	}

	public override void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Rail" && AttachedRail == null && jumpFudge >= 0) {
			ConnectToRail(new List<RailSegment>() { collision.GetComponent<RailSegment>() });
		}
		else if(collision.gameObject.tag == "ScreenBoundary") {
			float dir = mainCamera.Camera.WorldToViewportPoint(transform.position).x;
			RailSpeed = Speed * (dir > 0.5f ? -2.0f : 2.0f);
			targetSpeed = Speed;

			StopCoroutine(RecoverFromBoundaryBump());
			StartCoroutine(RecoverFromBoundaryBump());
		}
		else if (collision.gameObject.tag == "RechargeMarker") {
			overlappingMarkers.Add(collision.GetComponent<RechargeMarker>());
		}
		else if (collision.gameObject.tag == "RechargePellet") {
			var pellet = collision.GetComponent<RechargePellet>();
			if( pellet == null ) {
				var sin = collision.GetComponent<SinusoidHead>();
				ModifyCharge(sin.Value);
				sin.ActivatedAction();
			}
			else {
				ModifyCharge(pellet.Value);
				pellet.ActivatedAction();
			}
		}
		else if (collision.tag == "Corruption") {
			var comp =	(collision.GetComponent<CorruptedTrail>() as ICorruption) ??
						(collision.GetComponent<PatrolEnemy>() as ICorruption) ??
						collision.transform.parent?.GetComponent<SinusoidEnemy>() as ICorruption;
			collidedCorruptions.Add(comp);
			ModifyCharge(-comp.InitialDamage);
		}
	}


	public override void OnTriggerExit2D(Collider2D collision) {
		if (collision.gameObject?.tag == "RechargeMarker") {
			overlappingMarkers.Remove(collision.GetComponent<RechargeMarker>());
		}
		else if (collision.tag == "Corruption") {
			var comp =	(collision.GetComponent<CorruptedTrail>() as ICorruption) ??
						(collision.GetComponent<PatrolEnemy>() as ICorruption) ??
						collision.transform.parent?.GetComponent<SinusoidEnemy>() as ICorruption;
			collidedCorruptions.Remove(comp);
		}
	}

	private IEnumerator RecoverFromBoundaryBump() {
		speedChangeDelta = 0.8f;
		yield return new WaitForSeconds(0.5f);
		speedChangeDelta = 0.2f;
	}
}
