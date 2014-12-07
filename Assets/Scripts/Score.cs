using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour 
{
	
	// Use this for initialization
	void Start () 
	{
		GetComponent<GUIText>().text = "Your got to wave: " + Data.Wave;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
