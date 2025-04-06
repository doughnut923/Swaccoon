using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Unity.Mathematics;

public class HeavyBoxBehaviour : BoxBehaviour
{

    public override Vector2 getMovement(){

        //Check if the current player is a horse
        Vector2 Update = new Vector2(0, 0);

        Debug.Log("HeavyBoxBehaviour: getMovement() called, Checking if current player is a horse");

        GameObject currentPlayer = PlayerManager.Instance.CurrentCharacter;
        if(currentPlayer.GetComponent<HorseBehaviour>() == null){
            Debug.Log("Fuck i am not a horse!");
            //If the current player is not a horse, we don't want to move the box
            return Update;
        }
        return base.getMovement();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //Everything is the same as the BoxBehaviour script, except for the following:
        // Only the goal is Heavy, so we only need to check for the goal 
        if (collision.gameObject.tag == "Goal")
        {
            GameObject goal = collision.gameObject;

            //if the goal is not heavy, we don't want to do anything
            if(!goal.GetComponent<GoalBehaviour>().isHeavy){
                return;
            }
            // stopping the box at the goal/pressure plate
            box.velocity = Vector2.zero;
            boxCollider.isTrigger = false;          // changing the trigger so that the box wont be moved again
            box.isKinematic = true;

            isOnGoal = true;

            sokobanScript.DecrementGoals();
            sokobanScript.DecrementCrates();

            if (goal.GetComponent<GoalBehaviour>() != null)
            {
                goal.GetComponent<GoalBehaviour>().onGoalReached();
            }

            //Destroy(collision.gameObject);
            //Destroy(gameObject);
            IEnumerator coroutine = MoveSelfToPosition(collision.transform.position);
            StartCoroutine(coroutine);

            CameraManager.Instance.ShakeCamera(0.2f, 0.1f);
            // checks if we won everytime we move box onto goal
            //sokobanScript.Win();
            Debug.Log("stopping moving sound");
            boxSoundSource.Stop();
            
            boxSoundSource.volume = Mathf.Clamp(boxSoundSource.volume - 0.1f, 0f, 1f);
            if (boxSoundSource.volume == 0f)
            {
                boxSoundSource.Stop();
            }
        }

    }
}
