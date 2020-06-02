using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetController : MonoBehaviour
{
    public float ForwardSpeed = 3;
    public float SideSpeed = 1f;
    public Transform HornetSprite;
    private Rigidbody2D rb;
    public GameObject HornetPlasmPrefab;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, ForwardSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        float mouseWorldXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        Vector2 mousePosition = new Vector2(mouseWorldXPos, transform.position.y);
        
        transform.position = Vector3.MoveTowards(transform.position, mousePosition, SideSpeed * Time.deltaTime);

        if (transform.position.x < mouseWorldXPos) HornetSprite.localScale = new Vector3(-1, 1, 1);
        else if(transform.position.x > mouseWorldXPos) HornetSprite.localScale = new Vector3(1, 1, 1);

        if (Input.GetMouseButtonDown(0))
        {
            GameObject plasm = Instantiate(HornetPlasmPrefab, transform.position, Quaternion.identity);
            plasm.GetComponent<Rigidbody2D>().velocity = 40 * (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Honeycomb"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
