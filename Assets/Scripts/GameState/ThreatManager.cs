using System;
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
			dynamic t = o.GetComponent(EnemyTypes[index].type);
			t.Init();

			nextThreatSpawnTime = Time.time + UnityEngine.Random.Range(15f, 20f);
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
