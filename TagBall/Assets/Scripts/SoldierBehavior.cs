using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject ball;

    [HideInInspector] public GameObject towerTarget;
    [HideInInspector] public GameManager.BEHAVIOR_TYPE soldierType;
    [HideInInspector] public float speed;
    [HideInInspector] public GameObject slotBall;
    [HideInInspector] public bool AttackerWithBall;
    [HideInInspector] public GameObject AttackerWithBallObj;
    private TeamAttribute teamAttribute;
    private Vector3 originalPosition;
    private bool isActive;

    public void InitSoldier(GameManager.BEHAVIOR_TYPE _soldierType, TeamAttribute _teamAttribute, Vector3 _originalPos)
    {
        originalPosition = _originalPos;
        teamAttribute = _teamAttribute;
        soldierType = _soldierType;
        if (soldierType == GameManager.BEHAVIOR_TYPE.Attacker)
        {
            speed = 0.015f;
        }
        else
        {
            speed = 0.01f;
        }
        FindTowerTarget();
        DisableSoldierForAMoment(0.5f);
    }

    public void DisableSoldierForAMoment(float seconds)
    {
        isActive = false;
        transform.GetComponent<Renderer>().material.SetColor("_Color", teamAttribute.GetNonHighlightedColor());
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        StartCoroutine(WaitForActive(seconds));
    }
    IEnumerator WaitForActive(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        transform.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePosition;
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        isActive = true;
        transform.GetComponent<Renderer>().material.SetColor("_Color", teamAttribute.GetHighlightedColor());
    }

    private void FindTowerTarget()
    {
        TowerAttribute[] towers = GameObject.FindObjectsOfType<TowerAttribute>();
        foreach (TowerAttribute tower in towers)
        {
            if (soldierType == GameManager.BEHAVIOR_TYPE.Attacker)
            {
                if (tower.towerType == GameManager.BEHAVIOR_TYPE.Defender)
                {
                    towerTarget = tower.gameObject;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<TowerAttribute>())
        {
            if (collision.gameObject.GetComponent<TowerAttribute>().towerType == GameManager.BEHAVIOR_TYPE.Defender && AttackerWithBall)
            {
                GameManager.Instance.EndRound(GameManager.BEHAVIOR_TYPE.Attacker);
            }
            else if (collision.gameObject.GetComponent<TowerAttribute>().towerType == GameManager.BEHAVIOR_TYPE.Defender && !AttackerWithBall)
            {
                teamAttribute.members.Remove(this.name);
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.GetComponent<SoldierBehavior>())
        {
            if (collision.gameObject.GetComponent<SoldierBehavior>().AttackerWithBall)
            {
                collision.gameObject.GetComponent<SoldierBehavior>().GiveTheBall();
                AttackerWithBallObj = null;
                if(!GameManager.Instance.isRushActive)
                    DisableSoldierForAMoment(4);
                collision.gameObject.GetComponent<SoldierBehavior>().DisableSoldierForAMoment(2.5f);
            }
        }
    }

    void Update()
    {
        if (soldierType == GameManager.BEHAVIOR_TYPE.Attacker && isActive && GameManager.Instance.gameState == GameManager.GAME_STATE.BATTLE)
        {
            if (GameManager.Instance.isRushActive && AttackerWithBall)
                speed = 0.015f;
            float Direction = Mathf.Sign(towerTarget.transform.position.x - transform.position.x);
            Vector3 MovePos = new Vector3(
                transform.position.x + Direction * speed, //MoveTowards on 1 axis
                transform.position.y, transform.position.z
            );
            transform.position = MovePos;
        }
        if (soldierType == GameManager.BEHAVIOR_TYPE.Defender && GameManager.Instance.gameState == GameManager.GAME_STATE.BATTLE)
        {
            if (isActive)
            {
                if (AttackerWithBallObj != null)
                {
                    speed = 0.01f;
                    transform.position = Vector3.MoveTowards(transform.position, AttackerWithBallObj.transform.position, speed);
                }
            }           
            else
            {
                speed = 0.02f;
                float Direction = Mathf.Sign(originalPosition.x - transform.position.x);
                Vector3 MovePos = new Vector3(
                    transform.position.x + Direction * speed, //MoveTowards on 1 axis
                    transform.position.y, transform.position.z + Direction * speed
                );
                transform.position = MovePos;
            }
        }
    }

    public Transform GetClosestFriend(Dictionary<string, GameObject> target, GameObject excludeObj)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (KeyValuePair<string, GameObject> t in target)
        {
            float dist = Vector3.Distance(t.Value.gameObject.transform.position, currentPos);
            if (dist < minDist && t.Key != excludeObj.name)
            {
                tMin = t.Value.gameObject.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

    public void GetTheBall(GameObject _ball)
    {
        ball = _ball;
        ball.gameObject.transform.parent = transform.GetComponentInParent<SoldierBehavior>().transform;
        ball.gameObject.transform.position = transform.GetComponentInParent<SoldierBehavior>().slotBall.transform.position;
        ball.GetComponent<BallBehavior>().FreezeBall();
    }

    public void GiveTheBall()
    {
        if (ball != null)
        {
            speed = 0.015f;
            EnableOutline(false);
            AttackerWithBall = false;
            AttackerWithBallObj = null;
            ball.transform.parent = null;
            ball.GetComponent<BallBehavior>().GiveToAnother = true;
            ball.GetComponent<BallBehavior>().holded = false;
            if (GetClosestFriend(teamAttribute.members, gameObject) != null)
            {
                ball.GetComponent<BallBehavior>().Target = GetClosestFriend(teamAttribute.members, gameObject).gameObject;
            }
            else
            {
                GameManager.Instance.EndRound(GameManager.BEHAVIOR_TYPE.Defender);
            }
        }
    }

    public void EnableOutline(bool isEnabled)
    {
        gameObject.GetComponent<Outline>().enabled = isEnabled;
        gameObject.GetComponent<Outline>().OutlineColor = teamAttribute.GetHighlightedColor();
    }
}
