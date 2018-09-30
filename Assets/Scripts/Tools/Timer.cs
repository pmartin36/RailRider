using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {

	private TMP_Text timer;

	void Start () {
		timer = GetComponent<TMP_Text>();
	}
	
	void Update () {
		float elapsed = GameManager.Instance.LevelManager.Elapsed;
		timer.text = elapsed.ToString("F2");
	}
}
