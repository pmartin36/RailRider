using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPackage {
	public float Vertical { get; set; }
	public float Horizontal { get; set; }
	public bool Jump { get; set; }

	public bool Quit { get; set; }
	public bool Enter { get; set; }
}

public class InputManager : MonoBehaviour {

	InputPackage package;

	// Use this for initialization
	void Start () {
		package = new InputPackage();
	}
	
	// Update is called once per frame
	void Update () {
		package.Vertical = Input.GetAxis("Vertical");
		package.Horizontal = Input.GetAxis("Horizontal");
		package.Jump = Input.GetButtonDown("Jump");

		GameManager.Instance.ProcessInputs(package);
	}
}
