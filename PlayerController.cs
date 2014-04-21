using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerController : MonoBehaviour {

	public class PlanetSorter : IComparer <GameObject>
	{
		public GameObject thisRacer;

		public int Compare(GameObject one, GameObject two)
		{
			return Vector3.Distance (thisRacer.transform.position, one.transform.position).CompareTo (
				Vector3.Distance (thisRacer.transform.position, two.transform.position));

		}
	}


	public enum PlayerNum
	{
		Player1 = 1,
		Player2 = 2,
		Player3 = 3,
		Player4 = 4
	};
	private enum TurnStates
	{
		TURNRIGHT,
		TURNLEFT,
		BANKRIGHT,
		BANKLEFT,
		NORMAL
	};
	public enum MoveStates
	{
		NORMAL,
		SLINGSHOT,
		BOOSTED,
		TRAPPED,
		DEAD,
		FINISH
	}

	public PlayerNum num;
	public float distance;
	public bool finishedTheRace = false;
	public string place = "1st";
	public GameObject follower;
	public float slingShotDistance = 10.0f;
	public float dangerDistance = 5.0f;
	public GameObject rendererChild;
	//slingshot
	private float thetaStart;
	private float theta;
	private Vector3 to;
	private float r;
	public float blinkDuration = 100f;
	private float blink;
	private int orrientation = 1;

	/// <summary>
	///Control Strings;
	/// </summary>
	private string accelString = "accel";
	private string joyXString = "Horizontal";
	private string joyYString = "Vertical";
	private string aButton = "A";
	private string startString = "start";
	private string BR = "BR";
	private string BL = "BL";
	public List<GameObject> planets;

	private TurnStates turn = TurnStates.NORMAL;
	public MoveStates move = MoveStates.NORMAL;
	private Vector3 start;
	public float respawn;
	private float respawnMod;
	private bool off;
	private float time;
	public Transform planet = null;
	private Vector3 beforeRotation;
	private bool aPressed;
	private float rotated;
	// Use this for initialization
	public float rotate;
	public float sensitive;
	public float senseTurn;
	public float speed;
	private float speedHolder;
	public float topSpeed;
	private float accelerationHolder;
	float joyX;
	float joyY;
	float speedBefore;
	public float bankTurn;
	public Transform target;
	PlanetSorter sorter;

	void Start () {
		sorter = new PlanetSorter ();
		blinkDuration = blink;
		sorter.thisRacer = this.gameObject;
		planets = GameObject.FindGameObjectsWithTag ("Planet").ToList();
		start = transform.position;
		respawnMod = respawn;
		rotated = rotate;
		if(num == PlayerNum.Player1)
			return;		//Player one! I dont want to mod the input variables;
		accelString += ((int)num).ToString ();
		joyXString += ((int)num).ToString ();
		joyYString += ((int)num).ToString ();
		aButton += ((int)num).ToString ();
		startString += ((int)num).ToString ();
		BR += ((int)num).ToString ();
		BL += ((int)num).ToString ();
//		StartCoroutine ("rayCastAndSort");
	}
	
	// Update is called once per frame
	void Update () {
		planets.Sort (sorter);
		planet = planets [0].transform;
		joyX = Input.GetAxisRaw(joyXString) ;
		joyY = Input.GetAxisRaw(joyYString);
		switch(move)
		{
			case(MoveStates.NORMAL):
				movement(true);
				break;
			case(MoveStates.SLINGSHOT):
//				Debug.Log(to);
				speed += Mathf.Sqrt((6.67f * Mathf.Pow (10, -11) * planet.gameObject.rigidbody.mass*100000000000f) / r);
				transform.position = new Vector3 ((r) * Mathf.Cos (theta), 0 , (r)*Mathf.Sin (orrientation*theta))+planet.position;
				theta += speed/r*Time.deltaTime;
				to = -orrientation*(planet.position - transform.position);
				transform.right = to;
				if (theta > (2*Mathf.PI+ thetaStart)+.1f)
				{
					move = MoveStates.BOOSTED;
					speedBefore = speed;
					planet = null;
				}
				break;
			case(MoveStates.BOOSTED):
				if(speed>topSpeed)
				{	
					
					speed -= 1f;
				}
				else
				{
					move = MoveStates.NORMAL;
					theta = 0;
	
				}
				movement(false);
			break;
		case(MoveStates.FINISH):
			transform.LookAt(target);
			if(Vector3.Distance(transform.position,target.position)>=0.1f)
				transform.position += 50*transform.forward;
			break;
		case(MoveStates.DEAD):
			rendererChild.GetComponent<MeshRenderer>().enabled = false;
			transform.position = transform.forward*500;
			speed = 0;
			if(respawn<=0f)
			{
				respawn = respawnMod;
				rendererChild.GetComponent<MeshRenderer>().enabled = true;
				blink = blinkDuration;
				blinkDuration = 0;
				for( float i = 4f; i>0f; i -=Time.deltaTime)
					respawnBlink();
				rendererChild.GetComponent<MeshRenderer>().enabled = true;
				move = MoveStates.NORMAL;
			}
			respawn -= Time.deltaTime;
			break;
		case(MoveStates.TRAPPED):
			DestroyShip();
			break;
		}

		}
	
//	void OnCollisionEnter(Collision collision)
//	{
//		if(collision.gameObject.tag == "Planet")
//		{
//			transform.position -= 3*Vector3.right;
//		}
//		else if(collision.gameObject.tag == "FinishLine")
//		{
//			move = MoveStates.FINISH;
//		}
//
//	}

	void respawnBlink()
	{
		if(blinkDuration<=0f)
		{
			blinkDuration = blink;
			rendererChild.GetComponent<MeshRenderer>().enabled = !rendererChild.GetComponent<MeshRenderer>().enabled;
		}
		blinkDuration -= Time.deltaTime;
	}

	private void movement(bool notBoosted)
	{
		if(notBoosted)
		{
			float accel = Input.GetAxis(accelString);
			if( accel > 0)
			{
				if(speed <= topSpeed)
				{
					speed += .05f;
				}
			}
			else
			{
				if(speed >=0)
				{
					speed -= .05f;
				}
				if( speed < 0f)
					speed = 0;
				
			}
			Vector3 beforePos = transform.position;
			transform.position += transform.forward * Mathf.Abs(accel) * sensitive * speed * Time.deltaTime;
			rigidbody.velocity = (transform.position - beforePos)/Time.deltaTime;
		}
		else transform.position += transform.forward * sensitive * speed * Time.deltaTime;
		switch(turn)
		{
		case(TurnStates.BANKRIGHT):
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,-85);
			transform.Rotate (Vector3.up * joyX * bankTurn * Time.deltaTime);
			if(joyX <= 0.005)
				turn = TurnStates.NORMAL;
			break;
		case(TurnStates.BANKLEFT):
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,85);
			transform.Rotate (Vector3.up * joyX * bankTurn * Time.deltaTime);
			if(joyX >= -0.005)
				turn = TurnStates.NORMAL;
			break;
		case(TurnStates.TURNLEFT):
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,30);
			transform.Rotate (Vector3.up * joyX * senseTurn * Time.deltaTime);
			if(joyX >= -0.005)
				turn = TurnStates.NORMAL;
			break;
		case(TurnStates.TURNRIGHT):
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,-30);
			transform.Rotate (Vector3.up * joyX * senseTurn * Time.deltaTime);
			if(joyX <= 0.005)
			{
				turn = TurnStates.NORMAL;
			}
			break;
		default:
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0);
			break;
		}
		if(joyX >= 0.005)
			turn = (Input.GetButton (BR)) ? TurnStates.BANKRIGHT : TurnStates.TURNRIGHT;
		else if (joyX <= -0.005)
			turn = (Input.GetButton (BL)) ? TurnStates.BANKLEFT : TurnStates.TURNLEFT;
		//transform.Rotate(Vector3.right * joyY * sensitive * 10f * Time.deltaTime);
		transform.eulerAngles = new Vector3(25*joyY*time,transform.eulerAngles.y, transform.eulerAngles.z);
		if(time <1f)
			time += 2f*Time.deltaTime;
		else if(time >1f)
			time = 1f;
		if (joyY <= 0.005f && joyY >= -0.005f)
		{
			time = 0;
			float velocity = 1f;
			transform.eulerAngles = new Vector3(Mathf.SmoothDamp(transform.eulerAngles.x,0f,ref velocity,1000000000f),transform.eulerAngles.y, transform.eulerAngles.z);
		}
		if(notBoosted)
		{
			float distance = Vector3.Distance(transform.position,planet.position)-planet.GetComponent<Orbit>().radius;
			if(distance<=slingShotDistance && distance>=dangerDistance)
			{
//				Debug.Break();
				if(Input.GetButtonDown(aButton))
				{
					move = MoveStates.SLINGSHOT;
					to = (planet.position - transform.position);
					float sidevar = to.z<0 ? 0 :2*Mathf.PI;
					float val = to.z>0 ? -90:90f+180;
					orrientation = to.z>0 ? -1:1;
					//val = val == 0 && to.x>0 ? 180 :0;
					theta = (-(Vector3.Angle (to, Vector3.right) +val) * Mathf.PI/180f) + Mathf.PI/2 ;//- sidevar;
					thetaStart = theta;
					r = Vector3.Distance (transform.position, planet.position);
				}
			}
			else if(distance <= dangerDistance)
			{
				to = (planet.position - transform.position);
				float sidevar = to.z<0 ? 0 :2*Mathf.PI;
				float val = to.z>0 ? -90:90f+180;
				orrientation = to.z>0 ? -1:1;
				//val = val == 0 && to.x>0 ? 180 :0;
				theta = (-(Vector3.Angle (to, Vector3.right) +val) * Mathf.PI/180f) + Mathf.PI/2 ;//- sidevar;
				thetaStart = theta;
				r = Vector3.Distance (transform.position, planet.position);
				move = MoveStates.TRAPPED;
			}
		}
	}

	void DestroyShip()
	{
		speed += Mathf.Sqrt((6.67f * Mathf.Pow (10, -11) * planet.gameObject.rigidbody.mass*100000000000f) / r);
		transform.position = new Vector3 ((r) * Mathf.Cos (theta), 0 , (r)*Mathf.Sin (orrientation*theta))+planet.position;
		theta += speed/r*Time.deltaTime;
		to = -orrientation*(planet.position - transform.position);
		transform.right = to;
		r -= 10f;
	}
	
	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "Planet")
		{
			move = MoveStates.DEAD;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "finishLine")
		{
			move = MoveStates.FINISH;
			finishedTheRace = true;
		}
	}
	
	
}









