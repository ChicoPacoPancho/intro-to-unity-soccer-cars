using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBall : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Goal")
        {
            
            GameManager.Instance.GoalScored();
        }
    }
}
