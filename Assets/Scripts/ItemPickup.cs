using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public GameObject S, R, P, H;
    public TextMesh ValueText;
    
    public enum PickupTypes
    {
        Health,
        Power,
        Rapid,
        Storage //firing charge capacity
    }

    public PickupTypes PickupType;

    public float Power;
    public float Duration;

    private void Awake()
    {
        SetupLetters();
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bool destroy = false;
            if(PickupType == PickupTypes.Health)
            {
                destroy = FindObjectOfType<PlayerHandler>().AddHealth(Power);
            }else if(PickupType == PickupTypes.Power)
            {
                destroy = FindObjectOfType<PlayerHandler>().SetPlasmaPowerBuff(Power, Duration);
            }
            else if (PickupType == PickupTypes.Rapid)
            {
                destroy = FindObjectOfType<PlayerHandler>().SetPlasmaChargeRateBuff(Power, Duration);
            }
            else if (PickupType == PickupTypes.Storage)
            {
                destroy = FindObjectOfType<PlayerHandler>().SetMaxShotBuff((int)Power, Duration);
            }
            if(destroy) Destroy(gameObject);
        }
    }

    public void SetupLetters()
    {
        hideLetters();
        if (PickupType == PickupTypes.Health) H.SetActive(true);
        else if (PickupType == PickupTypes.Power) P.SetActive(true);
        else if (PickupType == PickupTypes.Rapid) R.SetActive(true);
        else if (PickupType == PickupTypes.Storage) S.SetActive(true);
        ValueText.text = Utility.Utility.FormatFloat(Power, 2);
    }

    private void hideLetters()
    {
        H.SetActive(false);
        R.SetActive(false);
        P.SetActive(false);
        S.SetActive(false);
    }
}
