using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PillapillarEgg : MonoBehaviour
{
    public PillapillarController pillapillarPrefab;
    List<PillapillarController> pillapillars = new List<PillapillarController>();
    public Collider2D[] triggers;
    public Text counterText;
    public int counter = 0;

    public float chunkRadius = 20;
    private bool activeChunk = true;

    // Start is called before the first frame update
    void Start()
    {
        //PillapillarController pillapillar = Instantiate(pillapillarPrefab, transform);
        //PillapillarBornt(pillapillar);
        PillapillarController.SpawnPillipallar(this, transform.position, transform);

        foreach (Collider2D trigger in triggers)
        {
            trigger.enabled = false;
        }
    }

    //private void Update()
    //{
    //    if (LevelHandler.singleton.Player)
    //    {
    //        if(activeChunk && Vector2.Distance(LevelHandler.singleton.Player.transform.position, transform.position) > chunkRadius)
    //        {
    //            activeChunk = false;
    //            SetPillapillarActive(activeChunk);

    //        }else if(!activeChunk && Vector2.Distance(LevelHandler.singleton.Player.transform.position, transform.position) < chunkRadius)
    //        {
    //            activeChunk = true;
    //            SetPillapillarActive(activeChunk);
    //        }
    //    }
    //}

    private void SetPillapillarActive(bool active)
    {
        foreach(PillapillarController pillapillar in pillapillars)
        {
            pillapillar.gameObject.SetActive(active);
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
        if (pillapillars.Count <= 0)
        {
            if(counter < 5)
                EggDead();
            else OpenEgg();
        }
        
    }

    public void PillapillarBornt(PillapillarController pillapillar)
    {
        if (!pillapillars.Contains(pillapillar)) pillapillars.Add(pillapillar);
        pillapillar.egg = this;

        counter++;
        counterText.text = counter.ToString();
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

        counterText.text = "H";
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
