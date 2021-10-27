using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebProjectile : MonoBehaviour
{
    private Rigidbody2D rb;

    public float StartScale = .01f;
    public float EndScale = .25f;
    public float ScaleSteps = 1000f;
    public float MoveTimer = 1f;
    public float SelfDesctuctTimer = 10f;

    public float DamageDirect = 1f;
    public float DamageIndirect = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.localScale = Vector3.one * StartScale;
        StartCoroutine(StopMoving(MoveTimer));
        StartCoroutine(SelfDestuct(SelfDesctuctTimer));
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x <= EndScale)
        {
            float delta = (EndScale - StartScale) / ScaleSteps + transform.localScale.x;
            transform.localScale = Vector3.one * delta;
        }
    }

    IEnumerator SelfDestuct(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Destroy(gameObject);
    }

    IEnumerator StopMoving(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        rb.velocity = Vector2.zero;
    }

    DamageDPS dps;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            HornetController player = collision.GetComponent<HornetController>();
            if (rb.velocity.magnitude > 0.1)
            {
                // Direct hit
                print("Direct hit");
                player.TakeDamage(DamageDirect);
            }
            else
            {
                print("Indirect hit");
            }

            // Slow debuff
            SpeedBuff speedBuff = collision.gameObject.AddComponent<SpeedBuff>();
            speedBuff.BuffName = "Spider Web";
            speedBuff.Duration = 30f;
            speedBuff.Unique = true;
            speedBuff.SpeedMuliplier = 0.5f;
            speedBuff.BeginBuff();

            // Damage over time
            if (dps == null)
            {
                dps = collision.gameObject.AddComponent<DamageDPS>();
                dps.BuffName = "Spider Web";
                dps.Damage = 1f;
                dps.Delay = 0f;
                dps.ApplyDamageNTimes = 30f;
                dps.ApplyEveryNSeconds = .1f;
                //dps.BeginDPS();
            }
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            dps = null;
        }
    }

}
