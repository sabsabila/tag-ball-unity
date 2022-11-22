using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public bool holded;
    public bool GiveToAnother;
    public GameObject Target;
    private float Speed = 0.020f;
    public void InitBall(GameObject plane)
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        Vector3 pos = plane.transform.position + new Vector3(Random.Range(-plane.transform.localScale.x, plane.transform.localScale.x), 0f, Random.Range(-plane.transform.localScale.z, plane.transform.localScale.z));
        transform.position = pos;
    }

    public void FreezeBall()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SoldierBehavior>())
        {
            if (other.GetComponent<SoldierBehavior>().soldierType == GameManager.BEHAVIOR_TYPE.Attacker && !holded)
            {
                holded = true;
                GiveToAnother = false;
                other.GetComponent<SoldierBehavior>().GetTheBall(this.gameObject);
                other.GetComponent<SoldierBehavior>().EnableOutline(true);
            }
        }
    }

    private void Update()
    {
        if (GiveToAnother && Target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, Speed);
        }

        if (GiveToAnother && Target == null)
        {
            GiveToAnother = false;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}
