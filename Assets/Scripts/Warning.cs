using UnityEngine;
using System.Collections;

public class Warning : MonoBehaviour 
{
	
	Sprite spr;
	
	SpriteRenderer spr_renderer;
	
	private float timer;
	
	public float OnscreenTime;
	
	private bool fading;
	
	// Use this for initialization
	void Start () 
	{
		fading = false;
		timer = 0;
		Vector2 screenSize = new Vector2(GameController.self.NumTilesX, GameController.self.NumTilesY);
		Vector2 scale = GameController.self.GetTileSize();
		transform.localScale = new Vector3(scale.x * screenSize.x * 0.10416f, 
										   scale.y * screenSize.y * 0.10416f);
		
		spr_renderer = GetComponent<SpriteRenderer>();
	
		spr = spr_renderer.sprite;
		spr_renderer.sprite = null;
	}
	
	// Update is called once per frame
	void Update() 
	{
		if (fading)
		{
			if (timer <= 0)
			{
				GameController.self.StartWave();
				spr_renderer.sprite = null;
				fading = false;
			}
			else
			{
				timer -= Time.deltaTime;
				spr_renderer.color = new Color(spr_renderer.color.r, spr_renderer.color.g, spr_renderer.color.b, Mathf.Abs(Mathf.Sin(OnscreenTime - timer)));
			}
		}
	}
	
	public void StartWarning()
	{
		spr_renderer.sprite = spr;
		timer = OnscreenTime;
		fading = true;
	}
}
