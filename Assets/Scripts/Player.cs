﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : RailRider {

	private Coroutine boundaryBump;

	public override void Start () {
		GameManager.Instance.Player = this;
		base.Start();
	}

	public override void Update () {
		base.Update();
	}

	public void ProcessInputs(InputPackage p) {
		Vector2 inputDirection = new Vector2(p.Horizontal, p.Vertical);
		if (AttachedRail != null) {
			if ( p.Jump && Mathf.Abs(p.Vertical) > 0.2f ) {
				// jump
				float direction = Mathf.Sign(p.Vertical);
				Gravity = direction * Mathf.Abs(Gravity);
				GravitySpeed = 10f * direction;
				DisconnectFromRail();
				
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
			
			StopCoroutine(RecoverFromBoundaryBump());
			StartCoroutine(RecoverFromBoundaryBump());
		}
	}

	public override void OnTriggerExit2D(Collider2D collision) {

	}

	private IEnumerator RecoverFromBoundaryBump() {
		speedChangeDelta = 0.8f;
		yield return new WaitForSeconds(0.5f);
		speedChangeDelta = 0.2f;
	}
}
