using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Transform target;
    int maxPoints = 10;

    public int speed;
    public List<Vector3> playerPoints;

    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;

        StartCoroutine(TrackTarget());
    }

    IEnumerator TrackTarget()
    {
        playerPoints.Add(target.position);

        while (Vector3.Distance(transform.position, target.position) > 2f)
        {
            if (Vector3.Distance(target.position, playerPoints[playerPoints.Count-1]) > 3f)
            {
                playerPoints.Add(target.position);
            }

            if (Vector3.Distance(transform.position, playerPoints[0]) < 1f)
            {
                playerPoints.RemoveAt(0);
            }

            if (playerPoints.Count > 0)
            {
                Quaternion lookRot = Quaternion.LookRotation(playerPoints[0] - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, 75 * Time.deltaTime);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
            else
            {
                break;
            }

            yield return null;
        }

        //If we can get to here, then we must be close enough to kill the player
        StartCoroutine(KillPlayer());
    }

    IEnumerator KillPlayer()
    {
        //Freeze the player
        GameManager.instance.CallPause(true, false);

        //Turn the player to look at the enemy
        float timer = 5f;
        while(timer > 0f)
        {
            timer -= 1 * Time.deltaTime;

            Quaternion lookRot = Quaternion.LookRotation(transform.position - target.position);
            target.rotation = Quaternion.RotateTowards(target.rotation, lookRot, (10*timer) * Time.deltaTime);

            yield return null;
        }

        target.GetComponentInChildren<Animator>().SetTrigger("Dead");

        //Potentially make a new post processing override, that slowly fades the vision

        yield return new WaitForSeconds(4f);
        print("You have been killed! Teleporting new clone...");
        GameManager.instance.RestartLevel();
    }
}
