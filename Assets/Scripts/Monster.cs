using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour 
{	
	public float destroyAmount;

	public float walkingSpeed;
	
	public Vector3 walkingDirection;

	private GUIText EnemyCounter;
	
	private bool dead;

	// Use this for initialization
	void Start() 
	{	
		dead = false;
		EnemyCounter = GameObject.Find("EnemyCounter").GetComponent<GUIText>();
		
		Vector2 scale = GameController.self.GetTileScale();
		transform.localScale = new Vector3(scale.x, scale.y, 1.0f);
	}
	
	public Vector2 GetTilePosition()
	{
		Vector2 tileSize = GameController.self.GetTileSize();
		Vector3 position = transform.position;
		float xTranslate = tileSize.x * (GameController.self.NumTilesX / 2.0f);
		float yTranslate = tileSize.y * (GameController.self.NumTilesY / 2.0f);
		int posX = (int) ((position.x + xTranslate) / (tileSize.x));
		int posY = (int) ((position.y + yTranslate) / (tileSize.y));
		
		return new Vector2(posX, posY);
	}
	
	public Vector2 GetGoalPosition()
	{
		return GameController.self.GetBatteryPosition(0);
	}
	
	public Vector3 TileToWorld(Vector2 tilePosition)
	{
		Vector2 tileSize = GameController.self.GetTileSize();
		float xTranslate = tileSize.x * (GameController.self.NumTilesX / 2.0f);
		float yTranslate = tileSize.y * (GameController.self.NumTilesY / 2.0f);
		float posX = tilePosition.x * tileSize.x;
		float posY = tilePosition.y * tileSize.y;
		
		return new Vector3(posX - xTranslate + (tileSize.x / 2.0f), 
						   posY - yTranslate + (tileSize.y / 2.0f), 0.0f);
	}
	
	// Update is called once per frame
	void Update() 
	{
		walkingDirection = (TileToWorld(GetGoalPosition()) - transform.position).normalized;
		
		Vector2 walkingVector = new Vector2(walkingDirection.x, walkingDirection.y);
		Rigidbody2D rgdbd = GetComponent<Rigidbody2D>();
		Vector2 rgbdPos = rgdbd.position;
		rgdbd.MovePosition(rgbdPos + walkingVector * walkingSpeed * Time.deltaTime);
	}
	
	public void SetDead()
	{
		if (!dead)
		{
			dead = true;
			GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
			Destroy(gameObject);
			Debug.Log("Set Dead");
			GameController.self.DecrementEnemies();
		}
	}
	
	void OnCollisionStay2D(Collision2D coll)
	{
		Tile tile = coll.gameObject.GetComponent<Tile>();
		if (tile != null)
		{
			// Make sure the angle between the collided tile and the walking direction isn't
			// too big, otherwise the mosnter will just eat through anything
			if (Vector3.Angle(walkingDirection, (coll.transform.position - transform.position)) < 10 ||
			    Vector3.Angle(walkingDirection, (coll.transform.position - transform.position)) > -10);
				tile.DestroyByMonster(destroyAmount * Time.deltaTime);
		}
	}
}
