using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CSharpQuadTree{
	public class QuadTree<T> where T : class, IQuadObject{
		private readonly int maxDeep;
		private QuadNode root = null;
		private Dictionary<T, QuadNode> objectToNodeLookup = new Dictionary<T, QuadNode> ();
		public QuadNode Root { get { return root; } }
		public QuadTree (float x,float y,float width,float height, int maxDeep){
			this.maxDeep = maxDeep;
			root = new QuadNode (new Rect (x, y, width, height),1);
		}
		public void Insert (T quadObject){
			InsertNodeObject (root, quadObject);
		}
		public List<T> Query (Rect bounds){
			List<T> results = new List<T> ();
			if (root != null)
				Query (bounds, root, results);
			return results;
		}
		private void Query (Rect bounds, QuadNode node, List<T> results){
			if (node == null)
				return;
			if (IntersectsWith (bounds, node.theBounds)) {
				foreach (T quadObject in node.Objects) {
					if (IntersectsWith (bounds, quadObject.theBounds))
						results.Add (quadObject);
				}
				foreach (QuadNode childNode in node.Nodes) {
					Query (bounds, childNode, results);
				}
			}
		}
		private bool IntersectsWith (Rect a, Rect b){
			return 	a.x < b.xMax && a.xMax > b.x &&
					a.y < b.yMax && a.yMax > b.y;
		}
		private void InsertNodeObject (QuadNode node, T quadObject){
			if (!node.theBounds.Contains (new Vector3 (quadObject.theBounds.x, quadObject.theBounds.y, 0f))){
				Debug.Log("This should not happen, child does not fit within node bounds");
				Remove(quadObject);
				return;
			}
			if (!node.HasChildNodes () && node.deep<maxDeep) {
				SetupChildNodes (node);
				List<T> childObjects = new List<T> (node.Objects);
				List<T> childrenToRelocate = new List<T> ();
				foreach (T childObject in childObjects) {
					foreach (QuadNode childNode in node.Nodes) {
						if (childNode == null)
							continue;
						if (childNode.theBounds.Contains (new Vector3 (childObject.theBounds.x, childObject.theBounds.y, 0f))) {
							childrenToRelocate.Add (childObject);
						}
					}
				}
				foreach (T childObject in childrenToRelocate) {
					RemoveQuadObjectFromNode (childObject);
					InsertNodeObject (node, childObject);
					return;
				}
			}
			foreach (QuadNode childNode in node.Nodes) {
				if (childNode != null) {
					if (childNode.theBounds.Contains (new Vector3 (quadObject.theBounds.x, quadObject.theBounds.y, 0f))) {
						InsertNodeObject (childNode, quadObject);
						return;
					}
				}
			}
			AddQuadObjectToNode (node, quadObject);
		}
		private void RemoveQuadObjectFromNode (T quadObject){
			QuadNode node = objectToNodeLookup [quadObject];
			node.quadObjects.Remove (quadObject);
			objectToNodeLookup.Remove (quadObject);
			quadObject.inTree = false;
			quadObject.Changed -= new EventHandler (quadObject_moved);
		}
		private void AddQuadObjectToNode (QuadNode node, T quadObject)
		{
			node.quadObjects.Add (quadObject);
			objectToNodeLookup.Add (quadObject, node);
			quadObject.Changed += new EventHandler (quadObject_moved);
			quadObject.inTree = true;
		}
		void quadObject_moved (object sender, EventArgs e){
			T quadObject = sender as T;
			if (quadObject != null) {
				QuadNode node = objectToNodeLookup [quadObject];
				if (!node.theBounds.Contains (new Vector3 (quadObject.theBounds.x, quadObject.theBounds.y, 0f)) || node.HasChildNodes ()) {
					RemoveQuadObjectFromNode (quadObject);
					Insert (quadObject);
				}
			}
		}
		private void SetupChildNodes (QuadNode node){
				node [Direction.TL] = new QuadNode (node.theBounds.x, node.theBounds.y, node.theBounds.width / 2,
																  node.theBounds.height / 2,node.deep+1);
				node [Direction.TR] = new QuadNode (node.theBounds.x + node.theBounds.width / 2, node.theBounds.y,
																  node.theBounds.width / 2,
																  node.theBounds.height / 2,node.deep+1);
				node [Direction.BL] = new QuadNode (node.theBounds.x, node.theBounds.y + node.theBounds.height / 2,
																  node.theBounds.width / 2,
																  node.theBounds.height / 2,node.deep+1);
				node [Direction.BR] = new QuadNode (node.theBounds.x + node.theBounds.width / 2,
																  node.theBounds.y + node.theBounds.height / 2,
																  node.theBounds.width / 2, node.theBounds.height / 2,node.deep+1);
		}
		public void Remove (T quadObject){
			if (!objectToNodeLookup.ContainsKey (quadObject))
				return;
			RemoveQuadObjectFromNode (quadObject);
		}
		private List<T> GetChildObjects (QuadNode node){
			List<T> results = new List<T> ();
			results.AddRange (node.quadObjects);
			foreach (QuadNode childNode in node.Nodes) {
				if (childNode != null)
					results.AddRange (GetChildObjects (childNode));
			}
			return results;
		}
		public int GetQuadObjectCount (){
			if (root == null)
				return 0;
			int count = GetQuadObjectCount (root);
			return count;
		}
		private int GetQuadObjectCount (QuadNode node){
			int count = node.Objects.Count;
			foreach (QuadNode childNode in node.Nodes) {
				if (childNode != null) {
					count += GetQuadObjectCount (childNode);
				}
			}
			return count;
		}
		public int GetQuadNodeCount (){
			if (root == null)
				return 0;
			int count = GetQuadNodeCount (root, 1);
			return count;
		}
		private int GetQuadNodeCount (QuadNode node, int count){
			if (node == null)
				return count;
			foreach (QuadNode childNode in node.Nodes) {
				if (childNode != null)
					count++;
			}
			return count;
		}
		public List<QuadNode> GetAllNodes (){
			List<QuadNode> results = new List<QuadNode> ();
			if (root != null) {
				results.Add (root);
				GetChildNodes (root, results);
			}
			return results;
		}
		public void debugDraw (){
			var list = GetAllNodes ();
			foreach (var node in list) {
				var count = GetQuadObjectCount (node);
				node.debugDraw (count);
			}
		}
		private void GetChildNodes (QuadNode node, ICollection<QuadNode> results){
			foreach (QuadNode childNode in node.Nodes) {
				if (childNode != null) {
					results.Add (childNode);
					GetChildNodes (childNode, results);
				}
			}
		}
		public class QuadNode{
			private static int _id = 0;
			public readonly int ID = _id++;
			public int deep { get; set; }
			public QuadNode Parent { get; internal set; }
			private QuadNode[] _nodes = new QuadNode[4];
			public QuadNode this [Direction direction] {
				get {
					switch (direction) {
					case Direction.TL:
						return _nodes [0];
					case Direction.TR:
						return _nodes [1];
					case Direction.BL:
						return _nodes [2];
					case Direction.BR:
						return _nodes [3];
					default:
						return null;
					}
				}
				set {
					switch (direction) {
					case Direction.TL:
						_nodes [0] = value;
						break;
					case Direction.TR:
						_nodes [1] = value;
						break;
					case Direction.BL:
						_nodes [2] = value;
						break;
					case Direction.BR:
						_nodes [3] = value;
						break;
					}
					if (value != null)
						value.Parent = this;
				}
			}
			public ReadOnlyCollection<QuadNode> Nodes;
			internal List<T> quadObjects = new List<T> ();
			public ReadOnlyCollection<T> Objects;
			public Rect theBounds { get; internal set; }
			public bool HasChildNodes (){
				return _nodes [0] != null;
			}
			public QuadNode (Rect bounds,int deep){
				this.deep = deep;
				theBounds = bounds;
				Nodes = new ReadOnlyCollection<QuadNode> (_nodes);
				Objects = new ReadOnlyCollection<T> (quadObjects);
			}
			public QuadNode (float x, float y, float width, float height,int deep)
				: this(new Rect(x, y, width, height),deep){}
			public void debugDraw (int count){
				Color color = new Color (1f, 1f, 1f, 1f);
				if (count > 0) {
					color = new Color (1f, 1f, 0f, 1f);
				}
				if (count > 1) {
					color = new Color (1f, 0f, 0f, 1f);
				}
				if (count > 2) {
					color = new Color (1f, 0f, 1f, 1f);
				}
				var d = count * 0.005f;
				var z = -0.01f;
				var r = theBounds;
				Debug.DrawLine (new Vector3 (r.x + d, r.yMax - d, z * count), new Vector3 (r.xMax - d, r.yMax - d, z * count), color);//top
				Debug.DrawLine (new Vector3 (r.x + d, r.y + d, z * count), new Vector3 (r.x + d, r.yMax - d, z * count), color);//left
				Debug.DrawLine (new Vector3 (r.xMax - d, r.y + d, z * count), new Vector3 (r.xMax - d, r.yMax - d, z * count), color);//right
				Debug.DrawLine (new Vector3 (r.x + d, r.y + d, z * count), new Vector3 (r.xMax - d, r.y + d, z * count), color);//bt
			}
		}
	}
	public enum Direction : int{
		TL = 0,
		TR = 1,
		BL = 2,
		BR = 3
	}
}