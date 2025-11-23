using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class MenuManager : MonoBehaviour
{
    public string playSceneName;

    public Button playButton;
    public Button quitButton;
    public Button creditsButton;
    public Button explButton;

    public GameObject menuTitle;
    public GameObject creditsPanel;
    public GameObject explPanel;

    private ChangeScene changeScene;

    private void Start()
    {
        // Initialize the buttons
        playButton.onClick.AddListener(ShowPlay);
        quitButton.onClick.AddListener(QuitGame);
        creditsButton.onClick.AddListener(ToggleCredits);
        explButton.onClick.AddListener(ToggleExpl);
        changeScene = FindObjectOfType<ChangeScene>();

        if (changeScene == null)
            Debug.Log("ERROR : CHANGE SCEEN NOT FOUND");

        // Start with showing Play and Quit, hide credits panel
        HideCredits();
    }

    private void ShowPlay()
    {
        // Logic for starting the game (to be implemented)
        changeScene.Goto(playSceneName);
        Debug.Log("Play Button Pressed");
    }

    private void QuitGame()
    {
        Debug.Log("Quit Button Pressed");
        Application.Quit();
    }

    private void ToggleCredits()
    {
        if (creditsPanel.activeSelf)
        {
            HideCredits();
        }
        else
        {
            ShowCredits();
        }
    }

    private void ToggleExpl()
    {
        if (explPanel.activeSelf)
        {
            HideExpl();
        }
        else
        {
            ShowExpl();
        }
    }

    private void ShowCredits()
    {
        menuTitle.SetActive(false);
        creditsPanel.SetActive(true);
        explButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        creditsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Go Back";
    }

    private void HideCredits()
    {
        menuTitle.SetActive(true);
        creditsPanel.SetActive(false);
        explButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        creditsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Credits";
    }

    private void ShowExpl()
    {
        menuTitle.SetActive(false);
        explPanel.SetActive(true);
        playButton.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        explButton.GetComponentInChildren<TextMeshProUGUI>().text = "Go Back";
    }

    private void HideExpl()
    {
        menuTitle.SetActive(true);
        explPanel.SetActive(false);
        playButton.gameObject.SetActive(true);
        creditsButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        explButton.GetComponentInChildren<TextMeshProUGUI>().text = "Credits";
    }
}

