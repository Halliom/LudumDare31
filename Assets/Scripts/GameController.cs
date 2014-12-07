using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{

	public static GameController self;

	public int NumTilesX;
	
	public int NumTilesY;
	
	private Tile[,] Tiles;
	
	public GameObject InstMonster;
	public GameObject InstTile;
	
	private GUIText WaveCounter;
	private GUIText EnemyCounter;
	private GUIText ResourceCounter;
	
	private Bounds SpawnZoneTop;
	private Bounds SpawnZoneBottom;
	private Bounds SpawnZoneLeft;
	private Bounds SpawnZoneRight;
	
	private int NumResources;
	
	private int Enemies;
	
	private string[] sides;
	private string[] SIDES = {"Top", "Bottom", "Left", "Right"};
	
	private float startupTimer;
	private bool startedUp;
	
	private int Wave;

	// Use this for initialization
	void Awake() 
	{
		NumResources = 13;
		self = this;
		Tiles = new Tile[NumTilesX, NumTilesY];
		sides = new string[4];
		
		AddTile<BatteryTile>(NumTilesX / 2, NumTilesY / 2);
		startupTimer = 1.0f;
		startedUp = false;
	}
	
	public void DecrementEnemies()
	{
		Enemies--;
		if (Enemies <= 0)
		{
			GameController.self.OnWaveComplete();
		}
		EnemyCounter.text = "Enemies: " + Enemies;
	}
	
	public int GetWave()
	{
		return Wave;
	}
	
	public void AddResource()
	{
		NumResources++;
		ResourceCounter.text = "Resources: " + NumResources;
	}
	
	public void RemoveResource()
	{
		NumResources--;
		ResourceCounter.text = "Resources: " + NumResources;
	}
	
	void Start()
	{
		WaveCounter = GameObject.Find("WaveCounter").GetComponent<GUIText>();
		EnemyCounter = GameObject.Find("EnemyCounter").GetComponent<GUIText>();
		ResourceCounter = GameObject.Find("ResourceCounter").GetComponent<GUIText>();
		
		SpawnZoneTop = GameObject.Find("SpawnZoneTop").GetComponent<BoxCollider2D>().bounds;
		SpawnZoneBottom = GameObject.Find("SpawnZoneBottom").GetComponent<BoxCollider2D>().bounds;
		SpawnZoneLeft = GameObject.Find("SpawnZoneLeft").GetComponent<BoxCollider2D>().bounds;
		SpawnZoneRight = GameObject.Find("SpawnZoneRight").GetComponent<BoxCollider2D>().bounds;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!startedUp)
		{
			if (startupTimer > 0)
			{
				startupTimer -= Time.deltaTime;
			}
			else
			{
				ResourceCounter.text = "Resources: " + NumResources;
				Wave = 0;
				OnWaveComplete();
				startedUp = true;
			}
		}
	}
	
	public void AddTile<T>(int x, int y) where T : Tile
	{
		if (Tiles[x, y] == null && !IsOccupied(x, y))
		{
			//if (typeof(T) == typeof(BatteryTile) || NumResources > 0)
			{
				GameObject tile = (GameObject) Instantiate(InstTile);
				Tile comp = tile.AddComponent<T>();
				Tiles[x, y] = comp;
				comp.OnAdded(x, y);
				if (typeof(T) != typeof(BatteryTile))
					RemoveResource();
			}
		}
	}
	
	public bool IsOccupied(int x, int y)
	{
		Vector2 tileSize = GameController.self.GetTileSize();
		float xTranslate = tileSize.x * (GameController.self.NumTilesX / 2.0f);
		float yTranslate = tileSize.y * (GameController.self.NumTilesY / 2.0f);
		float posX = x * tileSize.x;
		float posY = y * tileSize.y;
		
		Vector3 worldPos = new Vector3(posX - xTranslate + (tileSize.x / 2.0f), 
		                   			   posY - yTranslate + (tileSize.y / 2.0f), 0.0f);
		     		     
		GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");   			   
		foreach (GameObject go in monsters)
		{
			if (go.GetComponent<BoxCollider2D>().bounds.Contains(worldPos))
				return true;
		}
		
		return false;
	}
	
	public void RemoveTile(int x, int y)
	{
		// TODO: chek for nullptr exception
		if (Tiles[x, y] != null)
		{
			Tiles[x, y].OnRemoved();
			Destroy(Tiles[x, y].gameObject);
			AddResource();
		}
		Tiles[x, y] = null;
	}
	
	public List<Tile> GetAdjacentTiles(int x, int y)
	{
		List<Tile> tiles = new List<Tile>();
		
		if (IsWithinBounds(x + 1, y))
			if (Tiles[x + 1, y] != null)
				tiles.Add(Tiles[x + 1, y]);
		if (IsWithinBounds(x - 1, y))
			if (Tiles[x - 1, y] != null)
				tiles.Add(Tiles[x - 1, y]);
		if (IsWithinBounds(x, y + 1))
			if (Tiles[x, y + 1] != null)
				tiles.Add(Tiles[x, y + 1]);
		if (IsWithinBounds(x, y - 1))
			if (Tiles[x, y - 1] != null)
				tiles.Add(Tiles[x, y - 1]);
			
		return tiles;
	}
	
	public bool IsWithinBounds(int x, int y)
	{
		if (x < 0 || x >= NumTilesX) return false;
		if (y < 0 || y >= NumTilesY) return false;
		return true;
	}
	
	public Vector2 GetBatteryPosition(int index)
	{
		for (int i = 0; i < NumTilesX; i++)
			for(int j = 0; j < NumTilesY; j++)
			{
				if (Tiles[i, j] != null)
					if (Tiles[i, j].GetType() == typeof(BatteryTile))
						return new Vector2(i, j);
			}
			
		return new Vector2(0.0f, 0.0f);
	}
	
	public void StartWave()
	{
		//int spawnAmount = (int) ((Wave * Wave + Mathf.Sqrt(Wave) + (3.0f / Wave)) / 5.0f);
		int spawnAmount = (int) Wave * 2;
		NumResources += 5;
		ResourceCounter.text = "Resources: " + NumResources;
	
		for (int i = 0; i < 4; i++)
		{
			if (sides[i] != null)
			{
				switch(sides[i])
				{
					case "Top":
					{
						SpawnInZone(SpawnZoneTop, spawnAmount);
						break;
					}
					case "Bottom":
					{
						SpawnInZone(SpawnZoneBottom, spawnAmount);
						break;
					}
					case "Left":
					{
						SpawnInZone(SpawnZoneLeft, spawnAmount);
						break;
					}
					case "Right":
					{
						SpawnInZone(SpawnZoneRight, spawnAmount);
						break;
					}
				}
			}
			sides[i] = null;
		}
		
		Enemies = spawnAmount;
		EnemyCounter.text = "Enemies: " + spawnAmount;
	}
	
	public void SpawnInZone(Bounds zone, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			float xPos = zone.min.x + (Random.value * (zone.max.x - zone.min.x));
			float yPos = zone.min.y + (Random.value * (zone.max.y - zone.min.y));
			float zPos = 0.0f;
			Instantiate(InstMonster, new Vector3(xPos, yPos, zPos), Quaternion.identity);
		}
	}
	
	public void OnWaveComplete()
	{
		Wave++;
		
		WaveCounter.text = "Wave: " + Wave;
		
		int numSides = 1 + (int) ((Mathf.Sqrt(Wave) + (1 / Wave)) / 4);
		
		float Skip = Random.value * 16 + (Random.value * 4) + Random.value;
		int side = 0;
		for (int i = 0; i < numSides; i++)
		{
			side += (int) Skip;
			side %= 5;
			side -= 1;
			while (side == -1)
			{
				side += (int) Skip;
				side %= 5;
				side -= 1;
			}

			string s_side = SIDES[side];
			sides[i] = s_side;
			GameObject warning = GameObject.Find("Warning" + SIDES[side]);
			warning.GetComponent<Warning>().StartWarning();
		}
	}
	
	public Vector2 GetTileSize()
	{
		// Calculates the visual rect
		// NOTE that orthographicSize is just half the height
		float ySize = 2 * Camera.main.orthographicSize;
		float xSize = ySize * Screen.width / Screen.height;
		
		return new Vector2(xSize / (float) NumTilesX, ySize / (float) NumTilesY);
	}
	
	public Vector2 GetTileScale()
	{
		return GetTileSize() * 3.125f;
	}
}
