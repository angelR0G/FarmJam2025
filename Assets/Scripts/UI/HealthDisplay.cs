using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image sanityBar;
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float healthPorcentage = player.GetComponent<HealthComponent>().GetHealthPercentage();
        float sanityPorcentage = player.GetComponent<SanityComponent>().GetSanityPercentage();

        healthBar.fillAmount = healthPorcentage;
        sanityBar.fillAmount = sanityPorcentage;
    }
}
