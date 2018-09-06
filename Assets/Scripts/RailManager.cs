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

	void Start () {
		Rails = GetComponentsInChildren<Rail>().ToList();

		lastRailSpawnTime = Time.time;
		numRailsSinceLastCorruption = 0;
		lastRailSpawnPosition = transform.position;
		nextRailSpawnPosition = lastRailSpawnPosition + Vector3.right * RailSize;

		SetRailSpawnTimes();
	}

	public void SetRailSpawnTimes() {
		TimeBetweenRailSpawns = RailSize / RailSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		float time = Time.time;
		float tdiff = (time - lastRailSpawnTime);
		float percent = tdiff/TimeBetweenRailSpawns;
		transform.position = Vector3.Lerp(lastRailSpawnPosition, nextRailSpawnPosition, percent);

		if(percent >= 1) {
			var lastSpawnAngle = spawnAngle;
			spawnAngle +=  Perlin.Noise(Time.time/5f) * 40;
			var diff = spawnAngle - lastSpawnAngle;

			transform.Rotate(0, 0, diff);

			numRailsSinceLastCorruption++;

			if (numRailsSinceLastCorruption >= CorruptionOccurrence) {
				for (int i = 0; i < Rails.Count; i++) {
					if (i == nextCorruptedRailIndex) {
						Rails[i].SpawnCorruptedRail(RailSize, diff);
					}
					else {
						Rails[i].SpawnRail(RailDensity, RailSize, diff);
					}
				}
				numRailsSinceLastCorruption = 0;
			}
			else if(numRailsSinceLastCorruption >= CorruptionOccurrence - 1 ) {
				// find next corrupted rail and make sure that there's a rail before it
				nextCorruptedRailIndex = Random.Range(0, Rails.Count);
				for (int i = 0; i < Rails.Count; i++) {
					if (i == nextCorruptedRailIndex) {
						Rails[i].SpawnRail(1, RailSize, diff);
					}
					else {
						Rails[i].SpawnRail(RailDensity, RailSize, diff);
					}
				}
			}
			else {
				for (int i = 0; i < Rails.Count; i++) {
					Rails[i].SpawnRail(RailDensity, RailSize, diff);
				}
			}

			lastRailSpawnTime = lastRailSpawnTime + TimeBetweenRailSpawns;
			lastRailSpawnPosition = this.transform.position;
			nextRailSpawnPosition = lastRailSpawnPosition + Utils.AngleToVector(spawnAngle) * RailSize;
		}
	}
}
