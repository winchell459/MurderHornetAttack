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
            print("1");
        }
        print("2");
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
}
