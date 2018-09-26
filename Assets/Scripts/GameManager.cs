using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(InputManager))]
public class GameManager : Singleton<GameManager> {

	private ContextManager ContextManager;
	public LevelManager LevelManager {
		get {
			return ContextManager is LevelManager ? 
					ContextManager as LevelManager : null;
		}
		set {
			ContextManager = value; 
		}
	}
	public MenuManager MenuManager {
		get
		{
			return ContextManager is MenuManager ?
					ContextManager as MenuManager : null;
		}
		set
		{
			ContextManager = value;
		}
	}

	public Player Player {
		get {
			return (ContextManager as LevelManager)?.Player;
		}
		set {
			(ContextManager as LevelManager).Player = value;
		}
	}
	public RailManager RailManager {
		get {
			return (ContextManager as LevelManager)?.RailManager;
		}
		set {
			(ContextManager as LevelManager).RailManager = value;
		}
	}


	public void Awake() {
		
	}

	public void Start () {

	}

	public void ProcessInputs(InputPackage p) {
		if(p.Quit) {
			Application.Quit();
		}

		ContextManager.ProcessInputs(p);
	}

	public void ReloadLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void ToggleSoundOn() {
		
	}

	public void PlayerLost(string reason) {

	}
}