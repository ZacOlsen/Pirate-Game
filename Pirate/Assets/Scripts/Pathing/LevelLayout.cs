using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayout {

	public List<MovementTile[,]> areas;
	public List<Portal> portals;
	public Graph<int> portalGraph;

	[System.Serializable]
	public struct WalkableArea {
		public Transform area;
		public Vector2Int dimensions;
	}

	[System.Serializable]
	public class Portal {
		public MovementTile hole1;
		public MovementTile hole2;

		public Portal (MovementTile hole1, MovementTile hole2) {
			this.hole1 = hole1;
			this.hole2 = hole2;
		}

		public bool IsForLayers (int layer1, int layer2) {
			return ContainsLayer (layer1) && ContainsLayer (layer2);
		}

		public bool ContainsLayer (int layer) {
			return hole1.layer == layer || hole2.layer == layer;
		}

		public int GetOtherLayer (int layer) {
			return layer == hole1.layer ? hole2.layer : hole1.layer;
		}

		public MovementTile GetHoleForLayer (int layer) {
			return hole1.layer == layer ? hole1 : hole2;
		}
	}

	//public LevelLayout (List<MovementTile[,]> areas, List<Portal> portals) {
	public LevelLayout () {
		this.areas = new List<MovementTile[,]> ();
		this.portals = new List<Portal> ();

		GeneratePortalGraph ();
	}

	public void GeneratePortalGraph () {

		portalGraph = new Graph<int> ();
		for (int i = 0; i < areas.Count; i++) {
			
			List<int> adjacents = new List<int> ();
			for (int j = 0; j < portals.Count; j++) {

				if (portals [j].ContainsLayer (areas [i] [0, 0].layer)) {
					adjacents.Add (portals [j].GetOtherLayer (areas [i] [0, 0].layer));
				}
			}

			portalGraph.Add (areas [i] [0, 0].layer, adjacents);
		}
	}

	public int GetLength (int dimension, int layer) {
		return GetArea (layer).GetLength (dimension);
	}

	/**
	 * may need to be refined based on game complexity
	 */
	public List<Portal> GetPortalsToLayerFromLayer (int fromLayer, int toLayer) {

		List<Portal> portalsToTake = new List<Portal> ();
		List<int> portalJumps = portalGraph.FindShortestPath (fromLayer, toLayer);

		//will have gaurentee at least 1 jump is used
		for (int i = 1; i < portalJumps.Count; i++) {
			for (int j = 0; j < portals.Count; j++) {

				if (portals [j].IsForLayers (portalJumps [i - 1], portalJumps [i])) {
					portalsToTake.Add (portals [j]);
					break;
				}
			}
		}

		return portalsToTake;
	}

	private MovementTile[,] GetArea (int layer) {

		for (int i = 0; i < areas.Count; i++) {
			if (layer == areas [i] [0, 0].layer) {
				return areas [i];
			}
		}

		Debug.LogError ("area doesnt exist");
		return null;
	}

	public MovementTile GetTile (int x, int y, int layer) {
		return GetArea (layer) [x, y];
	}

	public int GetLargestAreaSize () {

		int size = 0;
		for (int i = 0; i < areas.Count; i++) {
			
			int currentSize = areas [i].GetLength (0) * areas [i].GetLength (1);
			if (size < currentSize) {
				size = currentSize;
			}
		}

		return size;
	}
}
