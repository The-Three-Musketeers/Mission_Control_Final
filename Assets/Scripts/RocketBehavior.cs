using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RocketBehavior : MonoBehaviour {

    public ParticleSystem particleSyst = null;
    public Transform cam = null;

    int numFuelPods = 3;
	int fuelPos = 0;
	float moveSpeed = 5.0f;

	public static bool launch = false;

	Vector3 prevRocketDirectionVector = new Vector3 (0, 0, 0);
	Vector3 prevDirection = new Vector3 (0, 0, 0);

	Vector3 prevPos = new Vector3 (0, 0, 0);
    float rocketX = 0;
	float rocketY = 0;
    float rocketZ = 0;

    float initialX = 0;
	float initialY = 0;

	float velocity = RocketState.fuel;

	float angleRad = RocketState.angle * ((float)Math.PI) / 180;
		
	float gameTime = 0f;
	float time = 0f;
	float timeMult = 0f;

	Boolean turning = true;

    //Win/Lose conditions:
    public int min_height = 6000;
    public int max_height = 10000;

	// Update is called once per frame
	void FixedUpdate () {
        // Listens for lift off key

        initialX = transform.position.x;
        initialY = transform.position.y;
        rocketX = transform.position.x;
        rocketY = transform.position.y;
        rocketZ = transform.position.z;

        if (Input.GetKeyDown(KeyCode.Return) && launch == false) {

			Debug.Log("Lift off!!!");
			launch = true;
            //particleSyst.Play();
            //ScreenChanges.launch_sounds();

            velocity = RocketState.fuel;
			angleRad = (180 - RocketState.angle) * ((float)Math.PI) / 180;
		}

		Vector3 dVector = (transform.position - prevPos).normalized;

		// Handles moving rocket
		if (launch == true) {
            //Switch the GUI mode:
            GUISwitch.launch_mode();
            // update position of rocket
			dVector = (transform.position - prevPos).normalized;
			Vector3 RocketDirectionVector = (new Vector3((float) Math.Cos(angleRad), (float) Math.Sin(angleRad), 0).normalized)*velocity;
			Quaternion vQ = Quaternion.LookRotation (Vector3.forward, RocketDirectionVector);
			Vector3 toVec = vQ.ToEulerAngles();

			Vector3 move = launchSequence();

			if (Math.Pow(gameTime,3) < velocity) {
				move = launchSequence();
			} else if (turning && Quaternion.Angle(transform.rotation,vQ) > 5f) {
				move = launchToPathTransition (RocketDirectionVector,vQ);
			} else {
				move = trajectory(dVector);
			}



			// update time
			/*time += Time.deltaTime * timeMult;
			if (timeMult < 4) {
				timeMult += 0.01f;
			}*/
			// update position variables
			prevPos = transform.position;
            float old_y_pos = transform.position.y;
            transform.position = move;
            float new_y_pos = transform.position.y;

            if (new_y_pos - old_y_pos <= -10)
            {
                //If it's too low
                if (new_y_pos < min_height)
                {
                    launch = false;
                    ScreenChanges.launch_sounds();
                    ScreenChanges.staticSpecificScene("Lose_Screen_Low");
                }
                //If it's too high
                else if (new_y_pos > max_height)
                {
                    launch = false;
                    ScreenChanges.launch_sounds();
                    ScreenChanges.staticSpecificScene("Lose_Screen_High");
                }
                //If it's within the window of success
                else
                {
                    launch = false;
                    ScreenChanges.launch_sounds();
                    ScreenChanges.staticSpecificScene("Win_Screen");
                }
            }

        }

		// Handles dropping fuel pods
		if ( (Input.GetKeyDown(KeyCode.Space)) && (fuelPos < numFuelPods) ) {
			//Transform pod = transform.GetChild(0);
			//pod.parent = null;
			//pod.gameObject.AddComponent<Rigidbody>();
			//fuelPos += 1;
			changeAngle (20,dVector);
		}
	}

	void changeAngle(float amount, Vector3 dVector) {
		// change angle if fuel drop
		velocity = (float) (Math.Sqrt ((4.9 * rocketX) / (Math.Sin (angleRad) * Math.Cos (angleRad)))); //gets curr velocity
		angleRad = (float) (Math.PI - Math.Cos(dVector.x)); //sets angleRad to current angle
		initialX = rocketX;
		initialY = rocketY;
		time = 0; //resets equation
		Debug.Log((angleRad * 180 / Math.PI).ToString());
		angleRad -= amount * ((float)Math.PI) / 180; // makes new angle
		Debug.Log((angleRad * 180 / Math.PI).ToString());
	}

	/* This function moves the rocket in an upwards direction
	 * at a cubicaly increasing rate */
	Vector3 launchSequence() {

		gameTime += Time.deltaTime;
		rocketY += gameTime*gameTime*gameTime;
		return new Vector3 (rocketX, rocketY, rocketZ);
	}

	/* This function moves and rotates the rocket so that 
	 * it can smoothly transition to follow the trajectory
	 * given by the user input */
	Vector3 launchToPathTransition(Vector3 RocketDirection, Quaternion vQ) {
		// rotate the rocket
		transform.rotation = Quaternion.Slerp(transform.rotation, vQ, 0.5f*Time.deltaTime);
		GameObject.Find("HUD").transform.forward = cam.forward;
		// move the position of the rocket
		rocketX += RocketDirection.x;
		rocketY += RocketDirection.y;
		initialX = rocketX;
		initialY = rocketY;
		return new Vector3 (rocketX, rocketY, rocketZ);
	}

	Vector3 trajectory(Vector3 dVec) {
		turning = false;
		Quaternion vQ = Quaternion.LookRotation (Vector3.forward, dVec);
		transform.rotation = Quaternion.Slerp(transform.rotation, vQ, 0.5f*Time.deltaTime);
		GameObject.Find("HUD").transform.forward = cam.forward;
		rocketX = initialX + velocity * ((float)Math.Cos(angleRad)) * time;
		rocketY = (float) (initialY + velocity * Math.Sin(angleRad) * time - 0.5 * 9.8 * time * time);
		time += Time.deltaTime;
		return new Vector3(rocketX,rocketY,rocketZ);
	}

	// Use this for initialization
	void Start () {


	}
}
