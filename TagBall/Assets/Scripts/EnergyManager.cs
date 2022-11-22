using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    [SerializeField]
    private Slider[] energySlider;
    private bool fillEnergyRun;
    [SerializeField]
    int energyBar = 0;
    public float energySpeed;

    [SerializeField]
    private Color32 highlighted;
    [SerializeField]
    private Color32 nonHighlighted;

    private bool energyFull;

    private void Start()
    {
        energyFull = false;
        highlighted = transform.GetComponentInParent<TeamAttribute>().GetHighlightedColor();
        nonHighlighted = transform.GetComponentInParent<TeamAttribute>().GetNonHighlightedColor();
        energySpeed = 0.005f;
        energyBar = 0;
        fillEnergyRun = false;
        SetDefaultValue();
        fillEnergyRun = true;
        StartCoroutine(RegenerateEnergy());
    }

    private void SetDefaultValue()
    {
        for (int i = 0; i < energySlider.Length; i++)
        {
            ChangeFillColor(energySlider[i], nonHighlighted);
            energySlider[i].value = 0;
        }
    }

    public IEnumerator RegenerateEnergy()
    {
        while (true)
        {
            if (GameManager.Instance.gameState == GameManager.GAME_STATE.BATTLE)
            {
                if (!fillEnergyRun)
                {
                    break;
                }
                if (GameManager.Instance.isRushActive)
                    energySpeed = 0.01f;
                if (energyBar < energySlider.Length && !energyFull)
                {
                    if (energySlider[energyBar].value != energySlider[energyBar].maxValue)
                    {
                        energySlider[energyBar].value += energySpeed;
                    }
                    else
                    {
                        ChangeFillColor(energySlider[energyBar], highlighted);
                        energyBar++;
                    }
                }
                if (energyBar == energySlider.Length && !energyFull)
                {
                    energyFull = true;
                    energyBar--;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void ChangeFillColor(Slider sliderObj, Color32 _color)
    {
        var fill = (sliderObj as UnityEngine.UI.Slider).GetComponentsInChildren<UnityEngine.UI.Image>().FirstOrDefault(t => t.name == "Fill");
        if (fill != null)
        {
            fill.color = _color;
        }
    }

    public void DecreaseEnergyBar()
    {
        if (energyBar > 0)
        {
            ChangeFillColor(energySlider[energyBar - 2], nonHighlighted);
            ChangeFillColor(energySlider[energyBar - 1], nonHighlighted);
            energySlider[energyBar - 2].value = energySlider[energyBar].value;
            energySlider[energyBar].value = 0;
            energySlider[energyBar - 1].value = 0;
            energyBar -= 2;
            energyFull = false;
        }
    }

    public bool AvalaibleEnergy()
    {
        return (energyBar > 1);
    }
}
