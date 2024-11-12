using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using UnityEngine.UIElements;

public class FoodRPC : NetworkBehaviour
{
    public BoxCollider2D GameField;
    public bool PowerUp;

    public override void OnStartNetwork()
    {
        RandomizePosition();
    }

    [Server]
    public void RandomizePosition()
    {
        Bounds bounds = GameField.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x), y = Random.Range(bounds.min.y, bounds.max.y);

        transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0);
    }


    public IEnumerator SpawnPowerUp()
    {
        transform.position = new Vector2(32, -10);
        yield return new WaitForSeconds(8f);
        RandomizePosition();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServerStarted) return;

        if (collision.tag == "Player")
        {
            RandomizePosition();
        }

        if(collision.tag == "Player" && PowerUp == true)
        {
            StartCoroutine(SpawnPowerUp());
        }
    }
}
