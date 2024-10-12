using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public BoxCollider2D GridArea;

    private void Start()
    {
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        Bounds bounds = GridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x) , y = Random.Range(bounds.min.y,bounds.max.y);

        transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y),0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            RandomizePosition();
        }
    }
}
