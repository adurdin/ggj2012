using UnityEngine;
 
public class VirtualButtonRegion : MonoBehaviour {
	public static bool VBpressed;	 // Button pressed
	public Color activeColor;			// Button's color when active.
	public Color inactiveColor;			// Button's color when inactive.
	public Texture2D background2D;		// Background's Image.
	private GameObject backOBJ;			// Background's Object.
	private GUITexture background;		// Background's GUI.
	private Vector2 origin;				// Touch's Origin in Screen-Space.
	private Vector2 position;			// Pixel Position in Screen-Space.
	private int size;					// Screen's smaller side.
	private bool gotPosition;			// Button has a position.
	private int fingerID;				// ID of finger touching this Button.
	private int lastID;					// ID of last finger touching this Button.
	private bool enable;				// VJR external control.
	
	//
	
	public void DisableButton() {enable = false; ResetButton();}
	public void EnableButton() {enable = true; ResetButton();}
	
	//
 
	private void ResetButton() {
		VBpressed = false;
		lastID = fingerID; fingerID = -1;
		position = origin; gotPosition = false;
	}
 
	private void GetPosition() {
		foreach (Touch touch in Input.touches) {
			fingerID = touch.fingerId;
			if (fingerID >= 0 && fingerID < Input.touchCount) {
				if(Input.GetTouch(fingerID).position.x > Screen.width*2/3 && Input.GetTouch(fingerID).position.y < Screen.height/3 && Input.GetTouch(fingerID).phase == TouchPhase.Began) {
					position = Input.GetTouch(fingerID).position; origin = position;
					background.texture = background2D; background.color = activeColor;
					gotPosition = true;
					print("Got position:" + position);
				}
			}
		}
	}
 
	private void GetConstraints() {
		if (origin.x > (Screen.width-background.pixelInset.width/2)+25) {origin.x = (Screen.width-background.pixelInset.width/2)+25;}
		if (origin.y < (background.pixelInset.height/2)+25) {origin.y = (background.pixelInset.height/2)+25;}
		if (origin.x < Screen.width*2/3) {origin.x = Screen.width*2/3;}
		if (origin.y > Screen.height/3) {origin.y = Screen.height/3;}
	}
	
	private Vector2 GetControls(Vector2 pos, Vector2 ori) {
		Vector2 vector = new Vector2();
		if (Vector2.Distance(pos,ori) > 0) {vector = new Vector2(pos.x-ori.x,pos.y-ori.y);}
		return vector;
	}
 
	//
	
	private void Awake() {
		gameObject.transform.localScale = new Vector3(0,0,0);
		gameObject.transform.position = new Vector3(0,0,999);
		if (Screen.width > Screen.height) {size = Screen.height;} else {size = Screen.width;}
		backOBJ = new GameObject("VJR-Button Back");
		backOBJ.transform.localScale = new Vector3(0,0,0);
		background = backOBJ.AddComponent("GUITexture") as GUITexture;
		background.texture = background2D; background.color = inactiveColor;
		fingerID = -1; lastID = -1;
		position = new Vector2((Screen.width*2/3)+Screen.width/3/2,(Screen.height/3)/2); origin = position;
		gotPosition = false; EnableButton(); enable = true;
	}
	
	private void Update() {
		if (fingerID > -1 && fingerID >= Input.touchCount) {ResetButton();}
		if (enable == true) {
			VBpressed = false;
			if (Input.touchCount > 0 && gotPosition == false) {GetPosition(); GetConstraints();}
			if (Input.touchCount > 0 && fingerID > -1 && fingerID < Input.touchCount && gotPosition == true) {
				VBpressed = true;
			}
			print("pressed:" + VBpressed);
			if (gotPosition == true && Input.touchCount > 0 && fingerID > -1 && fingerID < Input.touchCount) {
				if (Input.GetTouch(fingerID).phase == TouchPhase.Ended || Input.GetTouch(fingerID).phase == TouchPhase.Canceled) {ResetButton();}
			}
			if (gotPosition == false && fingerID == -1) {if (background.color != inactiveColor) {background.color = inactiveColor;}}
			background.pixelInset = new Rect(origin.x-(background.pixelInset.width/2),origin.y-(background.pixelInset.height/2),size/5,size/5);
		} else if (background.pixelInset.width > 0) {background.pixelInset = new Rect(0,0,0,0);}
	}
}