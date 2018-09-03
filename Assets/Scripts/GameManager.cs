using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputManager))]
public class GameManager : Singleton<GameManager> {

	public bool Menu { get; set; }
	public Player Player;


	public void Awake() {
		
	}

	public void Start () {

	}

	public void ProcessInputs(InputPackage p) {
		if(p.Quit) {
			Application.Quit();
		}

		if(Menu) {
			
		}
		else {
			Player.ProcessInputs(p);
		}
	}

	public void ReloadLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void ToggleSoundOn() {
		
	}

	public void PlayerLost(string reason) {

	}
}