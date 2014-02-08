using UnityEngine;
using System.Collections;
using LitJson;

public class MyJsonData : MonoBehaviour {
	void Start () {
		//var d = Get("300_A5");
		//var d1 = d[0][0]["a13"];
		//Debug.Log(d1);
	}
	void Update () {
		
	}
	JsonData Get(string fileName){
		TextAsset bindata= Resources.Load(fileName) as TextAsset;
		JsonData jd = JsonMapper.ToObject(bindata.text);
		return jd;
	}
}
