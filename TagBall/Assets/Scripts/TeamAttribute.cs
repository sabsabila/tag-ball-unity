using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamAttribute : MonoBehaviour
{
    [SerializeField]
    private GameManager.BEHAVIOR_TYPE teamSide;
    [SerializeField]
    private Button spawnerBtn;
    [SerializeField]
    private TowerAttribute tower;
    [SerializeField]
    private GameObject soldierPrefab;
    [SerializeField]
    private EnergyManager energy;
    [SerializeField]
    private Color32 highlighted;
    [SerializeField]
    private Color32 nonHighlighted;
    [SerializeField]
    private GameObject noticePanel;
    private int memberNumber = 0;
    public Dictionary<string, GameObject> members;

    void Start()
    {
        members = new Dictionary<string, GameObject>();
        spawnerBtn.onClick.AddListener(SpawnSoldier);
    }

    private void SpawnSoldier()
    {
        if (GameManager.Instance.GetMarkerObj(teamSide) != null && GameManager.Instance.AvalaibleMarker(teamSide) && energy.AvalaibleEnergy())
        {
            Vector3 markerPosition = GameManager.Instance.GetMarkerObj(teamSide).transform.position;
            var pawn = Instantiate(soldierPrefab, new Vector3(markerPosition.x, 1f, markerPosition.z), Quaternion.identity);
            pawn.name = "pawn" + memberNumber;
            pawn.GetComponent<SoldierBehavior>().InitSoldier(teamSide, this, markerPosition);
            members.Add(pawn.name, pawn);
            GameManager.Instance.HideMarker(teamSide);
            energy.DecreaseEnergyBar();
            memberNumber++;
        }
        else
        {
            noticePanel.SetActive(true);
            Invoke("DisableNoticePanel", 1.5f);
        }
    }

    private void DisableNoticePanel()
    {
        noticePanel.SetActive(false);
    }

    public Color32 GetHighlightedColor()
    {
        return highlighted;
    }

    public Color32 GetNonHighlightedColor()
    {
        return nonHighlighted;
    }

    public void SetEnergySpeed(float speed)
    {
        energy.energySpeed = speed;
    }

    public void SwitchTeamSide(GameManager.BEHAVIOR_TYPE _teamSide)
    {
        teamSide = _teamSide;
    }

    public void ChangeButtonText(GameManager.BEHAVIOR_TYPE teamSide)
    {
        if (teamSide == GameManager.BEHAVIOR_TYPE.Attacker)
            spawnerBtn.GetComponentInChildren<TMP_Text>().text = "Atatcker";
        else if(teamSide == GameManager.BEHAVIOR_TYPE.Defender)
            spawnerBtn.GetComponentInChildren<TMP_Text>().text = "Defender";
    }

    public GameManager.BEHAVIOR_TYPE GetTeamSide() { return teamSide; }
}
