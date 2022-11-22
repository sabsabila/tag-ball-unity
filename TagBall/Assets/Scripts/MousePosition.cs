using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : MonoBehaviour
{
    [SerializeField]
    private GameObject markerPrefab;

    public GameObject markerObjTeam1;
    public GameObject markerObjTeam2;

    private static string Area1 = "Area1";
    private static string Area2 = "Area2";

    void Start()
    {
        if (markerObjTeam1 != null)
        {
            markerObjTeam1.SetActive(false);
        }
        if (markerObjTeam2 != null)
        {
            markerObjTeam2.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            DetectClickRaycast(ray);
        }

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                Ray ray = Camera.main.ScreenPointToRay(screenPosition);
                DetectClickRaycast(ray);
            }
        }
    }

    private void DetectClickRaycast(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            if (raycastHit.transform.gameObject.tag == Area1)
            {
                GameManager.Instance.markerTeam1 = true;
                SpawnMarker(Area1, raycastHit);
            }

            if (raycastHit.transform.gameObject.tag == Area2)
            {
                GameManager.Instance.markerTeam1 = true;
                SpawnMarker(Area2, raycastHit);
            }

        }
    }

    private void SpawnMarker(string area, RaycastHit raycastHit)
    {
        if (area == Area1)
        {
            if (markerObjTeam1 != null)
            {
                markerObjTeam1.transform.position = raycastHit.point;
                markerObjTeam1.SetActive(true);
            }
            else
            {
                markerObjTeam1 = Instantiate(markerPrefab, raycastHit.point, Quaternion.identity);
            }
        }
        if (area == Area2)
        {
            if (markerObjTeam2 != null)
            {
                markerObjTeam2.transform.position = raycastHit.point;
                markerObjTeam2.SetActive(true);
            }
            else
            {
                markerObjTeam2 = Instantiate(markerPrefab, raycastHit.point, Quaternion.identity);
            }
        }
    }
}
