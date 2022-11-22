using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource roundEnd;
    [SerializeField]
    private AudioSource matchEnd;

    public void PlayRoundEndSFX()
    {
        roundEnd.Play();
    }

    public void PlayMatchEndSFX()
    {
        matchEnd.Play();
    }
}
