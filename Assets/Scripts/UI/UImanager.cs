using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UIsystem
{
    public class UImanager : MonoBehaviour
    {

        public GameObject tooltip;
        public Text tooltipText;
        public Button menuButton;
        public Button returnButton;
        public Button restartButton;
        public Button quitButton;
        public GameObject menu;
        public string _text;

        void Start()
        {

            HideTooltip();
            menuButton = menuButton.GetComponent<Button>();
            returnButton = returnButton.GetComponent<Button>();
            restartButton = restartButton.GetComponent<Button>();
            quitButton = quitButton.GetComponent<Button>();
            CloseMenu();
        }

        public void ShowTooltip()
        {
            tooltipText.text = _text;
            tooltip.SetActive(true);
        }
        public void HideTooltip()
        {
            tooltipText.text = "";
            tooltip.SetActive(false);
        }
        public void OpenMenu()
        {
            menu.gameObject.SetActive(true);

            returnButton.onClick.AddListener(CloseMenu);
            menuButton.onClick.AddListener(CloseMenu);
            restartButton.onClick.AddListener(Restart);
            quitButton.onClick.AddListener(Quit);
        }
        public void CloseMenu()
        {
            menu.gameObject.SetActive(false);
            menuButton.onClick.AddListener(OpenMenu);
        }
        public void Restart()
        {
            SceneManager.LoadScene("SampleScene");
        }
        public void Quit()
        {
            Application.Quit();
        }

    }

}