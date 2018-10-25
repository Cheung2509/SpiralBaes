using UnityEngine;
using System.Collections;
using Rewired;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{

    Player m_player;
    int currentLevel = 0;
    public Image fader;
    public Image Background;
    public Text label;
    public Sprite[] levels;

    // Use this for initialization
    void Start()
    {
        m_player = ReInput.players.GetPlayer(0);
        StartCoroutine(fade());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAnimating)
        {
            if (m_player.GetButtonDown("Left"))
            {
                NextLevel();
            }
            if (m_player.GetButtonDown("Right"))
            {
                PrevLevel();
            }
            if (m_player.GetButtonDown("Start"))
            {
                if (currentLevel == 0)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Level_" + currentLevel + 1);
                }
            }
        }
    }

    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel > 4)
        {
            currentLevel = 0;
        }
        Background.sprite = levels[currentLevel];
        label.text = "Level " + (currentLevel + 1).ToString();
        StartCoroutine(fade());
    }
    public void PrevLevel()
    {
        currentLevel--;
        if (currentLevel < 0)
        {
            currentLevel = 4;
        }
        Background.sprite = levels[currentLevel];
        label.text = "Level " + (currentLevel + 1).ToString();
        StartCoroutine(fade());
    }

    bool isAnimating = false;

    IEnumerator fade()
    {
        isAnimating = true;
        float t = 0f;
        fader.color = new Color(0f, 0f, 0f, 1f);
        while (t < 0.5f)
        {
            fader.color = new Color(0f, 0f, 0f, 0.5f - t);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        fader.color = new Color(0f, 0f, 0f, 0f);
        isAnimating = false;
    }
}
