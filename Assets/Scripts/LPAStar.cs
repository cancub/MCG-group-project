using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPAStar{

	private List<edge> paths;

	public static List<edge> GetInitialPaths(Vector2 start, Vector2 end, List<edge> graph) {
		List<edge> initialPaths = AStar.navigate(start,end,graph);

		return initialPaths;
	}

//	public static List<Vector2> GetPath() {
//
//	}
}
