using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RailManager : MonoBehaviour {

	private List<Rail> Rails;

	[Range(0.2f,1)]
	public float RailDensity;
	public float RailSpeed;
	[Range(3, 10)]
	public int CorruptionOccurrence;

	public float RailSize;

	float lastRailSpawnTime;
	Vector3 lastRailSpawnPosition;
	Vector3 nextRailSpawnPosition;

	private int numRailsSinceLastCorruption;
	private int nextCorruptedRailIndex;
	private float TimeBetweenRailSpawns;
	private float spawnAngle;

	public int segmentIndex = 0;

	void Start () {
		GameManager.Instance.RailManager = this;
		Rails = GetComponentsInChildren<Rail>().ToList();

		lastRailSpawnTime = Time.time;
		numRailsSinceLastCorruption = 0;
		lastRailSpawnPosition = transform.position;
		
		SetRailSpawnTimes();

		var y = transform.position.y;
		for (int x = -80; x < 80; x += 20) {
			float kt = (3f / 80f) * x + 17;
			transform.position = new Vector3(x, y);

			for (int i = 0; i < Rails.Count; i++) {
				Rails[i].SpawnRail(1, RailSize, 0, segmentIndex, kt);
			}

			segmentIndex++;
			lastRailSpawnPosition = this.transform.position;
		}
		nextRailSpawnPosition = lastRailSpawnPosition + Vector3.right * RailSize;
	}

	public void SetRailSpawnTimes() {
		TimeBetweenRailSpawns = RailSize / RailSpeed;
	}

	public Rail GetAnotherRail(Rail r) {	
		Rail rail;
		do {
			int random = Random.Range(0, Rails.Count);
			rail = Rails[random];
		} while(rail == r);
		return rail;
	}

	public void AddRail() {
		var lastSpawnAngle = spawnAngle;
		spawnAngle += Perlin.Noise(Time.time / 5f) * 30 - 15;
		var diff = spawnAngle - lastSpawnAngle;

		transform.position = nextRailSpawnPosition;
		transform.Rotate(0, 0, diff);

		numRailsSinceLastCorruption++;

		if (numRailsSinceLastCorruption >= CorruptionOccurrence) {
			for (int i = 0; i < Rails.Count; i++) {
				if (i == nextCorruptedRailIndex) {
					Rails[i].SpawnCorruptedRail(RailSize, diff, segmentIndex);
				}
				else {
					Rails[i].SpawnRail(RailDensity, RailSize, diff, segmentIndex);
				}
			}
			numRailsSinceLastCorruption = 0;
		}
		else if (numRailsSinceLastCorruption >= CorruptionOccurrence - 1) {
			// find next corrupted rail and make sure that there's a rail before it
			nextCorruptedRailIndex = Random.Range(0, Rails.Count);
			for (int i = 0; i < Rails.Count; i++) {
				if (i == nextCorruptedRailIndex) {
					Rails[i].SpawnRail(1, RailSize, diff, segmentIndex);
				}
				else {
					Rails[i].SpawnRail(RailDensity, RailSize, diff, segmentIndex);
				}
			}
		}
		else {
			for (int i = 0; i < Rails.Count; i++) {
				Rails[i].SpawnRail(RailDensity, RailSize, diff, segmentIndex);
			}
		}

		segmentIndex++;
		lastRailSpawnTime = lastRailSpawnTime + TimeBetweenRailSpawns;
		lastRailSpawnPosition = this.transform.position;
		nextRailSpawnPosition = lastRailSpawnPosition + Utils.AngleToVector(spawnAngle) * RailSize;
	}

	public void AddRails(int num) {
		for(int n = 0; n < num; n++) {
			AddRail();
		}
	}
	
	// Update is called once per frame
	void Update () {
		//float time = Time.time;
		//float tdiff = (time - lastRailSpawnTime);
		//float percent = tdiff/TimeBetweenRailSpawns;
		//transform.position = Vector3.Lerp(lastRailSpawnPosition, nextRailSpawnPosition, percent);

		//if(percent >= 1) {
		//	var lastSpawnAngle = spawnAngle;
		//	spawnAngle +=  Perlin.Noise(Time.time/5f) * 30 - 15;
		//	var diff = spawnAngle - lastSpawnAngle;

		//	transform.Rotate(0, 0, diff);

		//	numRailsSinceLastCorruption++;

		//	if (numRailsSinceLastCorruption >= CorruptionOccurrence) {
		//		for (int i = 0; i < Rails.Count; i++) {
		//			if (i == nextCorruptedRailIndex) {
		//				Rails[i].SpawnCorruptedRail(RailSize, diff);
		//			}
		//			else {
		//				Rails[i].SpawnRail(RailDensity, RailSize, diff);
		//			}
		//		}
		//		numRailsSinceLastCorruption = 0;
		//	}
		//	else if(numRailsSinceLastCorruption >= CorruptionOccurrence - 1 ) {
		//		// find next corrupted rail and make sure that there's a rail before it
		//		nextCorruptedRailIndex = Random.Range(0, Rails.Count);
		//		for (int i = 0; i < Rails.Count; i++) {
		//			if (i == nextCorruptedRailIndex) {
		//				Rails[i].SpawnRail(1, RailSize, diff);
		//			}
		//			else {
		//				Rails[i].SpawnRail(RailDensity, RailSize, diff);
		//			}
		//		}
		//	}
		//	else {
		//		for (int i = 0; i < Rails.Count; i++) {
		//			Rails[i].SpawnRail(RailDensity, RailSize, diff);
		//		}
		//	}

		//	lastRailSpawnTime = lastRailSpawnTime + TimeBetweenRailSpawns;
		//	lastRailSpawnPosition = this.transform.position;
		//	nextRailSpawnPosition = lastRailSpawnPosition + Utils.AngleToVector(spawnAngle) * RailSize;
		//}
	}
}
