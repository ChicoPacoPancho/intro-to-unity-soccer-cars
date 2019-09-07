using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class PlayerCar : MonoBehaviour
{

    //store variables to reference components we need to talk to
    private Resettable resettable;
    private Rigidbody rigidbody;
    private CarController carController;

    //store variables related to boosting ( we can change most of these numbaers right in the unity inspector)
    public string boostInputAxis = "Boost";
    private bool canBoost = true;
    public float boostDuration = 5f;
    public float boostCooldown = 10f;
    public float normalSpeed = 100f;
    public float boostSpeed = 300f;
    public float boostJolt = 500f;
    
    //store a list of particle systems that we will activate when we boost
    public ParticleSystem[] boostParticles;

    //At the start of the game...
    void Start()
    {
        //Get the components we need
        resettable = GetComponent<Resettable>();
        rigidbody = GetComponent<Rigidbody>();
        carController = GetComponent<CarController>();
        
        //if our list of particle systems is created...
        if (boostParticles != null)
        { 
            //tell each one...
            foreach (ParticleSystem particleSystem in boostParticles)
            {
                //to stop playing and wait for us to say when to start
                particleSystem.Stop();
            }
        }

        //set our cars max speed to the normal speed we set in the unity inspector
        carController.MaxSpeed = normalSpeed;
    }

    //On every frame (before physics is simulated)...
    void FixedUpdate()
    {
        //get input from the player (To see the list of input axes go to: Edit > Project Settings > Input)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //move the car based on our input
        carController.Move(h, v, v, 0f);
    }

    //On every frame...
    void Update()
    {

        //if we have no boost particles.. stop running this "Update" function
        if (boostParticles == null)
            return;

        //check if player has pressed the button to boost
        if(Input.GetAxis("Boost") > 0) 
        {
            Boost();
        }

        //the rigidbody stores velocity in meters per second. To get KM per hour we need to multiply by 3.6
        float currentSpeed = rigidbody.velocity.magnitude * 3.6f;

        //our threshold to start the particle effects is 2 KM/hr more than our normal speed. 
        float particleEffectSpeed = normalSpeed + 2f;

        //if we are going faster than our normal speed...
        if (currentSpeed > particleEffectSpeed)
        {
            //tell each perticle effect...
            foreach (ParticleSystem particleSystem in boostParticles)
            {
                //if they are stopped, to play.
                if(particleSystem.isStopped)
                    particleSystem.Play();
            }
        }
        else //if we are not going faster than our normal speed...
        {
            //tell each particle effect...
            foreach (ParticleSystem particleSystem in boostParticles)
            {
                //if they are playing, to stop.
                if (particleSystem.isPlaying)
                    particleSystem.Stop();
            }
        }
    }

    //when this function is called, we will boost.
    void Boost()
    {
        //is the boost off of cooldown?
        if(canBoost)
        {

            //add a bit of a jolt of velocity to the car
            rigidbody.AddForce(transform.forward * boostJolt, ForceMode.Acceleration);

            //increase the max speed of the car to our boostSpeed variable we can set in the inspector
            carController.MaxSpeed = boostSpeed;

            //put the boost on cooldown
            canBoost = false;

            //tell the boost to stop later on
            Invoke("StopBoost", boostDuration);

            //tell the boost to come off of cooldown later on
            Invoke("ResetBoost", boostCooldown);
        }

    }

    //when this function is called, we stop boosting
    void StopBoost()
    {
        //reduce the cars max speed back down to the normal value
        carController.MaxSpeed = normalSpeed;
    }

    //when this function is called, the boot will come back off of cooldown
    void ResetBoost()
    {
        //tell therest of the code we can boost again
        canBoost = true;
    }
}
