using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BatteryTile : Tile 
{

	private float time;
	
	private const float INTERVAL = 3.5f;
	
	private const float CHARGE = 10.0f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		time += Time.deltaTime;
		
		if (time >= INTERVAL)
		{			
			List<Tile> adjacent = GameController.self.GetAdjacentTiles(xCoord + GameController.self.NumTilesX / 2, 
			                                                           yCoord + GameController.self.NumTilesY / 2);
						
			for (int i = 0; i < adjacent.Count; i++)
			{
				adjacent[i].ReceiveCharge(this, new List<Vector2>(), CHARGE);
			}
			
			time -= INTERVAL;
		}
	}
	
	public override void OnAdded(int x, int y)
	{
		toughness = 1000.0f;
		base.OnAdded(x, y);
		SpriteRenderer sprRnd = GetComponent<SpriteRenderer>();
		sprRnd.sprite = (Sprite) Resources.Load("battery", typeof(Sprite));
	}
	
	public override void OnRemoved()
	{
		Data.Wave = GameController.self.GetWave();
		Application.LoadLevel(3);
	}
}
