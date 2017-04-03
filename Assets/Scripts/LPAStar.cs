using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LPAStar{

	private List<gameTile> paths;

	public static List<gameTile> GetInitialPaths(Vector2 start, Vector2 end, List<edge> graph) {
		List<gameTile> initialPaths = AStar.navigate(start,end,graph);

		return initialPaths;
	}

//	public static List<Vector2> GetPath() {
//
//	}
}
