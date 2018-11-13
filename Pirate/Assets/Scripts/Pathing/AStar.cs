using System.Collections.Generic;
using UnityEngine;

public static class AStar {

	private static Heap open;
	private static LevelLayout layout = new LevelLayout ();

	public static MyLinkedList<MovementTile> FindPath (MovementTile start, MovementTile finish, bool ignoreOtherCrew) {
		bool unused = false;
		return FindPath (start, finish, ignoreOtherCrew, finish, true, out unused);
	}

	public static MyLinkedList<MovementTile> FindPath (MovementTile start, MovementTile finish, bool ignoreOtherCrew,
		out bool couldFindPath) {
		return FindPath (start, finish, ignoreOtherCrew, finish, true, out couldFindPath);
	}

	private static MyLinkedList<MovementTile> FindPath (MovementTile start, MovementTile finish, 
		bool ignoreOtherCrew, MovementTile blockedException, bool startCanBeBlocked, out bool couldFindPath) {

		if (start.crewMem && !startCanBeBlocked && start != blockedException) {
			couldFindPath = false;
			return new MyLinkedList<MovementTile> ();
		}

		couldFindPath = true;

		if (start == finish) {
			MyLinkedList<MovementTile> path = new MyLinkedList<MovementTile> ();
			path.AddFirst (finish);
			return path;
		}

		if (start.layer != finish.layer) {

			List<LevelLayout.Portal> portalsForPath = layout.GetPortalsToLayerFromLayer (start.layer, finish.layer);
			int currentLayer = start.layer;
			MovementTile currentTile = start;

			MyLinkedList<MovementTile> totalPath = new MyLinkedList<MovementTile> ();
			for (int i = 0; i < portalsForPath.Count; i++) {
				
				MovementTile currentPortalTile = portalsForPath [i].GetHoleForLayer (currentLayer);
				MyLinkedList<MovementTile> pathPart = FindPath (currentTile, currentPortalTile, ignoreOtherCrew, 
					blockedException, i == 0, out couldFindPath); //very gross change to something better

				totalPath = MyLinkedList<MovementTile>.CombineLinkedLists (totalPath, pathPart);

				if (pathPart.Count > 0 && pathPart.last.data != currentPortalTile) {
					return totalPath;
				} else if (pathPart.Count == 0) {
					return totalPath;
				}

				currentLayer = portalsForPath [i].GetOtherLayer (currentLayer);
				currentTile = portalsForPath [i].GetHoleForLayer (currentLayer);
			}

			MyLinkedList<MovementTile> finalPathPart = FindPath (currentTile, finish, ignoreOtherCrew, 
				blockedException, false, out couldFindPath);
			totalPath = MyLinkedList<MovementTile>.CombineLinkedLists (totalPath, finalPathPart);

			return totalPath;
		}

		open.Add (start);
		start.goal = 0;

		start.fitness = DistBetween (start, finish);

		MovementTile closestAvailable = start;

		while (open.Count != 0) {
			
			MovementTile current = open.RemoveFirst ();
			current.closed = true;

			if (current == finish) {
				break;
			}

			MovementTile[] neighbors = GetTileNeighbors (current);

			for (int i = 0; i < neighbors.Length; i++) {

				if (neighbors [i] == null || !neighbors [i].walkable || neighbors [i].closed || 
					((neighbors[i].shipPos || (!ignoreOtherCrew && neighbors[i].crewMem)) && 
					neighbors[i] != blockedException)) {
					continue;
				}

				float tempGoal = current.goal + (DistBetween (current, neighbors [i]) * neighbors [i].weight);

				if (!open.Contains (neighbors [i])) {
					neighbors [i].prev = current;
					neighbors [i].goal = tempGoal;
					neighbors [i].fitness = neighbors [i].goal + DistBetween (neighbors [i], finish);
					open.Add (neighbors [i]);
				
					if (neighbors [i].fitness < closestAvailable.fitness) {
						closestAvailable = neighbors [i];
					}

				} else if (tempGoal >= neighbors [i].goal) {
					continue;
				}
			}
		}

//		PrintPathRetrace (finish);

		if (finish.prev == null) {
			couldFindPath = false;
		//	Debug.LogWarning ("could not path to finish");
			CleanMap (start.layer);
			return CreatePath (closestAvailable);
		}

		MyLinkedList<MovementTile> p = CreatePath (finish);
		CleanMap (start.layer);
		return p;
	}
		
	private static float DistBetween (MovementTile current, MovementTile neighbor) {
		float x = current.x - neighbor.x;
		float y = current.y - neighbor.y;
		return x * x + y * y;
	}

	private static void PrintPathRetrace (MovementTile end) {
		
		while (end != null) {
			Debug.Log (end.x + ", " + end.y);
			end = end.prev;
		}
	}

	public static MovementTile[] GetTileNeighbors (MovementTile tile) {

		MovementTile[] neighbors = new MovementTile [4];
		neighbors [0] = RangeCheckTile (tile.x + 1, tile.y, tile.layer);
		neighbors [1] = RangeCheckTile (tile.x, tile.y + 1, tile.layer);
		neighbors [2] = RangeCheckTile (tile.x - 1, tile.y, tile.layer);
		neighbors [3] = RangeCheckTile (tile.x, tile.y - 1, tile.layer);

		return neighbors;
	}

	private static MovementTile RangeCheckTile (int x, int y, int layer) {
		return x >= 0 && x < layout.GetLength (0, layer) && y >= 0 && y < layout.GetLength (1, layer) ?
			layout.GetTile (x, y, layer) : null;
	}

	private static MyLinkedList<MovementTile> CreatePath (MovementTile finish) {

		MyLinkedList<MovementTile> path = new MyLinkedList<MovementTile> ();

		while (finish != null) {
			path.AddFirst (finish);
			finish = finish.prev;
		}

		return path;
	}
		
	private static void CleanMap (int layer) {

		for (int x = 0; x < layout.GetLength (0, layer); x++) {
			for (int y = 0; y < layout.GetLength (1, layer); y++) {
				layout.GetTile (x, y, layer).closed = false;
				layout.GetTile (x, y, layer).goal = float.MaxValue;
				layout.GetTile (x, y, layer).fitness = float.MaxValue;
				layout.GetTile (x, y, layer).prev = null;
				layout.GetTile (x, y, layer).heapIndex = 0;
			}
		}
				
		open.Clear ();
	}

	public static MovementTile GetRandomTileOnLayer (int layer) {

		int xMax = layout.GetLength (0, layer);
		int yMax = layout.GetLength (1, layer);
		int x = Random.Range (0, xMax);
		int y = Random.Range (0, yMax);

		return layout.GetTile (x, y, layer);
	}

	public static MovementTile GetTile (int x, int y, int layer) {

		int xMax = layout.GetLength (0, layer);
		int yMax = layout.GetLength (1, layer);

		if(x < 0 || x >= xMax || y < 0 || y >= yMax) {
			return null;
		}

		return layout.GetTile (x, y, layer);
	}

	public static void AddAreaToLayout (MovementTile[,] area) {
		layout.areas.Add (area);

		int numOfTiles = area.GetLength (0) * area.GetLength (1);

		if (open == null) {
			open = new Heap (numOfTiles);
			return;
		}

		int largestSize = layout.GetLargestAreaSize ();
		if (open.GetMaxSize () < largestSize) {
			open = new Heap (largestSize);
		}
	}

	public static void AddPortalToLayout (LevelLayout.Portal portal) {
		layout.portals.Add (portal);
		layout.GeneratePortalGraph ();
	}

	public static void RemoveLayerFromLayout (int layer) {

		for (int i = 0; i < layout.areas.Count; i++) {
			if (layout.areas [i] [0, 0].layer == layer) {
				layout.areas.RemoveAt (i);
				break;
			}
		}

		for (int i = 0; i < layout.portals.Count; i++) {
			if (layout.portals [i].ContainsLayer (layer)) {
				layout.portals.RemoveAt (i);
				i--;
			}
		}
	}
}