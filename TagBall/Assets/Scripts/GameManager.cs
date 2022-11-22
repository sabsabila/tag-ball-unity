using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum BEHAVIOR_TYPE { Attacker, Defender, Neutral };
    public enum GAME_STATE { BATTLE, PAUSE };

    [HideInInspector] public GAME_STATE gameState;
    [HideInInspector] public bool markerTeam1;
    [HideInInspector] public bool markerTeam2;
    [HideInInspector] public bool isRushActive;

    [Header("Game Environment")]
    public TowerAttribute[] towers;
    public int numberOfRound = 3;
    [SerializeField]
    private MousePosition mousePosition;
    [SerializeField]
    private int timer = 140;
    [SerializeField]
    private GameObject attackerArea;
    [SerializeField]
    private GameObject defenderArea;
    [SerializeField]
    private GameObject attackerTower;
    [SerializeField]
    private GameObject defenderTower;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private TeamAttribute player1;
    [SerializeField]
    private TeamAttribute player2;
    [SerializeField]
    private SFXManager sfxManager;

    [Header("UI")]
    [SerializeField]
    private TMP_Text timerTxt;
    [SerializeField]
    private TMP_Text player1ScoreTxt;
    [SerializeField]
    private TMP_Text player2ScoreTxt;
    [SerializeField]
    private TMP_Text roundTxt;
    [SerializeField]
    private GameObject roundWinPanel;
    [SerializeField]
    private GameObject panelEnd;
    [SerializeField]
    private TMP_Text textResult;

    private const int TOTAL_ROUND = 5;
    private static int currentRound = 1;
    private static int player1Score = 0, player2Score = 0;
    private static BEHAVIOR_TYPE player1Side, player2Side;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        isRushActive = false;
        markerTeam1 = false;
        markerTeam2 = false;
        StartGame();
    }

    public void StartGame()
    {
        player1ScoreTxt.text = player1Score.ToString();
        player2ScoreTxt.text = player2Score.ToString();
        roundTxt.text = currentRound.ToString();
        SwitchTeamSide();
        var ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        ball.GetComponent<BallBehavior>().InitBall(attackerArea);

        gameState = GAME_STATE.BATTLE;
        StartCoroutine(TickTimer());
    }

    public GameObject GetMarkerObj(BEHAVIOR_TYPE teamSide)
    {
        if (teamSide == BEHAVIOR_TYPE.Attacker)
        {
            if (mousePosition.markerObjTeam1 != null)
            {
                return mousePosition.markerObjTeam1;
            }
        }
        else
        {
            if (mousePosition.markerObjTeam2 != null)
            {
                return mousePosition.markerObjTeam2;
            }
        }
        return null;
    }

    public void HideMarker(BEHAVIOR_TYPE teamSide)
    {
        if (teamSide == BEHAVIOR_TYPE.Attacker)
        {
            if (mousePosition.markerObjTeam1 != null)
            {
                mousePosition.markerObjTeam1.SetActive(false);
            }
        }
        else
        {
            if (mousePosition.markerObjTeam2 != null)
            {
                mousePosition.markerObjTeam2.SetActive(false);
            }
        }
    }

    public bool AvalaibleMarker(BEHAVIOR_TYPE teamSide)
    {
        if (teamSide == BEHAVIOR_TYPE.Attacker)
        {
            return mousePosition.markerObjTeam1.activeInHierarchy;
        }
        else
        {
            return mousePosition.markerObjTeam2.activeInHierarchy;
        }
    }

    IEnumerator TickTimer()
    {
        while (timer > 0)
        {
            if (gameState == GAME_STATE.BATTLE)
            {
                timer--;
                timerTxt.text = timer.ToString();
                if(timer == 15)
                {
                    isRushActive = true;
                }
            }
            yield return new WaitForSeconds(1);
        }
        EndRound(BEHAVIOR_TYPE.Neutral);
    }

    public void Pause()
    {
        if (gameState == GAME_STATE.BATTLE)
        {
            gameState = GAME_STATE.PAUSE;
        }
        else
        {
            gameState = GAME_STATE.BATTLE;
        }
    }

    public void GoTo(string _scene)
    {
        SceneManager.LoadScene(_scene);
    }

    public void EndRound(BEHAVIOR_TYPE winnerTeamSide)
    {
        if (player1.GetTeamSide() == winnerTeamSide)
        {
            player1Score++;
            roundWinPanel.GetComponentInChildren<TMP_Text>().text = "Player 1 Wins !";
            roundWinPanel.GetComponent<Image>().color = player1.GetHighlightedColor();
        }
        else if (player2.GetTeamSide() == winnerTeamSide)
        {
            player2Score++;
            roundWinPanel.GetComponentInChildren<TMP_Text>().text = "Player 2 Wins !";
            roundWinPanel.GetComponent<Image>().color = player2.GetHighlightedColor();
        }
        else
        {
            roundWinPanel.GetComponentInChildren<TMP_Text>().text = "Match Draw !";
        }

        currentRound++;
        Pause();

        if (currentRound > TOTAL_ROUND || player1Score >=3 || player2Score >= 3)
        {
            roundWinPanel.SetActive(true);
            player1ScoreTxt.text = player1Score.ToString();
            player2ScoreTxt.text = player2Score.ToString();
            Invoke("EndMatch", 1.5f);
        }
        else
        {
            roundWinPanel.SetActive(true);
            sfxManager.PlayRoundEndSFX();
            Invoke("ReloadRound", 1.5f);
        }
    }

    private void ReloadRound()
    {
        GoTo("GameScene");
    }

    private void EndMatch()
    {
        roundWinPanel.SetActive(false);
        panelEnd.SetActive(true);
        if (player1Score > player2Score)
        {
            textResult.text = "Player 1 Win";
        }
        else if (player1Score < player2Score)
        {
            textResult.text = "Player 2 Win";
        }
        else if (player1Score == player2Score)
        {
            textResult.text = "Match Draw";
        }
        sfxManager.PlayMatchEndSFX();
    }

    public void ResetMatch()
    {
        currentRound = 1;
        player1Score = 0;
        player2Score = 0;
    }

    private void SwitchTeamSide()
    {
        //set initial value
        if (currentRound <= 1)
        {
            player1Side = player1.GetTeamSide();
            player2Side = player2.GetTeamSide();
        }
        else
        {
            if (player1Side == BEHAVIOR_TYPE.Attacker)
            {
                player1Side = BEHAVIOR_TYPE.Defender;
                player1.SwitchTeamSide(BEHAVIOR_TYPE.Defender);
                defenderTower.transform.GetComponent<Renderer>().material.SetColor("_Color", player1.GetHighlightedColor());
            }
            else if (player1Side == BEHAVIOR_TYPE.Defender)
            {
                player1Side = BEHAVIOR_TYPE.Attacker;
                player1.SwitchTeamSide(BEHAVIOR_TYPE.Attacker);
                attackerTower.transform.GetComponent<Renderer>().material.SetColor("_Color", player1.GetHighlightedColor());
            }

            if (player2Side == BEHAVIOR_TYPE.Attacker)
            {
                player2Side = BEHAVIOR_TYPE.Defender;
                player2.SwitchTeamSide(BEHAVIOR_TYPE.Defender);
                defenderTower.transform.GetComponent<Renderer>().material.SetColor("_Color", player2.GetHighlightedColor());
            }
            else if (player2Side == BEHAVIOR_TYPE.Defender)
            {
                player2Side = BEHAVIOR_TYPE.Attacker;
                player2.SwitchTeamSide(BEHAVIOR_TYPE.Attacker);
                attackerTower.transform.GetComponent<Renderer>().material.SetColor("_Color", player2.GetHighlightedColor());
            }

            player1.ChangeButtonText(player1Side);
            player2.ChangeButtonText(player2Side);
        }
    }
}
