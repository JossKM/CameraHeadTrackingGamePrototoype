using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameSequence : MonoBehaviour
{
    [SerializeField]
    private bool tutorialComplete = false;
    [SerializeField]
    private bool isPaused = true;
    [SerializeField] 
    private PlayerLocomotionController player;
    public float playTime = 0.0f;
    public int pickupsToCollectCounter = 0;

    [SerializeField] private AudioSource knocking;
    private bool canEndGame = false;

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
        }
        else
        {
            background.SetActive(false);
            player.enabled = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PanelIndex = 0;
        player.enabled = false;

        var pickups = FindObjectsOfType<PickupItem>();
        foreach (var pickup in pickups)
        {
            pickup.OnPickupEvent.AddListener(OnPickupAcquiredHandler);
        }

        pickupsToCollectCounter = pickups.Length;
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

            if (!isPaused)
            {
                playTime += Time.deltaTime;
                timerText.text = "Time: " + Mathf.Ceil(playTime);

                if (playTime > 200.0f && !canEndGame)
                {
                    canEndGame = true;
                    knocking.Play();
                    //EndGame();
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
        panelWin.SetActive(true);
    }
    
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
