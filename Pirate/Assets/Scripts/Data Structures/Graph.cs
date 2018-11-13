using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph<T> {

	public class GraphNode {

		public T data;
		public List<GraphNode> adjacent;
	
		public bool visited;
		public GraphNode prev;

		public GraphNode(T t) {
			data = t;
			adjacent = new List<GraphNode> ();
		}
	}

	public List<GraphNode> nodes = new List<GraphNode> ();

	public void Add (T tNew, List<T> adjacents) {

		GraphNode g = new GraphNode (tNew);
		for (int i = 0; i < adjacents.Count; i++) {
			for (int j = 0; j < nodes.Count; j++) {
				if (adjacents [i].Equals (nodes [j].data)) {
					g.adjacent.Add (nodes [j]);
					nodes [j].adjacent.Add (g);
					break;
				}
			}
		}

		nodes.Add (g);
	}

	public void Remove (T t) {

		for (int i = 0; i < nodes.Count; i++) {

			if (nodes [i].data.Equals (t)) {
				nodes.RemoveAt (i);
				i--;
				continue;
			}

			for (int j = 0; j < nodes [i].adjacent.Count; j++) {
				if (nodes [i].adjacent [j].data.Equals (t)) {
					nodes [i].adjacent.RemoveAt (j);
				}
			}
		}
	}

	public List<T> FindShortestPath (T tFrom, T tTo) {

		//will find path in reverse order to traverse in proper order
		GraphNode current = null;
		for (int i = 0; i < nodes.Count; i++) {
			if (nodes [i].data.Equals (tTo)) {
				current = nodes [i];
				break;
			}
		}
			
		Queue<GraphNode> searchQueue = new Queue<GraphNode> ();
		searchQueue.Enqueue (current);

		while (searchQueue.Count > 0) {
			current = searchQueue.Dequeue ();

			if (current.visited) {
				continue;
			}

			if (current.data.Equals (tFrom)) {
				break;
			}

			current.visited = true;
			for (int i = 0; i < current.adjacent.Count; i++) {

				if (current.adjacent [i].visited) {
					continue;
				}

				current.adjacent [i].prev = current;
				searchQueue.Enqueue (current.adjacent [i]);
			}
		}

		List<T> path = new List<T> ();
		while (current != null) {
			path.Add (current.data);
			current = current.prev;
		}

		CleanGraph ();

		return path;
	}

	private void CleanGraph () {

		for (int i = 0; i < nodes.Count; i++) {
			nodes [i].prev = null;
			nodes [i].visited = false;
		}
	}
}
