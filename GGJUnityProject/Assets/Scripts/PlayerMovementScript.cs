using UnityEngine;
using System.Collections;

public class PlayerMovementScript : MonoBehaviour {

    public float baseSpeed = 10.0f;

    public float verticalBoundingBox = 7.0f;
    public float verticalStartPosition = 0.0f;

    public float horizontalBoundingBox = 12.0f;
    public float horizontalStartPosition = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //no more moving if the game is lost
        if (Globals.GameLost)
        {
            return;
        }
		
        //moving left and right
        float inputX = Input.GetAxis("Horizontal") * baseSpeed;
		inputX += VirtualJoystickRegion.VJRnormals.x * baseSpeed;
        inputX *= (Time.deltaTime) ;
        if (inputX > 0)
        {
            //going right
            //keep within right side of bounding box
            if (transform.position.z < (horizontalStartPosition + horizontalBoundingBox))
            {
                transform.Translate(inputX, 0, 0);
               // Globals.PlayerLife += 1;
            }

        }

        //this way 0.0 is not handled

        if (inputX < 0)
        {
            //going left
            //keep within right side of bounding box
            if (transform.position.z > (horizontalStartPosition - horizontalBoundingBox))
            {
                transform.Translate(inputX, 0, 0);
               
                
            }
        }




        //moving up and down
        float inputY = Input.GetAxis("Vertical") * baseSpeed;
		inputY += VirtualJoystickRegion.VJRnormals.y * baseSpeed;
        inputY *= Time.deltaTime;

        if (inputY > 0)
        {
            //going up
            // keep it within the upper bounding box
            if (transform.position.y < (verticalStartPosition + verticalBoundingBox))
            {
                transform.Translate(0, inputY, 0);
            }

           
        }

        //this way 0.0 will not be handled

        if (inputY < 0) 
        {
            //Debug.Log("going down?");
            //going down
            //keep it within the lower bounding box
            if (transform.position.y > (verticalStartPosition - verticalBoundingBox))
            {
                transform.Translate(0, inputY, 0);
            }

            

        }
        //float translationVertical = Input.GetAxis("Vertical") * speed;
        //translationVertical *= Time.deltaTime;
	}
}
