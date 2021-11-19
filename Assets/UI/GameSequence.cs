using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameSequence : MonoBehaviour
{
    [SerializeField]
    GameObject cursorSprite;

    [SerializeField]
    private bool tutorialComplete = false;
    [SerializeField]
    private bool isPaused = true;
    [SerializeField] 
    private PlayerLocomotionController player;
    public float playTime = 0.0f;

    private int pickupsToCollectCounter = 0;
    [SerializeField] private float gameDuration = 300.0f;

    [SerializeField] private AudioSource clockTicking;
    [SerializeField] private AudioSource knocking;
    private bool canEndGame = false;
    private bool gameOver = false;

    [SerializeField]
    private int panelIndex = 0;
    public int PanelIndex
    {
        get => panelIndex;
        set
        {
            panelSequence[panelIndex].SetActive(false);
            panelIndex = value; 
            panelSequence[panelIndex].SetActive(true);
        }

    }

    [SerializeField] private List<GameObject> panelSequence;
    [SerializeField] private GameObject toSeeControls;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject panelWin;

    [SerializeField] private TextMeshProUGUI pickupsCollectedText;
    [SerializeField] private TextMeshProUGUI timerText;

    public void Advance()
    {
        if (PanelIndex < panelSequence.Count - 1)
        {
            PanelIndex = PanelIndex + 1;
        }
        else
        {
            background.SetActive(false);
            tutorialComplete = true;
        }
    }

    public void SetPaused(bool pause)
    {
        isPaused = pause;
        if (isPaused)
        {
            PanelIndex = panelSequence.Count - 1;
            background.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            player.enabled = false;

            cursorSprite.SetActive(false);
            Cursor.visible = true;
            clockTicking.Pause();
            toSeeControls.SetActive(false);
        }
        else
        {
            toSeeControls.SetActive(true);
            clockTicking.UnPause();
            background.SetActive(false);
            player.enabled = true;

            cursorSprite.SetActive(true);
            Cursor.visible = false;
        }
    }

    void Awake()
    {
        clockTicking.Pause();
        PanelIndex = 0;
        player.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        var pickups = FindObjectsOfType<PickupItem>();
        foreach (var pickup in pickups)
        {
            if (pickup.gameObject.activeInHierarchy)
            {
                pickup.OnPickupEvent.AddListener(OnPickupAcquiredHandler);
                pickupsToCollectCounter++;
            }
        }
        
        pickupsCollectedText.text = "Limited Edition Super Kawaii Nyaa Nyaa™ School Cat Girl Kohaku Sweet® Figurines to hide:" + pickupsToCollectCounter;
    }

    void Update()
    {
        //if (tutorialComplete)
        {
            if (Input.GetButtonDown("Pause"))
            {
                SetPaused(!isPaused);
            }

            if (!isPaused && !gameOver)
            {
                playTime += Time.deltaTime;
                timerText.text = "Time: " + Mathf.Ceil(playTime);

                float clockVolume = Mathf.Lerp(0.15f, 0.7f, playTime / gameDuration);
                clockTicking.volume = clockVolume;

                if (playTime > gameDuration && !canEndGame)
                {
                    canEndGame = true;
                    knocking.Play();
                }
            }
        }
    }

    void OnPickupAcquiredHandler(PickupItem pickup)
    {
        pickup.OnPickupEvent.RemoveListener(OnPickupAcquiredHandler);
        pickupsToCollectCounter--;
        pickupsCollectedText.text = "Limited Edition Super Kawaii Nyaa Nyaa™ School Cat Girl Kohaku Sweet® Figurines to hide:" + pickupsToCollectCounter;
        if (pickupsToCollectCounter <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        panelWin.GetComponentInChildren<TextMeshProUGUI>().text =
    "Congratulations! You found them all! Good heavens, who knows what would happen if anyone found out about your hobby. Time: " +
    playTime;
        clockTicking.Stop();
        panelWin.SetActive(true);
        gameOver = true;
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
