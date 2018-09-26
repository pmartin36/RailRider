using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour, ContextManager {

	// Use this for initialization
	void Start () {
		GameManager.Instance.MenuManager = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ProcessInputs(InputPackage p) {
		throw new System.NotImplementedException();
	}
}
