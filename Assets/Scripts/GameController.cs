using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public struct vertex
//{
//	public int id;
//	public Vector2 location;
//}
//
//public struct edge
//{
//	public vertex p,q;
//}

public class GameController : MonoBehaviour {

	private List<List<bool>> map;
	private Vector2 playerLocation;
	public GameObject ground;
	public GameObject borderPrefab;
	private Vector2 size;


	// Use this for initialization
	void Start () {
		map = new List<List<bool>> ();
		BuildInitialMap ();
		DrawMap ();
		size = 10f * new Vector2 (ground.transform.localScale.x, ground.transform.localScale.z);
		print (size);
	}
	
	// Update is called once per frame
	void Update () {
		RandomMapChange ();
		DrawMap ();
	}

	private void BuildInitialMap() {
		// build an initial grid that is essentially a network of straight roads 

		// the map can hold 12 x 12 cells to build the network, so the initial map
		// will simply have roads set as the locations at odd columns and rows (zero indexed)

		for (int i = 0; i < size.x; i++) {
			map[i] = new List<bool>;
			for (int j = 0; j < size.y; j++) {
				if (i%2==1 && j%2==1){
					map[i][j] = true;
				} else {
					map[i][j] = false;
				}
			}
		}
	}

	private void RandomMapChange() {
		// with a certain probability, add a connection to the "cut"

	}

	private void DrawMap() {

	}

}
