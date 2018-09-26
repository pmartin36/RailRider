using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, ContextManager {

	public Player Player;
	public RailManager RailManager;
	public float Elapsed;

	// Use this for initialization
	void Start () {
		GameManager.Instance.LevelManager = this;
	}
	
	// Update is called once per frame
	void Update () {
		Elapsed += Time.deltaTime;
	}

	public void ProcessInputs(InputPackage p) {
		Player?.ProcessInputs(p);
	}
}
