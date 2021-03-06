﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatManager : MonoBehaviour {

	private float nextThreatSpawnTime;
	public List<EnemyListType> EnemyTypes;

	// Use this for initialization
	public void Start () {
		GameManager.Instance.ThreatManager = this;
		nextThreatSpawnTime = Time.time + UnityEngine.Random.Range(10f, 12f);
	}
	
	// Update is called once per frame
	void Update () {
		if ( Time.time > nextThreatSpawnTime ) {
			int index = UnityEngine.Random.Range(0, EnemyTypes.Count);

			GameObject o = Instantiate(EnemyTypes[index].gameObject);
			var type = EnemyTypes[index].type;
			if (type == "Sinusoid") {
				nextThreatSpawnTime = Time.time + UnityEngine.Random.Range(10f, 15f);

				var t = o.GetComponent<SinusoidEnemy>();
				t.Init();
			}
			else if (type == "CorruptedAIRider") {
				nextThreatSpawnTime = Time.time + UnityEngine.Random.Range(15f, 20f);

				var t = o.GetComponent<CorruptedAIRider>();
				t.Init();
			}
			else {
				nextThreatSpawnTime = Time.time + UnityEngine.Random.Range(15f, 20f);

				var t = o.GetComponent<PatrolEnemy>();
				t.Init();
			}
		}
	}
}

[System.Serializable]
public class EnemyListType {
	public string type;
	public GameObject gameObject;
}

public interface EnemyType {
	void Init();
}
