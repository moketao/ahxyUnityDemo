using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;
using CSharpQuadTree;
public class BaseCtrl: MonoBehaviour { 
	public string actionNow = "";
	public string pre = "";//P1158_ or P300_
	public bool isBoss = false;
	public bool isPlayer = true;
	public static bool useSocket = true;
	public tk2dSpriteAnimator ani;
	public static BaseCtrl instance;
	
	private float lastHitTime;
	private int hitCount;
	private Dictionary<GameObject,bool> hitObjects = new Dictionary<GameObject,bool>();
	private Gravity g;
	private tk2dSprite sprite;
	private Move move;
	private QuadTreeObject tree;
	private BoxCollider box;
	void Start(){
		box = gameObject.GetComponent<BoxCollider>();
		g = gameObject.GetComponent<Gravity>();
		g.OnDownToTheFloor += downToTheFloor;
		move = gameObject.transform.parent.GetComponent<Move>();
		move.obName = pre;
		tree = gameObject.transform.parent.GetComponent<QuadTreeObject>();
		
		sprite = gameObject.GetComponent<tk2dSprite>();
		ani = gameObject.GetComponent<tk2dSpriteAnimator>();
		
		ani.AnimationCompleted = AnimationCompleteDelegate;
		ani.AnimationEventTriggered = AnimationEventDelegate;
		
		if(isPlayer)instance = this;
	}
	void OnEnable(){
		if (g!=null){
			g.OnDownToTheFloor += downToTheFloor;
		}
	}
	public bool canMoveOrIdle(){
		return (actionNow=="" || actionNow=="idle" || actionNow=="walk" || actionNow=="run");
	}
	void OnDisable(){
		if (g!=null){
			g.OnDownToTheFloor -= downToTheFloor;
		}
	}
	void OnTriggerEnter(Collider c){
		hitObjects.Add(c.gameObject,true);
	}
	void OnTriggerExit(Collider c){
		hitObjects.Remove(c.gameObject);
	}
	public void Play(string n){
		if(ani==null)return;
		var arr = n.Split(new string[] { "_" },System.StringSplitOptions.None);
		move.SetSpeed(arr[0]);//run or walk
		var c = ani.CurrentClip.name;
		if(c!=pre+n){
			Details(n,c,arr);
		}
	}
	void Details(string n,string c,string[] arr){
		if(n=="jump"){
			if(!canMoveOrIdle())return;
			g.speedH = move.speed_Run_Walk*move.dirX*move.speed*0.01f;
			g.speed = 0.1f;
		}
		if(arr[0]=="att"){
			if(c==pre+"hit" || c==pre+"hitfly" || c==pre+"dead")return;
		}
		if(arr[0]=="hit"){
			if(c==pre+"att_1")return;
			var d = Time.time-lastHitTime;
			if(d<1){
				hitCount++;
			}else{
				hitCount=1;
			}
			lastHitTime = Time.time;
			if(hitCount>2){
				n = arr[0] = "hitfly";
				g.speedH = -move.dirX*0.01f;
				g.speed = 0.1f;
				g.isHitFly = true;
				hitCount = 0;
			}
		}
		if(arr[0]=="hitfly"){
			g.speedH = -move.dirX*0.01f;
			g.speed = 0.1f;
			g.isHitFly = true;
			hitCount = 0;
		}
		if(arr[0]=="dead"){
			sprite.FlipX = move.dirX==1?false:true;
			relive(2f);
		}
		if(arr[0]=="relive"){
			sprite.FlipX = move.dirX==1?false:true;
		}
		actionNow = arr[0];
		ani.Play(pre+n);
	}
	void AnimationCompleteDelegate(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip){
		actionNow = "";
	}
	void AnimationEventDelegate(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameNo){
		tk2dSpriteAnimationFrame frame = clip.GetFrame( frameNo );
		if(frame.eventInfo=="hit"){
			if(useSocket){
				CheckHitUseTree();
			}else{
				CheckHitUseCollider();
			}
		}
		if(frame.eventInfo=="hitfly"){
			CheckHitUseCollider(true);
		}
		if(frame.eventInfo=="force"){
			move.force = frame.eventFloat;
		}
		if(frame.eventInfo=="clearhit"){
			hitCount=0;//todo: debug
		}		
	}
	void downToTheFloor(bool isHitFly){
		if(isHitFly){
			Play("dead");//hitfly
		}else{
			actionNow = "";
		}
	}
	public void relive(float sec){
		Invoke("relive_real",sec);
	}
	void relive_real(){
		if(actionNow=="dead")Play("relive");
	}
	public void playHit(GameObject killer){
		if(actionNow=="att" && isBoss)return;
		//todo:		blood down;
		var killer_move = killer.transform.parent.GetComponent<Move>();
		move.dirV = new Vector2(1f,1f)*-killer_move.dirX;
		sprite.FlipX = move.dirX==1?false:true;
		move.force = -0.5f;
		Play("hit");
	}
	public void playHitFly(GameObject killer){
		if(actionNow=="att" && isBoss)return;
		//todo:		blood down;
		var killer_move = killer.transform.parent.GetComponent<Move>();
		move.dirV = new Vector2(1f,1f)*-killer_move.dirX;
		sprite.FlipX = move.dirX==1?false:true;
		Play("hitfly");
	}
	void CheckHitUseCollider(bool isHitFly = false){
		foreach (KeyValuePair<GameObject, bool> i in hitObjects) {
			var ctrl = i.Key.GetComponent<BaseCtrl>();
			if(isHitFly){
				ctrl.playHitFly(gameObject);
			}else{
				ctrl.playHit(gameObject);
			}
		}
	}
	void CheckHitUseTree(bool isHitFly = false){
		var b = box.bounds;
		var p = gameObject.transform.position;
		var r = new Rect(b.center.x-b.extents.x,p.y-0.07f,b.size.x,0.14f);
		List<QuadTreeObject> list = tree.Query(r);
		foreach (QuadTreeObject i in list) {
			if(i==this)continue;
			var ctrl = i.transform.Find("body").gameObject.GetComponent<BaseCtrl>();
			if(isHitFly){
				ctrl.playHitFly(gameObject);
			}else{
				ctrl.playHit(gameObject);
			}			
		}
	}
}
