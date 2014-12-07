using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
	protected int xCoord;
	protected int yCoord;
	
	protected float destroyMeter;

	public float toughness = 0.0f;
	
	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	
	}
	
	public virtual void ReceiveCharge(Tile sender, List<Vector2> visited, float charge)
	{
	}
	
	public virtual void OnAdded(int x, int y)
	{
		xCoord = x - GameController.self.NumTilesX / 2;
		yCoord = y - GameController.self.NumTilesY / 2;

		Vector2 tileSize = GameController.self.GetTileSize();
		Vector2 tileScale = GameController.self.GetTileScale();
		transform.position = new Vector3(xCoord * tileSize.x + (tileSize.x / 2.0f), yCoord * tileSize.y + (tileSize.y / 2.0f), 0.0f);
		transform.localScale = new Vector3(tileScale.x, tileScale.y, 1.0f);
		
		destroyMeter = toughness;
	}
	
	public void DestroyByMonster(float amount)
	{
		destroyMeter -= amount;
		
		if (destroyMeter <= 0)
			Destroy(gameObject, Time.deltaTime);
		
		float progress = (toughness - destroyMeter) / toughness;
		SpriteRenderer spr_renderer = GetComponent<SpriteRenderer>();
		Color c = spr_renderer.color;
		Color newColor = new Color(c.r + (progress / (1 - c.r)),
		                  		   c.g + (progress / (1 - c.g)),
		                 		   c.b + (progress / (1 - c.b)),
		                  		   c.a);
		spr_renderer.color = newColor;
	}
	
	public virtual void OnRemoved()
	{
	}
}
