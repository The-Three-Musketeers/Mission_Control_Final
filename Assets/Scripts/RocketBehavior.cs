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

	float velocity = 500;//RocketState.fuel;

	float angleRad = RocketState.angle * ((float)Math.PI) / 180;
	float currAngle = RocketState.angle * ((float)Math.PI) / 180;
		
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
            particleSyst.Play();
            ScreenChanges.launch_sounds();

            velocity = RocketState.fuel * 5;
			angleRad = (180 - RocketState.angle) * ((float)Math.PI) / 180;
			currAngle = (180 - RocketState.angle) * ((float)Math.PI) / 180;
		}

		// Handles moving rocket
		if (launch == true) {
            //Switch the GUI mode:
            GUISwitch.launch_mode();
            // update position of rocket
			Vector3 dVector = (transform.position - prevPos).normalized;
			Vector3 RocketDirectionVector = (new Vector3((float) Math.Cos(angleRad), (float) Math.Sin(angleRad), 0).normalized)*velocity;
			Quaternion vQ = Quaternion.LookRotation (Vector3.forward, RocketDirectionVector);
			Vector3 toVec = vQ.ToEulerAngles();

			Vector3 move = launchSequence();

			if (Math.Pow(gameTime,3) < velocity) {
				move = launchSequence();
			} else if (Quaternion.Angle(transform.rotation,vQ) > 5f) {
				move = launchToPathTransition (RocketDirectionVector,vQ);
			} else {
				move = trajectory(vQ);
			}

			// update time
			time += Time.deltaTime * timeMult;
			if (timeMult < 4) {
				timeMult += 0.01f;
			}
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
			Transform pod = transform.GetChild(0);
			pod.parent = null;
			pod.gameObject.AddComponent<Rigidbody>();
			fuelPos += 1;
			changeAngle (3);
		}
	}
		
	void changeAngle(float amount) {
		// change angle if fuel drop
		velocity = (float) (Math.Sqrt ((4.9 * rocketX) / (Math.Sin (angleRad) * Math.Cos (angleRad)))); //gets curr velocity
		initialX = rocketX;
		initialY = rocketY;
		time = 0; //resets equation
		angleRad += amount * ((float)Math.PI) / 180; // makes new angle
	}

	Vector3 launchSequence() {
		/* This function moves the rocket in an upwards direction
		 * at a cubicaly increasing rate
		 */
		gameTime += Time.deltaTime;
		rocketY += gameTime*gameTime*gameTime;
		return new Vector3 (rocketX, rocketY, rocketZ);
	}

	Vector3 launchToPathTransition(Vector3 RocketDirection, Quaternion vQ) {
		/* This function moves and rotates the rocket so that 
		 * it can smoothly transition to follow the trajectory
		 * given by the user input
		 */
		// rotate the rocket
		transform.rotation = Quaternion.Lerp(transform.rotation, vQ, 0.3f*Time.deltaTime);
		// move the position of the rocket
		rocketX += RocketDirection.x;
		rocketY += RocketDirection.y;
        GameObject.Find("HUD").transform.forward = cam.forward;
        return new Vector3 (rocketX, rocketY, rocketZ);
	}

	Vector3 trajectory(Quaternion vQ) {
		turning = false;
		transform.rotation = vQ;
		rocketX = initialX + velocity * ((float)Math.Cos(angleRad)) * time;
		rocketY = (float) (initialY + velocity * Math.Sin(angleRad) * time - 0.5 * 9.8 * time * time);
		return new Vector3(rocketX,rocketY,rocketZ);
	}

	// Use this for initialization
	void Start () {

	}
}
