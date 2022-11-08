using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject settingPanel;

    [Header("UI 온오프")]
    private bool IsUIOn = false;
    public GameObject[] GameUI;
    public Button UIOnOffBtn;
    public Sprite[] OnOffSprs;

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnclickedSetting()
    {
        settingPanel.SetActive(true);
    }

    public void ExitSetting()
    {
        settingPanel.SetActive(false);
    }


    public void UIOnOff()
    {
        if (IsUIOn)
        {
            UIOnOffBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1250, -170);
            UIOnOffBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text = "UI 끄기";
            UIOnOffBtn.gameObject.transform.GetChild(1).GetComponent<Image>().sprite = OnOffSprs[1];

            for (int i = 0; i < GameUI.Length; i++)
            {
                GameUI[i].gameObject.SetActive(true);
            }

            IsUIOn = false;
        }
        else
        {
            UIOnOffBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-148, -170);
            UIOnOffBtn.gameObject.transform.GetChild(0).GetComponent<Text>().text = "UI 켜기";
            UIOnOffBtn.gameObject.transform.GetChild(1).GetComponent<Image>().sprite = OnOffSprs[0];

            for (int i = 0; i < GameUI.Length; i++)
            {
                GameUI[i].gameObject.SetActive(false);
            }

            IsUIOn = true;
        }
    }
}
