using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	
	
	public GameObject[]ModelHolder=new GameObject [3];
	public GameObject finishTarget;

	public enum PlayerNum 
	{
		PLAYER1,
		PLAYER2,
		PLAYER3,
		PLAYER4
	};
	
	public PlayerNum num;
	private GameObject model;
	// Use this for initialization
	void Start () {
		GameObject temp;
		switch(num)
		{
			case (PlayerNum.PLAYER1):
				model = ModelHolder[GameObject.FindGameObjectWithTag("manager").GetComponent<GameManager>().player1];
				temp = (GameObject) Instantiate(model,transform.position, transform.rotation);
				temp.GetComponent<PlayerController>().num = PlayerController.PlayerNum.Player1;
				temp.tag = "Player";
				GameObject.Find("p1Camera").GetComponent<Follow>().target = temp.GetComponent<PlayerController>().follower;
				GameObject.Find ("p1Camera").GetComponent<Follow>().facerientation = temp.GetComponent<PlayerController>().follower;
				GameObject.Find ("p1Camera").GetComponent<Follow>().player = temp;
				GameObject.FindGameObjectWithTag("manager").GetComponent<GameManager>().player1GO = temp;
//				GameObject.FindGameObjectWithTag("manager").GetComponent<GameManager>().cars.Add(temp);
				temp.GetComponent<PlayerController>().target = finishTarget.transform;
				break;
			case (PlayerNum.PLAYER2):
				model = ModelHolder[GameObject.FindGameObjectWithTag("manager").GetComponent<GameManager>().player2];
				temp = (GameObject) Instantiate(model,transform.position, transform.rotation);
				temp.GetComponent<PlayerController>().num = PlayerController.PlayerNum.Player2;
				temp.tag = "Player";
				GameObject.Find("p2Camera").GetComponent<Follow>().target = temp;
				GameObject.Find ("p2Camera").GetComponent<Follow>().facerientation = temp;
				GameObject.FindGameObjectWithTag("manager").GetComponent<GameManager>().player2GO = temp;
//				GameObject.FindGameObjectWithTag("manager").GetComponent<GameManager>().cars.Add(temp);
			temp.GetComponent<PlayerController>().target = finishTarget.transform;
			break;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
