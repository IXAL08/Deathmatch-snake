using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    public BoxCollider2D GridArea;
    bool isActive = true;
    public float Cooldown = 0;

    private void Start()
    {
        transform.position = new Vector3(30, 30, 0);
    }
    private void Update()
    {
        if (isActive)
        {
            Cooldown += Time.deltaTime;
        }

        if (Cooldown >= 25f)
        {
            Cooldown = 0;
            isActive = false;
            RandomizePosition();
        }

    }
    public void RandomizePosition()
    {
        Bounds bounds = GridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x), y = Random.Range(bounds.min.y, bounds.max.y);

        transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0);
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isActive = true;
        }
    }
}
