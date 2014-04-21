using UnityEngine;
using System.Collections;

public class StartController : MonoBehaviour {

	public enum PlayerNum
	{
		Player1 = 1,
		Player2 = 2,
		Player3 = 3,
		Player4 = 4
	};

	public PlayerNum num;
	private string joyX;
	private string joyY;
	private string aButton;
	private string start;
	private string select;

	private int index = 0;
	private GameObject GM;
	private float time;

	public bool isActive = false;
	public bool ready = true;
	private bool done = false;
	// Use this for initialization
	void Awake()
	{
		joyX = "Horizontal";
		joyY = "Vertical";
		aButton = "A";
		start = "start";
		select = "select";
	}
	void Start () 
	{
		GM = GameObject.FindGameObjectWithTag ("manager");
		if(num == PlayerNum.Player1)
		{
			isActive = true;
			ready = false;
			return;
		}
		joyX += ((int)num).ToString ();
		joyY += ((int)num).ToString ();
		aButton += ((int)num).ToString ();
		start += ((int)num).ToString ();
		select += ((int)num).ToString ();
//		Debug.Log (joyX);
	}
	
	// Update is called once per frame
	void Update () {
//		if(ready && num == PlayerNum.Player1)
//			Debug.Break();
		if(isActive)
		{
			if(Input.GetAxis(joyY)>0 && time <=0.0f)
			{
				time = .25f;
//				if(num == PlayerNum.Player2) Debug.Break();
				GM.GetComponent<GameManager>().horizontalChange(-1);
			}
			if(Input.GetAxis(joyY)<0 && time <=0.0f)
			{
				time = .25f;
				GM.GetComponent<GameManager>().horizontalChange(1);
			}
			if(Input.GetAxis(joyX)>0 && time<=0.0f)
			{
				time = .25f;
				GM.GetComponent<GameManager>().changeIndex(num,1);
			}
			if(Input.GetAxis(joyX)<0 && time<=0.0f)
			{
				time = .25f;
				GM.GetComponent<GameManager>().changeIndex(num,-1);
			}
			if (Input.GetButtonDown (aButton) && time<=0.0f)
			{
				time = .25f;
				if(GM.GetComponent<GameManager>().onCharacter && done)
					this.ready = true;
				GM.GetComponent<GameManager>().handleA(num);
			}
			time -= Time.deltaTime;
		}
		else if( Input.GetButtonDown(select))
		{
			isActive = true;
			ready = false;
			GM.GetComponent<GameManager>().setActive(num);
		}

	}

	public void OnTweenComplete()
	{
		done = true;
	}
}
