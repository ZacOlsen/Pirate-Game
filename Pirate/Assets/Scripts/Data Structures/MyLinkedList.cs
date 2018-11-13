using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyLinkedList<T> {

	public int Count {
		private set;
		get;
	}

	public MyLinkedListNode first;
	public MyLinkedListNode last;

	[System.Serializable]
	public class MyLinkedListNode {
		public T data;
		public MyLinkedListNode next = null;
		public MyLinkedListNode previous = null;

		public MyLinkedListNode (T t) {
			data = t;
		}
	}

	public MyLinkedList () {
		Count = 0;
	}

	public void AddFirst (T t) {

		MyLinkedListNode n = new MyLinkedListNode (t);

		if (first != null) {
			first.previous = n;
			n.next = first;
			first = n;

		} else {
			first = n;
			last = n;
		}

		Count++;
	}

	public void AddLast (T t) {

		MyLinkedListNode n = new MyLinkedListNode (t);

		if (last != null) {
			last.next = n;
			n.previous = last;
			last = n;

		} else {
			first = n;
			last = n;
		}

		Count++;
	}

	public void RemoveFirst () {
		first = first.next;
		if (first != null) {
			first.previous = null;
		}

		Count--;
	}

	public void RemoveLast () {
		last = last.previous;
		if (last != null) {
			last.next = null;
		}

		Count--;
	}

	public void Empty () {
		Count = 0;
		first = null;
		last = null;
	}

	/**
	 * modifies both lists
	 */
	 public static MyLinkedList<T> CombineLinkedLists (MyLinkedList<T> list1, MyLinkedList<T> list2) {

		if (list1.Count == 0) {
			return list2;
		} else if (list2.Count == 0) {
			return list1;
		}

		MyLinkedList<T> list = new MyLinkedList<T> ();
		list.first = list1.first;
		list.last = list2.last;

		list1.last.next = list2.first;
		list2.first.previous = list1.last;

		list.Count = list1.Count + list2.Count;
		return list;
	}
}
