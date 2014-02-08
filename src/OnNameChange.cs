using UnityEngine;
using System.Collections;

public class OnNameChange : MonoBehaviour {
	private tk2dTextMesh me;
	private tk2dTextMesh mesh;
	void Start(){
		me = gameObject.GetComponent<tk2dTextMesh>();
	}
	void OnTheNameChange(){
		mesh = GameObject.Find("PlayerInputName").GetComponent<tk2dTextMesh>();
		Invoke("change",0.5f);// To avoid the "double" text bug , we must make a later call;
	}
	void change(){
		me.text = mesh.text;
	}
	void Update(){
		
	}
}
