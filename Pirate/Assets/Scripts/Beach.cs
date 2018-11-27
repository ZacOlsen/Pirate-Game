using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beach : MonoBehaviour {

	public LevelLayout.WalkableArea area;
	public MovementTile boardingTile;

	protected MovementTile[,] islandTiles;

	void Awake () {

		Transform island = area.area;
		islandTiles = new MovementTile [16, 8];

		for (int x = 0; x < islandTiles.GetLength (0); x++) {
			for (int y = 0; y < islandTiles.GetLength (1); y++) {
				islandTiles [x, y] = island.GetChild (x * 8 + y).GetComponent<MovementTile> ();
				islandTiles [x, y].x = x;
				islandTiles [x, y].y = y;
			}
		}

		AStar.AddAreaToLayout (islandTiles);
	}

	void OnDestroy () {
		AStar.RemoveLayerFromLayout (islandTiles [0, 0].layer);
	}

	public MovementTile GetRandomTile () {
		return islandTiles [Random.Range(0, islandTiles.GetLength (0)), Random.Range(0, islandTiles.GetLength (1))];
	}
}
