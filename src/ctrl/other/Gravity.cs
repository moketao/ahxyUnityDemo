using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {
	public float speed = 0;
	public float speedH = 0;
	public GameObject floot;
	public bool isHitFly = false;
	public event System.Action<bool> OnDownToTheFloor;
	void Update () {
		if(speed==0) return;
		var p = gameObject.transform.position;
		speed = speed-9.8f*0.0006f;
		var yy = p.y+speed;
		var theFloor = floot.transform.position.y;
		if(yy<theFloor){
			yy=theFloor;
			speed = 0;
			if (OnDownToTheFloor != null) { OnDownToTheFloor(isHitFly); }
			isHitFly = false;
		}
		gameObject.transform.position = new Vector3(p.x,yy,p.z);
		var pt = gameObject.transform.parent.position;
		gameObject.transform.parent.position = new Vector3(pt.x+speedH,pt.y,pt.z);
	}
	void Start () {
		
	}
}
