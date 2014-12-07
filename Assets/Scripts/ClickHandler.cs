using UnityEngine;
using System.Collections;

public class ClickHandler : MonoBehaviour 
{
	
	bool mouseLDown = false;
	bool mouseRDown = false;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Apparently GetMouseButtonDown is a lot slower, perhaps
		// something to look into?
		if (Input.GetMouseButton(0))
			if (!mouseLDown)
			{
				OnMouseDown(0);
				mouseLDown = true;
			}
		else
			if (mouseLDown)
				mouseLDown = false;
				
		if (Input.GetMouseButton(2))
			if (!mouseRDown)
		{
			OnMouseDown(1);
			mouseRDown = true;
		}
		else
			if (mouseRDown)
				mouseRDown = false;
	}
	
	void OnMouseDown(int button)
	{
		Vector2 tileSize = GameController.self.GetTileSize();
		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float xTranslate = tileSize.x * (GameController.self.NumTilesX / 2.0f);
		float yTranslate = tileSize.y * (GameController.self.NumTilesY / 2.0f);
		int posX = (int) ((position.x + xTranslate) / (tileSize.x));
		int posY = (int) ((position.y + yTranslate) / (tileSize.y));
		
		if (button == 0)
			GameController.self.AddTile<ConductTile>(posX, posY);
		else if (button == 1)
			GameController.self.RemoveTile(posX, posY);
	}
}
