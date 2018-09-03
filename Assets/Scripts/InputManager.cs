using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPackage {
	public float Vertical { get; set; }

	public bool Left { get; set; }
	public bool Right { get; set; }

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

		package.Jump = Input.GetButtonDown("Jump");
		if ( Input.GetButton("Join") ) {
			var j = Input.GetAxisRaw("Join");
			package.Left = j < 0;
			package.Right = j > 0;
		}
		else {
			package.Left = false;
			package.Right = false;
		}

		GameManager.Instance.ProcessInputs(package);
	}
}
