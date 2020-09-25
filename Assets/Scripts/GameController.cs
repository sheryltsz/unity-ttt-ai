using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text[] buttonList;
    public string playerSide;
    public ttt_ai agent1, agent2;

    public GameObject gameOverPanel;
    public Text gameOverText;

    public int moveCount;
    public GameObject restartButton;

    [Header("General Settings")]
    public bool NiceToWatchMode;
    [HideInInspector]
    public bool PlayerChanged = true;


    public enum WinState
    {
        Draw,
        NoughtWin,
        CrossWin
    }

    void Awake() 
    {
        gameOverPanel.SetActive(false);
        SetGameControllerReferenceOnButtons();
        playerSide = "X";
        moveCount = 0;
        restartButton.SetActive(false);
    }

   void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i < buttonList.Length; i++) 
        {
            buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }

    public string GetPlayerSide()
    {
        return playerSide;
    }

    private void FixedUpdate()
    {
        if (PlayerChanged)
        {
            PlayerChanged = false;

            if (NiceToWatchMode) StartCoroutine(DelayedRequestDecision());
            else GetAgentOfType(playerSide).RequestDecision();
        }
    }

    private IEnumerator DelayedRequestDecision()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GetAgentOfType(playerSide).RequestDecision();
        yield return true;
    }

    public void SelectField(int action, string type)
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            if (action == i)
            {
                buttonList[i].text = type;
            }

        }
    }

    public int ObserveState(string playerType, int i)
    {
            string currentState = buttonList[i].text;
            //Flip state depending on player to align observations
            if (playerType == "O")
            {
                if (currentState == "X") return 2;
                if (currentState == "O") return 1;
            }

            if (playerType == "X")
            {
                if (currentState == "X") return 1;
                if (currentState == "O") return 2;
            }
            return 0;
    }

    public IEnumerable<int> GetOccupiedFields()
    {
        List<int> impossibleFields = new List<int>(9);

        for (int i = 0; i < 9; i++)
        {
            if (buttonList[i].text != "")
                impossibleFields.Add(i);
        }
        return impossibleFields.ToArray();
    }

    private ttt_ai GetAgentOfType(string type)
    {
        if (agent1.type == type) return agent1;
        else return agent2;
    }

    public void AssignRewardsOnWin(WinState winState)
    {
        if (winState == WinState.CrossWin)
        {
            GetAgentOfType("X").SetReward(1f);
            GetAgentOfType("O").SetReward(-1f);
        }
        else if (winState == WinState.NoughtWin)
        {
            GetAgentOfType("O").SetReward(1f);
            GetAgentOfType("X").SetReward(-1f);
        }
        else if (winState == WinState.Draw)
        {
            if (moveCount >= 9)
            {
                GetAgentOfType("O").SetReward(0.75f);
                GetAgentOfType("X").SetReward(-0.25f);
            }
            else
            {
                GetAgentOfType("O").SetReward(-0.25f);
                GetAgentOfType("X").SetReward(0.75f);
            }
        }
    }

    public void EndTime()
    {
        moveCount++;
        //game is small enough to check if win using bruteforce
        if (buttonList[0].text == playerSide && buttonList[1].text == playerSide && buttonList[2].text == playerSide)
        {
            GameOver();
        }
        else if (buttonList[3].text == playerSide && buttonList[4].text == playerSide && buttonList[5].text == playerSide)
        {
            GameOver();
        }
        else if (buttonList[6].text == playerSide && buttonList[7].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver();
        }
        else if (buttonList[0].text == playerSide && buttonList[3].text == playerSide && buttonList[6].text == playerSide)
        {
            GameOver();
        }
        else if (buttonList[1].text == playerSide && buttonList[4].text == playerSide && buttonList[7].text == playerSide)
        {
            GameOver();
        }
        else if (buttonList[2].text == playerSide && buttonList[5].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver();
        }
        else if (buttonList[0].text == playerSide && buttonList[4].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver();
        }
        else if (buttonList[2].text == playerSide && buttonList[4].text == playerSide && buttonList[6].text == playerSide)
        {
            GameOver();
        }
        else
        {
            if (moveCount >= 9) {
                gameOverText.text =" It's a draw!";
                gameOverPanel.SetActive(true);
                restartButton.SetActive(true);
            }
            ChangeSides();            
        }
    }

    void GameOver()
    {
        for(int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = false;        
        }
        agent1.EndEpisode();
        agent2.EndEpisode();
        agent1.gameCount++;
        agent2.gameCount++;
        agent1.SwitchSide();
        agent2.SwitchSide();
        gameOverText.text = playerSide + " wins!";
        gameOverPanel.SetActive(true);
        restartButton.SetActive(true);

    }

    void ChangeSides() 
    {
        if (playerSide == "X")
        {
            playerSide = "O"; 
        }
        else 
        {
            playerSide = "X"; 
        }
        PlayerChanged = true;
    }

    public void RestartGame()
    {
        playerSide = "X";
        moveCount = 0;

        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = true;
            buttonList[i].text = "";
        }
    }
}
