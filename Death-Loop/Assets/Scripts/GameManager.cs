using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isMain;

    public bool isPaused;

    public float wallHealth;
    public Text textHealth;

    public int spawns;
    public Text textSpawns;

    void Awake()
    {
        instance = this;

        if (isMain)
        {
            if (!PlayerPrefs.HasKey("Health"))
            {
                PlayerPrefs.SetFloat("Health", 100f);
            }
            else
            {
                if (PlayerPrefs.GetFloat("Health") <= 0f)
                {
                    PlayerPrefs.SetFloat("Health", 100f);
                }
            }

            wallHealth = PlayerPrefs.GetFloat("Health");

            //Everytime the level gets reset, the health of the wall will increase a little
            wallHealth += 15f;
            if (wallHealth > 100f)
            {
                wallHealth = 100f;
            }

            PlayerPrefs.SetFloat("Health", wallHealth);

            if (!PlayerPrefs.HasKey("Spawns"))
            {
                PlayerPrefs.SetInt("Spawns", 0);
            }

            spawns = PlayerPrefs.GetInt("Spawns");

            spawns++;
            textSpawns.text = spawns.ToString();

            PlayerPrefs.SetInt("Spawns", spawns);

        }
    }

    void Start()
    {
        if(textHealth != null)
            textHealth.text = Mathf.RoundToInt(wallHealth).ToString();

        StartCoroutine(SpawnPlayer());
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            PlayerPrefs.SetFloat("Health", wallHealth);
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator SpawnPlayer()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        PlayerController player = FindObjectOfType<PlayerController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();
        player.enabled = false;
        yield return new WaitForSeconds(5);
        rb.AddForce(player.transform.forward * 250 * Time.deltaTime, ForceMode.Impulse);
        player.enabled = true;
    }

    public void CallPause(bool pause, bool willFreeze)
    {
        if (pause)
        {
            //Everything runs off of the game time, so we can just set the timeScale to 0
            if (willFreeze)
            {
                Time.timeScale = 0;
            }

            isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            isPaused = false;
        }
    }

    public void RestartLevel()
    {
        PlayerPrefs.SetFloat("Health", wallHealth);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("Health", wallHealth);
    }

    public void OnWin()
    {
        Destroy(FindObjectOfType<EnemyController>().gameObject);
        StartCoroutine(WinLoop());
    }

    IEnumerator WinLoop()
    {
        Transform player = FindObjectOfType<PlayerController>().transform;

        while (true)
        {
            if(Vector3.Distance(player.position, transform.position) < 2f)
            {
                break;
            }

            yield return null;
        }

        PlayerPrefs.SetFloat("Health", 100);
        PlayerPrefs.SetInt("Spawns", 0);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
