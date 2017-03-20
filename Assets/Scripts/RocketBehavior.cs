using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This script handles all behavior relating to the rocket launch.
// Lots goes on in here, so comments are placed appropriately.

public class RocketBehavior : MonoBehaviour {

    // Public variables for the particle system and camera
    public ParticleSystem particleSyst = null;
    public Transform stars;
    public Transform cam = null;

    // Internal variables for the dropping of fuel tanks
    int numFuelPods = 3;
	int fuelPos = 0;

    // This keeps track of the launch mode
	public static bool launch = false;

    // Some position vectors
	Vector3 prevPos = new Vector3 (0, 0, 0);
    float initialX = 0;
    float initialY = 0;
    float rocketX = 0;
	float rocketY = 0;
    float rocketZ = 0;
	float old_y_pos = 0;
	float new_y_pos = 0;

    // Fuel velocity
    float velocity = RocketState.fuel * 5;

    // Angle variables
	float angleRad = (180 - RocketState.angle) * ((float)Math.PI) / 180;
	float currAngle = (180 - RocketState.angle) * ((float)Math.PI) / 180;
		
    // Time variables for segmenting the launch
	float gameTime = 0f;
	float time = 0f;
	float prevTime = 0f;
	float timeMult = 0f;

    // For determing whether or not the rocket is turning
	Boolean turning = true;

    // Win/Lose conditions:
    public int min_height = 100000;
    public int max_height = 400000;

	int LaunchPadHeight = 700;


	/* This function is called once per frame and is used to 
	 * update the game
	*/
	void FixedUpdate () {
        initialX = transform.position.x;
        initialY = transform.position.y;
        rocketX = transform.position.x;
        rocketY = transform.position.y;
        rocketZ = transform.position.z;

        if (launch == false) {
            //Set the initial conditions for the launch
            Camera.reset();
            Skybox.reset();
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
            // Change camera position and modify stars
            Camera.launchShift();
            stars.transform.forward = cam.forward;
            StartExplosion.Explode();

            // update position of rocket
			dVector = (transform.position - prevPos).normalized;
			Vector3 RocketDirectionVector = (new Vector3((float) Math.Cos(angleRad), (float) Math.Sin(angleRad), 0).normalized)*velocity;
			Quaternion vQ = Quaternion.LookRotation (Vector3.forward, RocketDirectionVector);
			Vector3 toVec = vQ.ToEulerAngles();

			Vector3 move = launchPhase_TakeOff();

            //Update the position based on the different parts of the launch sequence
			if (Math.Pow(gameTime,3) < velocity || transform.position.y < LaunchPadHeight) {
				move = launchPhase_TakeOff();
			} else if (turning && Quaternion.Angle(transform.rotation,vQ) > 5f) {
				move = launchPhase_TurnToAngle (RocketDirectionVector,vQ);
			} else {
				move = launchPhase_PhysicsTrajectory(dVector);
			}

            // Reorient the camera
            GameObject.Find("HUD").transform.forward = cam.forward;

			// update position variables
			prevPos = transform.position;
            old_y_pos = transform.position.y;
            transform.position = move;
            new_y_pos = transform.position.y;

			// Check win/lose conditions
			check_win_lose();
        }

		// Handles dropping fuel pods 
		if ( (Input.GetKeyDown(KeyCode.Space)) ) {
			dropPod (dVector);
			// Adjust the rockets trajectory if users drops fuel pod too early/late
			changeAngle (0,dVector,turning);
		}
	}


	/* This function drops one fuel pod
	*/
	void dropPod(Vector3 dVector) {
		Transform[] ts = gameObject.GetComponentsInChildren<Transform>();                  // get the components of the rocketship
		foreach (Transform pod in ts) {
			if (pod.name.StartsWith ("tank")) { 				                           // iterate through components and look for fuel tanks
				pod.parent = null;                                                         // remove component from the parent
				pod.gameObject.AddComponent<Rigidbody>();                                  // add physics engine (rigidbody) to component
				pod.gameObject.GetComponent<Rigidbody> ().velocity = (dVector) * velocity; // give a velocity away from rocket
				break;                                                                     // only drop 1 fuel pod
			}
		}
	}


	/* This function changes the trajectory of the rocketship by adjusting
	 * the angle it follows 
	*/
	void changeAngle(float amount, Vector3 dVector, Boolean turning) {
		if (!turning) {                                                         // only change angle if rocket is turning
			velocity = (dVector.magnitude) / (time - prevTime);
		}
		angleRad = (float) (Math.PI - Math.Cos(dVector.x));                     // sets angleRad to current angle
		initialX = rocketX;    
		initialY = rocketY;
		time = 0;                                                               // resets equation
		angleRad -= amount * ((float)Math.PI) / 180;                            // makes new angle
	}


	/* This function moves the rocket in an upwards direction at a cubicaly
	 * increasing rate 
	*/
	Vector3 launchPhase_TakeOff() {
		gameTime += Time.deltaTime;
		rocketY += gameTime*gameTime*gameTime;
		return new Vector3 (rocketX, rocketY, rocketZ);
	}


	/* This function moves and rotates the rocket so that it can smoothly 
	 * transition to follow the trajectory given by the user input 
	*/
	Vector3 launchPhase_TurnToAngle(Vector3 RocketDirection, Quaternion vQ) {
		// rotate the rocket
		transform.rotation = Quaternion.Slerp(transform.rotation, vQ, 0.5f * Time.deltaTime);
		GameObject.Find("HUD").transform.forward = cam.forward;
		// move the position of the rocket
		rocketX += RocketDirection.x;
		rocketY += RocketDirection.y;
		initialX = rocketX;
		initialY = rocketY;
		return new Vector3 (rocketX, rocketY, rocketZ);
	}


	/* This function moves the rocket along a trajectory based on the basic
	 * projectile equation from physics (throwing a ball) 
	*/
	Vector3 launchPhase_PhysicsTrajectory(Vector3 dVec) {
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


	/* This function checks if the rocket has reached a win or lose condition
	 * and loads a corresponding scene
	*/
	void check_win_lose() {
		//Check for leaving the atmosphere
		if (new_y_pos > 100000) {
			Skybox.leavingAtmosphere();
		}

		//Check for lose conditions: Too High
		if (new_y_pos > max_height) {
			launch = false;
			ScreenChanges.staticSpecificScene("Lose_Screen_High");
		}

		//Check for lose conditions: Too Low or just right when it starts to turn
		if (new_y_pos - old_y_pos <= -10) {
			if (new_y_pos < min_height) {                                      // If it's too low, switch contexts to the losing screen
				launch = false;
				ScreenChanges.staticSpecificScene("Lose_Screen_Low");
			} 
			else { 			                                                   //Otherwise it's just right!
				launch = false;
				ScreenChanges.launch_sounds();
				if (GameState.get_mission() == "Satellite") {
					ScreenChanges.staticSpecificScene("Win_Screen_Satellite");
				}
				else if (GameState.get_mission() == "Shuttle") {
					ScreenChanges.staticSpecificScene("Win_Screen_Shuttle");
				}
				else if (GameState.get_mission() == "Mars") {
					ScreenChanges.staticSpecificScene("Win_Screen_Mars");
				}
				else {
					Debug.Log("Something went wrong with the Win conditions!");
				}
			}
		}
	}



} // End Class: RocketBehavoir



