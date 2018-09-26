using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerMeter : MonoBehaviour {

	Image meter;
	
	void Start () {
		meter = GetComponent<Image>();
		Player.PlayerPowerChanged += PlayerPowerChanged;
	}
	
	
	void Update () {
		
	}

	private void OnDestroy() {
		Player.PlayerPowerChanged -= PlayerPowerChanged;
	}

	public void PlayerPowerChanged(object sender, float value) {
		transform.localScale = new Vector3(value / 100f, 1, 0);
		if(value > 70f) {
			meter.color = Color.green;
		}
		else if(value > 30f) {
			meter.color = Color.yellow;
		}
		else {
			meter.color = Color.red;
		}
	}
}
