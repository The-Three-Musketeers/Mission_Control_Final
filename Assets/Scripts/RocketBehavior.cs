using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This script handles all behavior relating to the rocket launch.
//Lots goes on in here, so comments are placed appropriately.

public class RocketBehavior : MonoBehaviour {

    //Public variables for the particle system and camera
    public ParticleSystem particleSyst = null;
    public Transform cam = null;

    //Internal variables for the dropping of fuel tanks
    int numFuelPods = 3;
	int fuelPos = 0;

    //This keeps track of the launch mode
	public static bool launch = false;

    //Some position vectors
	Vector3 prevPos = new Vector3 (0, 0, 0);
    float initialX = 0;
    float initialY = 0;
    float rocketX = 0;
	float rocketY = 0;
    float rocketZ = 0;

    //Fuel velocity
    float velocity = RocketState.fuel * 5;

    //Angle variables
	float angleRad = (180 - RocketState.angle) * ((float)Math.PI) / 180;
	float currAngle = (180 - RocketState.angle) * ((float)Math.PI) / 180;
		
    //Time variables for segmenting the launch
	float gameTime = 0f;
	float time = 0f;
	float prevTime = 0f;
	float timeMult = 0f;

    //For determing whether or not the rocket is turning
	Boolean turning = true;

    //Win/Lose conditions:
    public int min_height = 3300000;
    public int max_height = 6600000;

	// Update is called once per frame
	void FixedUpdate () {
 
        initialX = transform.position.x;
        initialY = transform.position.y;
        rocketX = transform.position.x;
        rocketY = transform.position.y;
        rocketZ = transform.position.z;


        if (launch == false) {
            //Set the initial conditions for the launch
            Camera.reset();
            velocity = RocketState.fuel;
			angleRad = (180 - RocketState.angle) * ((float)Math.PI) / 180;
			currAngle = (180 - RocketState.angle) * ((float)Math.PI) / 180;
		}
			
        //When the launchPad animation is done...
        if (LaunchPad.animationDone == true) {
            //Set the launch mode, play the particle system and rocket sounds
            GUISwitch.launch_mode();
            particleSyst.Play();
            ScreenChanges.launch_sounds();
            LaunchPad.reset();
        }

		Vector3 dVector = (transform.position - prevPos).normalized;
		// Handles moving rocket
		if (particleSyst.isPlaying) {
            //Switch camera position
            Camera.launchShift();
            StartExplosion.Explode();
            // update position of rocket
			dVector = (transform.position - prevPos).normalized;
			Vector3 RocketDirectionVector = (new Vector3((float) Math.Cos(angleRad), (float) Math.Sin(angleRad), 0).normalized)*velocity;
			Quaternion vQ = Quaternion.LookRotation (Vector3.forward, RocketDirectionVector);
			Vector3 toVec = vQ.ToEulerAngles();

			Vector3 move = launchSequence();

            //Update the position based on the different parts of the launch sequence
			if (Math.Pow(gameTime,3) < velocity) {
				move = launchSequence();
			} else if (turning && Quaternion.Angle(transform.rotation,vQ) > 5f) {
				move = launchToPathTransition (RocketDirectionVector,vQ);
			} else {
				move = trajectory(dVector);
			}
            //Reorient the camera
            GameObject.Find("HUD").transform.forward = cam.forward;
			// update position variables
			prevPos = transform.position;
            float old_y_pos = transform.position.y;
            transform.position = move;
            float new_y_pos = transform.position.y;

            //Check for leaving the atmosphere
            if (new_y_pos > 33000) {
                Skybox.leavingAtmosphere();
            }

            if (new_y_pos - old_y_pos <= -10) {
                //If it's too low, switch contexts to the losing screen
                if (new_y_pos < min_height) {
                    launch = false;
                    ScreenChanges.launch_sounds();
                    Skybox.reset();
                    ScreenChanges.staticSpecificScene("Lose_Screen_Low");
                }
                //If it's too high, switch contexts to the losing screen
                else if (new_y_pos > max_height) {
                    launch = false;
                    ScreenChanges.launch_sounds();
                    Skybox.reset();
                    ScreenChanges.staticSpecificScene("Lose_Screen_High");
                }
                //If it's within the window of success, switch contexts to the winning screen
                else {
                    launch = false;
                    ScreenChanges.launch_sounds();
                    Skybox.reset();
                    ScreenChanges.staticSpecificScene("Win_Screen");
                }
            }

			Debug.Log(((dVector.magnitude) / (time-prevTime)).ToString());
        }

		// Handles dropping fuel pods
		if ( (Input.GetKeyDown(KeyCode.Space)) ) {
			foreach (Transform pod in transform) {
				if (pod.name.StartsWith ("tank")) {
					pod.parent = null;
					pod.gameObject.AddComponent<Rigidbody>();
					changeAngle (10,dVector);
					break;
				}
			}
		}
	}

	void changeAngle(float amount, Vector3 dVector) {
		// change angle if fuel drop
		//velocity = (float) (Math.Sqrt ((4.9 * rocketX) / (Math.Sin (angleRad) * Math.Cos (angleRad)))); //gets curr velocity
		velocity = (dVector.magnitude) / (time-prevTime);
		angleRad = (float) (Math.PI - Math.Cos(dVector.x)); //sets angleRad to current angle
		initialX = rocketX;
		initialY = rocketY;
		time = 0; //resets equation
		angleRad -= amount * ((float)Math.PI) / 180; // makes new angle
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
		prevTime = time;
		time += Time.deltaTime;
		return new Vector3(rocketX,rocketY,rocketZ);
	}
}
