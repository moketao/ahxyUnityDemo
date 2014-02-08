using UnityEngine;
using System.Collections;

public class onbt1 : MonoBehaviour {
	public tk2dUIItem uiItem;
	void Start(){
		var s = gameObject.transform.Find("g1").gameObject.GetComponent<tk2dSprite>();
		s.color = new Color(1,1,1,0.5f);
	}
    void OnEnable(){
		if (uiItem){
			uiItem.OnDown += Go;
		}
    }
    void OnDisable(){
		if (uiItem){
			uiItem.OnDown -= Go;
		}
    }
	void Go(){
		if(BaseCtrl.instance!=null){
			BaseCtrl.instance.Play("att_1");
		}
	}
}
