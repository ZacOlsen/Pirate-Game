using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTreeNode<T> {

	public T data;
	public MTreeNode<T> parent;
	public List<MTreeNode<T>> children;

	public MTreeNode (T t) {
		data = t;
		children = new List<MTreeNode<T>> ();
	}

	public void AddChild (MTreeNode<T> node) {
		children.Add (node);
		node.parent = this;
	}

	public MTreeNode<T> GetRoot () {

		MTreeNode<T> node = this;
		while (this.parent != null) {
			node = node.parent;
		}

		return node;
	}
}
