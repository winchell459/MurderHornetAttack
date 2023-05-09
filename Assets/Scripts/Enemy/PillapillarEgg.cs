using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillapillarEgg : MonoBehaviour
{
    public PillapillarController pillapillarPrefab;
    List<PillapillarController> pillapillars = new List<PillapillarController>();
    public Collider2D[] triggers;

    // Start is called before the first frame update
    void Start()
    {
        PillapillarController pillapillar = Instantiate(pillapillarPrefab, transform);
        PillapillarBornt(pillapillar);

        foreach (Collider2D trigger in triggers)
        {
            trigger.enabled = false;
        }
    }

    private void PillapillarDied(PillapillarController pillapillar)
    {
        if (pillapillars.Contains(pillapillar)) pillapillars.Remove(pillapillar);
    }

    public void PillapillarKilled(PillapillarController pillapillar)
    {
        PillapillarDied(pillapillar);
        if (pillapillars.Count <= 0) OpenEgg();
    }
    public void PillapillarSuicide(PillapillarController pillapillar)
    {
        PillapillarDied(pillapillar);
        if (pillapillars.Count <= 0) EggDead();
    }

    public void PillapillarBornt(PillapillarController pillapillar)
    {
        if (!pillapillars.Contains(pillapillar)) pillapillars.Add(pillapillar);
        pillapillar.egg = this;
    }

    private void EggDead()
    {
        Destroy(gameObject);
    }

    private void OpenEgg()
    {
        foreach(Collider2D trigger in triggers)
        {
            trigger.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindObjectOfType<LevelHandler>().CollectionsUpdated(1, 0, 0);
            Destroy(gameObject);
        }
    }
}
