using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Dictionary<string, Queue<PoolObject>> {

	private PoolManager() { }
	private static PoolManager _instance = new PoolManager();
	public static PoolManager Instance { 
		get {
			return _instance ?? new PoolManager();
		}
	}

	public void CreatePool<T>(T po) where T : MonoBehaviour, PoolObject{
		this.Add(po.Key, new Queue<PoolObject>());

		for(int i = 0; i < po.StartingCount; i++) {
			T o = GameObject.Instantiate(po);
			o.gameObject.SetActive(false);
			this[po.Key].Enqueue(o);
		}
	}

	public T Next<T>(string key) where T : MonoBehaviour, PoolObject {
		var next = this[key].Dequeue() as T;

		// if we're taking the last object, create another one in its place
		if (this[key].Count == 1) {
			T o = GameObject.Instantiate(next);
			o.gameObject.SetActive(false);
			this[key].Enqueue(o);
		}

		next.gameObject.SetActive(true);
		return next;	
	}

	public void Recycle(PoolObject po) {
		this[po.Key].Enqueue(po);
	}

	public void RemovePool(string key) {
		this.Remove(key);
	}
}
