using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CSharpQuadTree;

public class QuadTreeObject : MonoBehaviour,IQuadObject {
	private tk2dSprite s;
	private QuadTreeRoot treeRoot;
	void Start () {
		s = gameObject.GetComponent<tk2dSprite>();
		if(s==null)s=gameObject.transform.Find("body").GetComponent<tk2dSprite>();
		treeRoot = GameObject.Find("tree").GetComponent<QuadTreeRoot>();
	}
    public Rect theBounds{
        get {
			var p = gameObject.transform.position;
			return new Rect(p.x,p.y,0.01f,0.01f); 
		}
    }
    private void hasChanged(){
        EventHandler handler = Changed;
        if (handler != null)
            handler(this, new EventArgs());
    }
	public event EventHandler Changed;
	private bool _inTree;
	public bool inTree{
		get{
			return _inTree;
		}
		set{
			_inTree = value;
		}
	}
	private Vector3 last_pos;
	public Rect QueryR;
	public List<QuadTreeObject> Query(Rect r){
		QueryR = r;
		return treeRoot.Query(r);
	}
	void OnGUI(){
		if(QueryR.x!=0){
			GUILayout.Label(QueryR.ToString());
			GUILayout.Label(Query(QueryR).Count.ToString());
		}
	}
	void Update () {
		if(!inTree){
			if(treeRoot.tree!=null){
				treeRoot.tree.Insert(this);
				return;
			}else{return;}
		}
		
		if(gameObject.transform.position!=last_pos){
			last_pos = gameObject.transform.position;
			hasChanged();
		}
		
		var o = gameObject.transform.position;
		var b = s.GetBounds();
		var b1 = new Vector3(o.x+b.min.x,o.y-b.min.y,o.z-0.01f);
		var b2 = new Vector3(o.x-b.min.x,o.y+b.min.y,o.z-0.01f);
		Debug.DrawLine(b1,b2);
			b1 = new Vector3(o.x-0.05f,o.y-0.05f,o.z-0.05f);
			b2 = new Vector3(o.x+0.05f,o.y+0.05f,o.z-0.05f);		
		Debug.DrawLine(b1,b2);
		
		if(QueryR.x!=0){
			b1 = new Vector3(QueryR.xMax,QueryR.yMax,0);
			b2 = new Vector3(QueryR.xMin,QueryR.yMin,0);
			Debug.DrawLine(b1,b2,new Color(1f,0.5f,0.5f,1f));
		}
	}
}
