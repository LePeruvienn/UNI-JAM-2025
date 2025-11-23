using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class MenuManager : MonoBehaviour
{
    public string playSceneName;

    public Button playButton;
    public Button quitButton;
    public Button creditsButton;

    public GameObject menuTitle;
    public GameObject creditsPanel;

    private ChangeScene changeScene;

    private void Start()
    {
        // Initialize the buttons
        playButton.onClick.AddListener(ShowPlay);
        quitButton.onClick.AddListener(QuitGame);
        creditsButton.onClick.AddListener(ToggleCredits);

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

    private void ShowCredits()
    {
        menuTitle.SetActive(false);
        creditsPanel.SetActive(true);
        playButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        creditsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Go Back";
    }

    private void HideCredits()
    {
        menuTitle.SetActive(true);
        creditsPanel.SetActive(false);
        playButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        creditsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Credits";
    }
}

