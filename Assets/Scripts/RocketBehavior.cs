using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This script handles all behavior relating to the rocket launch.
// Lots goes on in here, so comments are placed appropriately.

public class RocketBehavior : MonoBehaviour {

    // Public variables for the particle system and camera
    public ParticleSystem particleSyst = null;
    public ParticleSystem smoke = null;
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

    // Fuel velocity
    float velocity = RocketState.fuel + 20;

    // Angle variables
	float angleRad = (180 - RocketState.angle) * ((float)Math.PI) / 180;
		
    // Time variables for segmenting the launch
	float gameTime = 0f;
	float time = 0f;
	float prevTime = 0f;

    // For determing whether or not the rocket is turning
	Boolean turning = true;

    // Win/Lose conditions:
    public int atmosphere_height = 120000;
    int min_height = 0;
    int max_height = 0;

	int LaunchPadHeight = 700;

    //Start out by determining the min and max height based
    //on the selected mission
    private void Start() {
        if (GameState.get_mission() == "Satellite") {
            min_height = 130000;
            max_height = 275000;
        }
        else if (GameState.get_mission() == "Shuttle") {
            min_height = 230000;
            max_height = 375000;
        }
        else if (GameState.get_mission() == "Mars") {
            min_height = 330000;
            max_height = 660000;
        }
    }

	/* This function is called once per frame and is used to 
	 * update the game
	*/
	void FixedUpdate () {
        initialX = transform.position.x;
        initialY = transform.position.y;
        rocketX = transform.position.x;
        rocketY = transform.position.y;
        rocketZ = transform.position.z;

        //If the rocket is not in launch mode, keep it in a state of reset
        if (launch == false) {
            // Set the initial conditions for the launch
            Camera.reset();
            Skybox.reset();
            velocity = RocketState.fuel + 20;
			angleRad = (180 - RocketState.angle) * ((float)Math.PI) / 180;
		}
			
        // When the launchPad animation is done...
        if (LaunchPad.animationDone == true) {
            // Set the launch mode, play the particle system and rocket sounds
            GUISwitch.launch_mode();
            particleSyst.Play();
            smoke.Play();
            ScreenChanges.launch_sounds();
            LaunchPad.reset();
        }

		Vector3 rocket_direction = (transform.position - prevPos).normalized;

		// Handles moving rocket
		if (particleSyst.isPlaying) {
            // Change camera position and modify stars
            Camera.launchShift();
            stars.transform.forward = cam.forward;
            StartExplosion.Explode();

            // update position of rocket
			rocket_direction = (transform.position - prevPos).normalized;
			Vector3 input_angle_vector = (new Vector3((float) Math.Cos(angleRad), (float) Math.Sin(angleRad), 0).normalized)*velocity;
			Quaternion input_angle_rotation = Quaternion.LookRotation (Vector3.forward, input_angle_vector);

			Vector3 move = transform.position;

            // Update the position based on the different parts of the launch sequence
			if (Math.Pow(gameTime,3) < velocity || transform.position.y < LaunchPadHeight) {
				move = launchPhase_TakeOff();
			} else if (turning && Quaternion.Angle(transform.rotation,input_angle_rotation) > 5f) {
				move = launchPhase_TurnToAngle (input_angle_vector,input_angle_rotation);
			} else {
				move = launchPhase_PhysicsTrajectory(rocket_direction);
			}

            // Reorient the camera
            GameObject.Find("HUD").transform.forward = cam.forward;

			// update position variables
			prevPos = transform.position;
            transform.position = move;

			// Check win/lose conditions
			check_win_lose(prevPos.y, transform.position.y);
        }

		// Handles dropping fuel pods - This incomplete functionality has been removed for the first release
		/*if ( (Input.GetKeyDown(KeyCode.Space)) ) {
			dropPod (rocket_direction);
			// Adjust the rockets trajectory if users drops fuel pod too early/late
			changeAngle (0,rocket_direction,turning);
		}*/
	}

	/* This function moves the rocket in an upwards direction at a cubicaly
	 * increasing rate until it reaches its velocity. Once it reaches its
	 * velocity it moves it up at the rate of the velocity.
	*/
	Vector3 launchPhase_TakeOff() {
		gameTime += Time.deltaTime;
		if (Math.Pow (gameTime, 3) < velocity) {
			rocketY += (float) Math.Pow (gameTime, 3);
		} else {
			rocketY += velocity;
		}
		return new Vector3 (rocketX, rocketY, rocketZ);
	}


	/* This function moves and rotates the rocket so that it can smoothly 
	 * transition to follow the trajectory given by the user input 
	*/
	Vector3 launchPhase_TurnToAngle(Vector3 input_angle, Quaternion input_angle_rot) {
		// rotate the rocket
		transform.rotation = Quaternion.Slerp(transform.rotation, input_angle_rot, 0.5f * Time.deltaTime);
		GameObject.Find("HUD").transform.forward = cam.forward;
		// move the position of the rocket
		Vector3 rocket_pointing_vector = transform.up.normalized * velocity;
		rocketX += rocket_pointing_vector.x;
		rocketY += rocket_pointing_vector.y;
		initialX = rocketX;
		initialY = rocketY;
		return new Vector3 (rocketX, rocketY, rocketZ);
	}


	/* This function moves the rocket along a trajectory based on the basic
	 * projectile equation from physics (throwing a ball) where you give
	 * and ititial velocity and the only force is due to gravity
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
	void check_win_lose(float old_y_pos, float new_y_pos) {
		//Check for leaving the atmosphere
		if (new_y_pos > atmosphere_height) {
			Skybox.leavingAtmosphere();
		}
        //This statement handles an edge case where it is out of the atmosphere, but still too low
        if (new_y_pos - old_y_pos <= -10 && new_y_pos > atmosphere_height && new_y_pos < min_height) {
            launch = false;
            Skybox.globalAtmosphereThickness = 0;
            ScreenChanges.staticSpecificScene("Lose_Screen_Low");
        }
        // Check for lose conditions: Too High
        if (new_y_pos > max_height) {
			launch = false;
			ScreenChanges.staticSpecificScene("Lose_Screen_High");
		}
		// Check for lose conditions: Too Low or just right when it starts to turn
		if (new_y_pos - old_y_pos <= -10) {
            if (new_y_pos < atmosphere_height) {                                      // If it's too low, switch contexts to the losing screen
                launch = false;
                ScreenChanges.staticSpecificScene("Lose_Screen_Low");
            }
            else if (new_y_pos < min_height) {
                launch = false;
                ScreenChanges.staticSpecificScene("Lose_Screen_Low");
            }
            else {                                                              // Otherwise it's just right!
                launch = false;
                //Load up the appropriate win screen, based on the user's selected mission
                if (GameState.get_mission() == "Satellite") {
                    ScreenChanges.staticSpecificScene("Win_Screen_Satellite");
                }
                else if (GameState.get_mission() == "Shuttle") {
                    ScreenChanges.staticSpecificScene("Win_Screen_Shuttle");
                }
                else if (GameState.get_mission() == "Mars") {
                    ScreenChanges.staticSpecificScene("Win_Screen_Mars");
                }
            }
		}
	}

    //The following commented out functions were meant to handle dropping the fuel pods and allowing the rocket to adjust its path accordingly.
    //Due to time constraints, such functionality has been removed, but the (incomplete) functions remain if another team wants to try and implement
    //this functionality themselves. Caution: These are very buggy, as they were never finished

    /* This function drops one fuel pod
    */
    /*void dropPod(Vector3 dVector) {
		Transform[] ts = gameObject.GetComponentsInChildren<Transform>();                  // get the components of the rocketship
		foreach (Transform pod in ts) {
			if (pod.name.StartsWith ("tank")) { 				                           // iterate through components and look for fuel tanks
				pod.parent = null;                                                         // remove component from the parent
				//pod.gameObject.AddComponent<Rigidbody>();                                  // add physics engine (rigidbody) to component
				//pod.gameObject.GetComponent<Rigidbody> ().velocity = (dVector) * velocity; // give a velocity away from rocket
				break;                                                                     // only drop 1 fuel pod
			}
		}
	}*/

    /* This function changes the trajectory of the rocketship by adjusting
	 * the angle it follows 
	*/
    /*void changeAngle(float amount, Vector3 dVector, Boolean turning) {
		if (!turning) {                                                         // only change velocity if rocket is turning
			velocity = (dVector.magnitude) / (time - prevTime);
		}
		angleRad = (float) (Math.PI - Math.Cos(dVector.x));                     // sets angleRad to current angle
		initialX = rocketX;    
		initialY = rocketY;
		time = 0;                                                               // resets equation
		angleRad -= amount * ((float)Math.PI) / 180;                            // makes new angle
	}*/

} // End Class: RocketBehavior



