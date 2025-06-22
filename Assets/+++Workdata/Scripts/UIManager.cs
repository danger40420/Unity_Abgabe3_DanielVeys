using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // Static flag to track if we should skip the menu after reload
    public static bool startFromMenu = true;

    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelLost;
    [SerializeField] private GameObject panelWin;
    [SerializeField] private GameObject panelGame; // <- Add this in the Inspector
    [SerializeField] private Button buttonReloadLevel;

    [Header("Collectible UI")]
    [SerializeField] private TextMeshProUGUI textCounterOrbs;
    [SerializeField] private TextMeshProUGUI textCounterWisps;

    private int orbCount = 0;
    private int wispCount = 0;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI textTimerUI;
    [SerializeField] private TextMeshProUGUI textTimerResultUI;

    private float timeElapsed = 0f;
    private bool isTiming = false;

    [Header("Countdown")]
    public TMP_Text countdownText;
    [SerializeField] private CharacterControllerSide playerController;

    private void Start()
    {
        if (startFromMenu)
        {
            panelMenu.SetActive(true);
            panelGame.SetActive(false);
        }
        else
        {
            panelMenu.SetActive(false);
            panelGame.SetActive(true);
            StartCoroutine(StartCountdownRoutine());
        }

        panelLost.SetActive(false);
        panelWin.SetActive(false);
        buttonReloadLevel.onClick.AddListener(ReloadLevel);
    }

    private void Update()
    {
        if (isTiming)
        {
            timeElapsed += Time.deltaTime;
            if (textTimerUI != null)
            {
                textTimerUI.text = FormatTime(timeElapsed);
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartGame()
    {
        panelMenu.SetActive(false);
        panelLost.SetActive(false);
        panelWin.SetActive(false);
        panelGame.SetActive(true);

        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine()
    {
        yield return CountdownToStart(3f);
        playerController.EnableMovement();
    }

    public IEnumerator CountdownToStart(float duration)
    {
        countdownText.gameObject.SetActive(true);

        for (int i = (int)duration; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Run!";
        isTiming = true;
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
    }

    void ReloadLevel()
    {
        startFromMenu = false; // skip menu next time
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowPanelLost()
    {
        isTiming = false;
        if (textTimerResultUI != null)
        {
            textTimerResultUI.text = "Time: " + FormatTime(timeElapsed);
        }
        panelLost.SetActive(true);
    }

    public void ShowPanelWin()
    {
        isTiming = false;
        if (textTimerResultUI != null)
        {
            textTimerResultUI.text = "Time: " + FormatTime(timeElapsed);
        }
        panelWin.SetActive(true);
    }

    public void UpdateOrbText(int newOrbCount)
    {
        textCounterOrbs.text = newOrbCount.ToString();
    }

    public void UpdateWispText(int newWispCount)
    {
        textCounterWisps.text = newWispCount.ToString();
    }
}
