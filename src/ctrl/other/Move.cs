using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	private Transform t;
	void Start () {
		t = gameObject.transform;
	}
	public void SetSpeed(string n){
		//actionNow = n;
		switch (n) {
		case "run":
			speed_Run_Walk = 0.16f;
			break;
		case "walk":
			speed_Run_Walk = 0.14f;
			break;
		case "jump":
			break;
		default:
			speed_Run_Walk = 0;
			break;
		}
	}
	private float right = 3.15f;
	private float left = 0;
	private float bt = 0;
	private float top = 0.65f;
	void FixedUpdate(){
		var z = (t.position.y-1.1f)*0.1f;
		t.position = new Vector3(t.position.x,t.position.y,z);
		if(t.position.x> right){
			t.position = new Vector3(right,t.position.y,z);
		}
		if(t.position.x<left){
			t.position = new Vector3(left,t.position.y,z);
		}
		if(t.position.y<bt){
			t.position = new Vector3(t.position.x,bt,z);
		}
		if(t.position.y>top){
			t.position = new Vector3(t.position.x,top,z);
		}
	}
	private Vector2 _dirV;//x and y;
	public Vector2 dirV{
		set{
			//_dirV = limit(value);
			_dirV = value;
			if(_dirV.x!=0) dirX = _dirV.x>0? 1:-1;
			dirY = _dirV.y>0? 1:-1;
		}
		get{
			return _dirV;
		}
	}
	public static Vector2 limit(Vector2 v){
		var x = v.x;
		var y = v.y;
		
		var xa = Mathf.Abs(x);
		var xf = x>0?1:-1;
		
		var ya = Mathf.Abs(y);
		var yf = y>0?1:-1;
		
		x = (0.5f+xa*0.5f/1.5f)*xf;
		y = (0.5f+ya*0.5f/1.5f)*yf;
		return new Vector2(x,y);
	}
	public int dirX=1;//1 or -1
	public int dirY=1;//1 or -1
	public float speed = 1f;
	private float _speed_Run_Walk = 0f;
	//private string actionNow;
	public string obName;
	public float speed_Run_Walk{
		get{return _speed_Run_Walk;}
		set{
			_speed_Run_Walk = value;
		}
	}
	public float speedX = 1f;
	void doMove(){
		var pos = t.position;
		if(_dirV.x==0 &&_dirV.y!=0){
			t.position = 	new Vector3(pos.x												,pos.y+speed_Run_Walk*speedX*speed*0.1f*(dirV.y+0.5f*dirY)	,pos.z);
			return;
		}
		if(_dirV.y==0 &&_dirV.x!=0){
			t.position = 	new Vector3(pos.x+speed_Run_Walk*speedX*speed*0.1f*(dirV.x+0.5f*dirX)	,pos.y													,pos.z);
			return;
		}
		t.position = 		new Vector3(pos.x+speed_Run_Walk*speedX*speed*0.1f*(dirV.x+0.5f*dirX),pos.y+speed_Run_Walk*speedX*speed*0.1f*(dirV.y+0.5f*dirY)	,pos.z);
	}
	void Update () {
		doMove();
		doForce();
	}
	
	//以下是 force 部分
	public float force = 0;
	private float force_setp_down = 0.1f;
	void doForce(){
		if(force==0) return;
		var p = t.position;
		if(Mathf.Abs(force)-0>0.1){
			var f = force>0? 1:-1;
			force = f*(Mathf.Abs(force)-force_setp_down);
			if(Mathf.Abs(force)-0.1<0){
				force = 0;
			}else{
				t.position = new Vector3(p.x+force*dirX*0.1f,p.y,p.z);
			}
		}
	}
}
