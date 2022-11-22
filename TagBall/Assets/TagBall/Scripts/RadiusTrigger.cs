using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (transform.GetComponentInParent<SoldierBehavior>().soldierType == GameManager.BEHAVIOR_TYPE.Attacker)
            {
                transform.GetComponentInParent<SoldierBehavior>().AttackerWithBall = true;
                if(!GameManager.Instance.isRushActive)
                    transform.GetComponentInParent<SoldierBehavior>().speed = 0.0075f;
                transform.GetComponentInParent<SoldierBehavior>().GetTheBall(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<SoldierBehavior>())
        {
            if (other.gameObject.GetComponent<SoldierBehavior>().AttackerWithBall)
            {
                transform.GetComponentInParent<SoldierBehavior>().EnableOutline(true);
                transform.GetComponentInParent<SoldierBehavior>().AttackerWithBallObj = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<SoldierBehavior>())
        {
            if (other.gameObject.GetComponent<SoldierBehavior>().AttackerWithBall)
            {
                if (transform.GetComponentInParent<SoldierBehavior>().AttackerWithBallObj != null)
                {
                    transform.GetComponentInParent<SoldierBehavior>().EnableOutline(false);
                    transform.GetComponentInParent<SoldierBehavior>().AttackerWithBallObj = null;
                }
            }
        }
    }
}
