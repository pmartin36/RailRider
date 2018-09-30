using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface PoolObject {
	string Key { get; set; }
	int StartingCount { get; set; }

	void Recycle();
}

