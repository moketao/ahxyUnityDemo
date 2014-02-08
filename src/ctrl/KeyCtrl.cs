using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;
public class KeyCtrl: MonoBehaviour { 
	private Move move;
	private BaseCtrl ctrl;
	public GameObject tg;
	private tk2dSprite sprite;
	public Dictionary<GameObject,bool> hitObjects = new Dictionary<GameObject,bool>();
	void Start(){
		move = gameObject.GetComponent<Move>();
		move.speed = 0.6f;
		sprite = gameObject.transform.Find("body").GetComponent<tk2dSprite>();
		ctrl = gameObject.transform.Find("body").GetComponent<BaseCtrl>();
	}
	void OnEnable(){
		EasyJoystick.On_JoystickMove += On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd += On_JoystickMoveEnd;
	}
	void OnDisable(){
		EasyJoystick.On_JoystickMove -= On_JoystickMove	;
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
	}
	void OnDestroy(){
		EasyJoystick.On_JoystickMove -= On_JoystickMove;	
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
	}
	void On_JoystickMoveEnd(MovingJoystick e){
		if (e.joystickName == "MyJoystick"){
			if(ctrl.canMoveOrIdle())ctrl.Play("idle");
		}
	}
	void On_JoystickMove( MovingJoystick e){
		if(ctrl.actionNow=="hit")return;
		if(ctrl.actionNow=="hitfly")return;
		move.dirV = e.joystickAxis;
		sprite.FlipX = move.dirX==1? false:true;
		if(Vector2.SqrMagnitude(e.joystickAxis)>0.5f){
			if(ctrl.canMoveOrIdle())ctrl.Play("run");
		}else{
			if(ctrl.canMoveOrIdle())ctrl.Play("walk");
		}
	}
	void Update(){
		toIdle();
	}
	void toIdle(){
		if(ctrl.canMoveOrIdle()){
			if(move.speed_Run_Walk<=0){
				ctrl.Play("idle");
			}else{
				if(Vector2.SqrMagnitude(move.dirV)>0.5f){
					if(ctrl.canMoveOrIdle())ctrl.Play("run");
				}else{
					if(ctrl.canMoveOrIdle())ctrl.Play("walk");
				}
			}
		}
	}
}
