using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalAdjacencyListNode<T> {

	public T data;
	public List<DirectionalAdjacencyListNode<T>> children;
	public List<DirectionalAdjacencyListNode<T>> parents;

	public DirectionalAdjacencyListNode (T t) {
		data = t;
		children = new List<DirectionalAdjacencyListNode<T>>();
		parents = new List<DirectionalAdjacencyListNode<T>>();
	}

	public void AddChild (DirectionalAdjacencyListNode<T> node) {
		children.Add (node);
		node.parents.Add (this);
	}
}
