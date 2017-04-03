using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct gameTile {
	public Vector2 position,parent;
	public float G,F;
}



public static class AStar {	
	// A star is used to find the optimal path to any point in the game by looking at which edges exists between nodes

	public static List<gameTile> navigate(Vector2 start, Vector2 dest, List<edge> graph) {

//		GameController gc = (GameController)GameObject.Find ("GameController").GetComponent (typeof(GameController));
		gameTile currentTile;
		gameTile adjacentTile;

		// the initialization phase
		List<gameTile> open = new List<gameTile> ();
		List<gameTile> closed = new List<gameTile> ();

		// add current position to closed list
		currentTile.position = start;
		currentTile.G = 0;
		currentTile.F = getH (currentTile.position, dest) + currentTile.G;
		currentTile.parent = currentTile.position;
		closed.Add(currentTile);

		// search through all the edges in the graph to find adjacent nodes to the current node
		for( int i = 0; i < graph.Count; i++) {
			if ((graph[i].p == currentTile.position) || (graph[i].q == currentTile.position)) {
				// create a new gameTile for this location when one of the nodes on this edge
				if (graph [i].p == currentTile.position) {
					adjacentTile.position = graph [i].q;
				} else {
					adjacentTile.position = graph [i].p;
				}
				adjacentTile.G = graph[i].cost;
				adjacentTile.F = getH (adjacentTile.position, dest) + adjacentTile.G;
				adjacentTile.parent = currentTile.position;
				open.Add (adjacentTile);
			} else {
				// should we add this spot to closed?
			}
		}

		int currentTileIndex = 0;

		// now comes the actual algorithm

		Vector2 testPos;
		int testIndex;

		// pathfind until all the tiles have been closed
		while (open.Count > 0) {
			// determine the next tile to inspect based on being the closest to destination
			int nextTileIndex = findLowestScoreIndex (currentTile.position, open);
			currentTileIndex = nextTileIndex;

			// retrieve this tile for inspection
			currentTile = open [nextTileIndex];

			// remove the tile from the open list
			open.RemoveAt(nextTileIndex);

			// add this tile to the closed list since it has been visited
			closed.Add (currentTile);

			// look through all the edges for nodes attached to this one
			for( int i = 0; i < graph.Count; i++) {
				if (graph[i].p == currentTile.position || graph[i].q == currentTile.position) {
					if (graph [i].p == currentTile.position) {
						testPos = graph [i].q;
					} else {
						testPos = graph [i].p;
					}

					// we can't add tiles that are already in the closed list
					if (findInList (testPos, closed) == -1) {
						// now that we know this is a viable tile, check if it's already been added
						testIndex = findInList (testPos, open);

						if (testIndex == -1) {	
							// it hasn't been added, so add it
							// create a new gameTile for this location
							adjacentTile.position = testPos;
							adjacentTile.G = currentTile.G + graph[i].cost;
							adjacentTile.F = getH (testPos, dest) + adjacentTile.G;
							adjacentTile.parent = currentTile.position;
							open.Add (adjacentTile);
						} else {
							// we've seen this tile before, so check to see if it's new F value is
							// improved from the G based on the current position
							if (currentTile.G + graph[i].cost < open [testIndex].G) {
								adjacentTile = open [testIndex];
								// update the G
								adjacentTile.G = currentTile.G + graph[i].cost;
								// update the F
								adjacentTile.F = getH (testPos, dest) + adjacentTile.G;

								// update the parent too
								adjacentTile.parent = currentTile.position;

								open [testIndex] = adjacentTile;
							}
						}
					}
				}
			}
		}

		return closed;
//		return buildPath(dest,closed);
	}

	public static float getH(Vector2 start, Vector2 dest) {
		// this is as simple as finding the distance from this position to the destination
		return (start - dest).magnitude;
	}

	public static int findLowestScoreIndex(Vector2 currentPos, List<gameTile> tiles) {
		float min = float.MaxValue;
		int minIndex = 0;
		// return the index of the tile with the lowest score
		for (int i = 0; i < tiles.Count; i++) {
			if (tiles[i].F < min) {
				// save the new minimum
				min = tiles [i].F;
				minIndex = i;
			} else if (tiles[i].F == min && tiles[i].parent == currentPos) {
				// break a tie by going with the tile adjacent to the most recently inspected tile
				min = tiles [i].F;
				minIndex = i;
			}
		}

		return minIndex;
	}

	public static int findInList(Vector2 loc, List<gameTile> tiles) {
		// cycle through the list and see if any tiles are at this location
		// return the index of the tile in the list if so, otherwise return -1
		for (int i = 0; i < tiles.Count; i++) {
			if (loc == tiles [i].position) {
				return i;
			}
		}

		return -1;
	}

//	public static List<Vector2> buildPath(Vector2 dest, List<gameTile> tiles) {
//		// walk backwards from the destination tile to the starting tile, appending a time
//		// (don't add the starting tile because we know where we are)
//
//		List<Vector2> fullPath = new List<Vector2> ();
//
//		Vector2 currentPos = findInList (dest, tiles);
//
//		while (.id != 0) {
//			// insert the position at the front of the path and, time-wise, at the end of the window
//			Vector2 newTile = tiles [currentID].position;
//			fullPath.Insert(0,newTile);
//			// update the current id
//			currentID = findInList(tiles [currentID].parent,tiles);
//		}
//
//		return fullPath;
//
//	}

	public static List<edge> ConvertToGraph(List<gameTile> allPaths) {
		// go through each of the tiles and create the edges. here, each node should only have one
		// edges ending at it but can have multiple edges coming from it

		List<edge> graphTree = new List<edge> ();

		edge e;

		foreach (gameTile tile in allPaths) {
			// do not include an edge to the first node (where the player starts) when finding
			// edges leading into a node, since there is no input edge
			if (tile.position != tile.parent) {
				e.q = tile.position;
				// the parent will be the starting edge
				e.p = tile.parent;

				e.cost = (e.q - e.p).magnitude;
				graphTree.Add (e);
			}
		}
		return graphTree;
	}
}
