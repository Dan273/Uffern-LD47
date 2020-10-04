using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[System.Serializable]
public class Task
{
    //We should put the item required for the task here
    public Item item;
    public List<Part> partsToSpawn;
    public Transform room;
    public bool isComplete;
    public GameObject[] doorObjects;

    [HideInInspector]
    public List<Transform> spawnSpots = new List<Transform>();

    public Task(Item it, List<Part> pTS, Transform r, bool isC, GameObject[] dO)
    {
        item = it;
        partsToSpawn = pTS;
        room = r;
        isComplete = isC;
        doorObjects = dO;
    }
}

public class TaskSystem : MonoBehaviour
{
    public static TaskSystem instance;

    public int currentTask;
    
    public List<Task> tasks;

    public Transform lastDoor;
    bool onLastTask;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetTasks();
    }

    void SetTasks()
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            //Add the spawnspots from the room
            for (int t = 0; t < tasks[i].room.childCount; t++)
            {
                tasks[i].spawnSpots.Add(tasks[i].room.GetChild(t));
            }

            //Set the item randomly from the list of items in the database
            int itemNum = Random.Range(0, ItemDatabase.instance.itemList.Length-1);
            Item item = ItemDatabase.instance.itemList[itemNum];
            tasks[i].item = item;

            //Once we have the item for the task, we can set the parts needed, and then add some random ones
            for (int p = 0; p < item.recipe.parts.Length; p++)
            {
                if (item.recipe.parts[p].gameObject != null)
                {
                    tasks[i].partsToSpawn.Add(item.recipe.parts[p]);
                }
            }

            //Spawn the phaser part, which will be in every room
            tasks[i].partsToSpawn.Add(ItemDatabase.instance.GetPart("Phaser Part"));

            //Add a certain number of parts, if the number of parts is less than a certain amount
            while (tasks[i].partsToSpawn.Count < tasks[i].spawnSpots.Count)
            {
                tasks[i].partsToSpawn.Add(ItemDatabase.instance.partList[Random.Range(0, ItemDatabase.instance.partList.Length-1)]);
            }

            SpawnItems(i);
        }
    }

    void SpawnItems(int i)
    {
        for (int p = 0; p < tasks[i].partsToSpawn.Count; p++)
        {
            //Spawn each part at each given spawn point within the task
            if (tasks[i].partsToSpawn[p] != null)
            {
                GameObject newPart = Instantiate(tasks[i].partsToSpawn[p].gameObject, tasks[i].spawnSpots[p].position, Quaternion.identity, tasks[i].spawnSpots[p]);
                newPart.name = tasks[i].partsToSpawn[p].name;
            }
        }
    }

    public void CompleteTask()
    {
        if (currentTask < tasks.Count)
        {
            //When we complete a task, mark it as complete, and move on to the next one
            //We also want to remove the door(s) blocking the next route
            for (int i = 0; i < tasks[currentTask].doorObjects.Length; i++)
            {
                tasks[currentTask].doorObjects[i].SetActive(false);
            }

            //If this is the first task, then we are going to spawn the enemy
            if (currentTask == 0)
            {
                SpawnEnemy();
            }

            tasks[currentTask].isComplete = true;
            currentTask++;
        }
        else
        {
            if (!onLastTask)
            {
                onLastTask = true;
                StartCoroutine(DoLastTask());
            }
        }
    }

    IEnumerator DoLastTask()
    {
        yield return new WaitForSeconds(2);

        float doorHealth = GameManager.instance.wallHealth;
        Transform player = FindObjectOfType<PlayerController>().transform;
        Transform cam = Camera.main.transform;

        player.GetComponentInChildren<Animator>().SetLayerWeight(1, 1);

        while (onLastTask)
        {
            //This is where we can attack the final door, and eventually win
            if (PhaserController.instance.charge > 0)
            {
                //If we left click
                if (Input.GetMouseButton(0))
                {
                    //Fire phaser
                    PhaserController.instance.FirePhaser();

                    //Cast a ray
                    if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 5f))
                    {
                        if (hit.transform.name == "Wall_Final")
                        {
                            doorHealth -= 2f * Time.deltaTime;
                            GameManager.instance.textHealth.text = Mathf.RoundToInt(doorHealth).ToString();
                            GameManager.instance.wallHealth = doorHealth;
                        }
                    }
                }
            }

            if (doorHealth <= 0)
            {
                onLastTask = false;
                break;
            }

            yield return null;
        }

        //If we get to here, then we have broken the door
        Destroy(lastDoor.gameObject);
        GameManager.instance.OnWin();
    }

    void SpawnEnemy()
    {
        Instantiate(Resources.Load("Enemy"), new Vector3(-15, 1, 0), Quaternion.Euler(0, 90, 0));
    }
}
