using UnityEngine;
using System.Collections;

public class PlayerMovementScript : MonoBehaviour {

    public float baseSpeed;

    public float verticalBoundingBox;
    public float verticalStartPosition;

    public float horizontalBoundingBox;
    public float horizontalStartPosition;

	// Use this for initialization
	void Start () {

        baseSpeed = 10.0F;

        verticalBoundingBox = 4.0F;
        verticalStartPosition = 0.0F;

        horizontalBoundingBox = 2.0F;
        horizontalStartPosition = 0.0F;

	}
	
	// Update is called once per frame
	void Update () {


        
        

        //float translationHorizontal = Input.GetAxis("Horizontal") * speed;
        //translationHorizontal *= Time.deltaTime;

        //transform.Translate(translationHorizontal, 0, 0);
        

        //moving left and right
        float inputX = Input.GetAxis("Horizontal") * baseSpeed;
        inputX *= (Time.deltaTime) ;
        if (inputX > 0)
        {
            //going right
            //keep within right side of bounding box
            if (transform.position.x < (horizontalStartPosition + horizontalBoundingBox))
            {
                transform.Translate(inputX, 0, 0);
                Globals.PlayerLife += 1;
            }

        }

        //this way 0.0 is not handled

        if (inputX < 0)
        {
            //going left
            //keep within right side of bounding box
            if (transform.position.x > (horizontalStartPosition - horizontalBoundingBox))
            {
                transform.Translate(inputX, 0, 0);
                Globals.PlayerLife -= 1;
            }
        }




        //moving up and down
        float inputY = Input.GetAxis("Vertical") * baseSpeed;
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

            
        //}


	}
}