using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RocketBehavior : MonoBehaviour {

    public ParticleSystem particleSyst = null;
    //public ParticleSystem particleSyst2 = null;
    public Transform cam = null;
	int numFuelPods = 3;
	int fuelPos = 0;
	float moveSpeed = 5.0f;

	public static bool launch = false;

	Vector3 prevPos = new Vector3 (0, 0, 0);
	Vector3 prevPrevPos = new Vector3 (0, 0, 0);
	float rocketX = 0;
	float rocketY = 0;
	float rocketZ = 0;
	float initialX = 0;
	float initialY = 0;

	float velocity = 100;//RocketState.fuel;

	//float angleRad = 60 * ((float)Math.PI) / 180;//RocketState.angle * ((float)Math.PI) / 180;
	//float currAngle =  60 * ((float)Math.PI) / 180;//RocketState.angle * ((float)Math.PI) / 180;
	float angleRad = 0f;
	float currAngle = 0f;
		
	float time = 0f;
	float timeMult = 0f;

    //Win/lose conditions:
    public int min_height = 5900;
    public int max_height = 9900;
    public ScreenChanges screenchanger = new ScreenChanges();

	// Update is called once per frame
	void FixedUpdate () {
		// Listens for lift off key
		if (Input.GetKeyDown(KeyCode.Return) && launch == false) {
			

			//Debug.Log("Lift off!!!");
			launch = true;
			particleSyst.Play();
            ScreenChanges.launch_sounds();
			//particleSyst2.Play ();
			initialX = transform.position.x;
			initialY = transform.position.y;
			rocketX = transform.position.x;
			rocketY = transform.position.y;
			rocketZ = transform.position.z;

			velocity = RocketState.fuel * 5;
			angleRad = (180-RocketState.angle) * ((float)Math.PI) / 180;
			currAngle = (180-RocketState.angle) * ((float)Math.PI) / 180;
			//Debug.Log (RocketState.angle.ToString ());
			//Debug.Log (RocketState.fuel.ToString ());

			//Vector3 yTarget = Camera.main.transform.forward - (transform.forward * Vector3.Dot(Camera.transform.forward, transform.forward));
			// Find the needed rotation to rotate y to y-target
			//Quaternion desiredRotation = Quaternion.LookRotation(transform.forward, yTarget);
			// Apply that rotation
			//transform.rotation = desiredRotation;
		}

		// Handles moving rocket
		if (launch == true) {   
            //Switch the GUI into Launch mode
            GUISwitch.launch_mode();
            // update position of rocket
            float old_y_pos = transform.position.y;
            Vector3 move = trajectory();
			transform.position = move;
            float new_y_pos = transform.position.y;

            rotation();

			// update time
			time += Time.deltaTime * timeMult;
			if (timeMult < 4) {
				timeMult += 0.01f;
			}
			// update position variables
			prevPrevPos = prevPos;
			prevPos = transform.position;

            //Win/Lose Condition handling:
            //If the rocket is going vertically downward...
            print(new_y_pos - old_y_pos);
            if (new_y_pos - old_y_pos <= -10 )
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
			//Vector3 childPos = pod.position;
			//Vector3 breakAwayAngle = new Vector3(childPos.x,0,childPos.z);
			pod.parent = null;
			pod.gameObject.AddComponent<Rigidbody>();
			//pod.gameObject.GetComponent<Rigidbody>().velocity = breakAwayAngle;
			fuelPos += 1;
			changeAngle (3);
		}

	}

	void rotation() {
		/* Rotates the rocket such that it initially starts in the upward
		 * direction. Then it slowly turns towards the user's specified
		 * angle. Finally, it is angled along the tangent line of its path. */
		Vector3 dVector = (transform.position - prevPos).normalized;
		Quaternion vQ = Quaternion.LookRotation (Vector3.forward, dVector);
		Vector3 toVec = vQ.ToEulerAngles();
		if (time < 1) {
			return;
		} else if (Vector3.Distance(transform.eulerAngles, toVec) > 0.01f) {
			transform.rotation = Quaternion.Lerp(transform.rotation, vQ, 0.5f*Time.deltaTime);
            GameObject.Find("HUD").transform.forward = cam.forward;
            //transform.Rotate(0,0,5);
        } else {
			transform.rotation = vQ;
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

	Vector3 trajectory() {
		rocketX = initialX + velocity * ((float)Math.Cos(angleRad)) * time;
		rocketY = (float) (initialY + velocity * Math.Sin(angleRad) * time - 0.5 * 9.8 * time * time);
		return new Vector3(rocketX,rocketY,rocketZ);
	}

	// Use this for initialization
	void Start () {

	}
}
