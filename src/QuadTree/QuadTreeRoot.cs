using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpQuadTree;
public class QuadTreeRoot : MonoBehaviour {

	public QuadTree<QuadTreeObject> tree;
	void Awake () {
		tree = new QuadTree<QuadTreeObject>(0f,0f,3.2f,0.65f,4);
	}
	public List<QuadTreeObject> Query(Rect r){
		return tree.Query(r);
	}
	void Update () {
		if(tree!=null)tree.debugDraw();
	}
}
