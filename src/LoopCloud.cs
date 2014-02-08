using UnityEngine;
using System.Collections;

public class LoopCloud : MonoBehaviour {
	private Vector3 startPos1;
	private Vector3 startPos2;
	public static GameObject o1;
	public static GameObject o2;
	public static float moveStep = 0.002f;
	public static GameObject first;
	void Start () {
		if(first==null){
			o1 = gameObject;
			first = o1;
			var s1 = o1.transform.FindChild("cloud1").gameObject.GetComponent<tk2dSprite>();
			var width = s1.GetBounds().size.x;
			fixPos(o1,width);
			
			o2 = Instantiate(o1) as GameObject;
			var p2 = o2.transform.position;
			o2.transform.position = new Vector3(p2.x - width*2,p2.y,p2.z);
			
			startPos1 = o1.transform.position;
			startPos2 = o2.transform.position;
		}
	}
	void fixPos(GameObject o1,float width){
		var cloud1 = o1.transform.FindChild("cloud1").gameObject;
		var cloud1pos = cloud1.transform.position;
		var cloud2 = o1.transform.FindChild("cloud2").gameObject;
		cloud2.transform.position = new Vector3(cloud1pos.x+width,cloud1pos.y,cloud1pos.z);
	}
	void Update () {
		if(gameObject == first){
		
			var p = o1.transform.position;
			o1.transform.position = new Vector3(p.x+moveStep,p.y,p.z);
			
			p = o2.transform.position;
			o2.transform.position = new Vector3(p.x+moveStep,p.y,p.z);
			
			if(Mathf.Abs(p.x-startPos1.x)<0.01){
				o1.transform.position = startPos1;
				o2.transform.position = startPos2;
			}
		}
	}
}
