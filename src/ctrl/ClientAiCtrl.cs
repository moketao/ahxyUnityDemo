using UnityEngine;
using System.Collections;

public class ClientAiCtrl : MonoBehaviour {
	private Move move;
	private BaseCtrl ctrl;
	public GameObject tg;
	private tk2dSprite sprite;
	void Start () {
		move = gameObject.GetComponent<Move>();
		move.speed = 0.5f;
		sprite = gameObject.transform.Find("body").GetComponent<tk2dSprite>();
		ctrl = gameObject.transform.Find("body").GetComponent<BaseCtrl>();
	}
	private float lastAttTime;
	void Update () {
		if(!ctrl.canMoveOrIdle()) return;
		if(canAttackPlayer()){
			var d = Time.time-lastAttTime;
			if(d>6){
				ctrl.Play("att_1");
				lastAttTime = Time.time;
				return;
			}
		}
		if(isCloseToPlayer()){
			idle();
		}else{
			walk();
		}
	}
	public bool canAttackPlayer(){
		if(tg==null) return false;
		var y2 = gameObject.transform.position.y;
		var y1 = tg.transform.position.y;
		if(Mathf.Abs(y2-y1)<0.1){
			return true;
		}
		return false;
	}
	public bool isCloseToPlayer(){
		var p2 = gameObject.transform.position;
		var p1 = tg.transform.position;
		return Vector3.Distance(p2,p1)<0.1;
	}
	public void walk(){
		var p2 = gameObject.transform.position;
		var p1 = tg.transform.position;
		var p = p2 - p1;
		float s = 0.1f;
		var v = new Vector2(-p.x*s,-p.y*s);
		bool inCloseX = false;
		if(Mathf.Abs(p2.x-p1.x)<0.1){
			v.x = 0;
			inCloseX = true;
		}
		if(Mathf.Abs(p2.y-p1.y)<0.1){
			v.y = 0;
		}
		move.dirV = v*5;
		
		if(!inCloseX)sprite.FlipX = move.dirX==1? false:true;
		if(ctrl.actionNow!="walk"){
			ctrl.Play("walk");
		}
	}
	public void idle(){
		if(ctrl.actionNow!="idle"){
			ctrl.Play("idle");
			move.dirV = new Vector2();
		}
	}
}
