using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConductTile : Tile 
{
	private Tile Sender;

	private Sprite idleSprite;
	
	private Sprite activeSprite;
	
	private float activatedTimer;
	
	private float charge;
	
	List<Vector2> Visited;
	
	public const float ACTIVATION_TIME = 0.1f;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{	
		if (activatedTimer > 0)
			activatedTimer -= Time.deltaTime;
		else if (charge > 0)
		{
			ConductTile[] adjacent = GetAdjacentTiles(Sender);
			if (adjacent.Length > 0 && 
				!Visited.Contains(new Vector2(xCoord + GameController.self.NumTilesX / 2, 
											  yCoord + GameController.self.NumTilesY / 2)))
			{
				Visited.Add(new Vector2(xCoord + GameController.self.NumTilesX / 2, 
				                        yCoord + GameController.self.NumTilesY / 2));
				for (int i = 0; i < adjacent.Length; i++)
				{
					adjacent[i].ReceiveCharge(this, Visited, charge / (adjacent.Length + 1));
				}
			}
			else
			{
				GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
				
				for (int i = 0; i < monsters.Length; i++)
				{
					if ((monsters[i].transform.position - transform.position).sqrMagnitude < 1.0f) // radius = 2
					{
						monsters[i].GetComponent<Monster>().SetDead();
					}
				}
			}

			charge = 0;
			SpriteRenderer sprRnd = GetComponent<SpriteRenderer>();
			sprRnd.sprite = idleSprite;
			Sender = null;
			Visited = null;
		}
	}
	
	private ConductTile[] GetAdjacentTiles(Tile Sender)
	{
		List<Tile> tiles = GameController.self.GetAdjacentTiles(xCoord + GameController.self.NumTilesX / 2, 
		                                                        yCoord + GameController.self.NumTilesY / 2);
		
		List<ConductTile> newTiles = new List<ConductTile>();
		foreach (Tile tile in tiles)
		{
			if (tile.GetType() == typeof(ConductTile) && !tile.Equals(Sender))
				newTiles.Add((ConductTile) tile);
		}
		
		return newTiles.ToArray();
	}
	
	public override void ReceiveCharge(Tile sender, List<Vector2> visited, float charge)
	{
		this.charge = charge;
		Sender = sender;
		Visited = visited;
		SpriteRenderer sprRnd = GetComponent<SpriteRenderer>();
		sprRnd.sprite = activeSprite;
		activatedTimer = ACTIVATION_TIME;
	}
	
	public override void OnAdded(int x, int y)
	{
		toughness = 100.0f;
		base.OnAdded(x, y);
		
		idleSprite = (Sprite) Resources.Load("tile", typeof(Sprite));
		activeSprite = (Sprite) Resources.Load("tile_active", typeof(Sprite));
		
		SpriteRenderer sprRnd = GetComponent<SpriteRenderer>();
		sprRnd.sprite = idleSprite;
	}
	
	public override void OnRemoved()
	{
	}
}
