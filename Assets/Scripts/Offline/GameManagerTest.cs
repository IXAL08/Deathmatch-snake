using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTest : MonoBehaviour
{
    public GameObject Food, SpeedUp;
    public BoxCollider2D GameField;
    public Snake Player;
    void Start()
    {
        Food.transform.position = RandomizePosition();
        SpeedUp.transform.position = RandomizePosition();
    }

    // Update is called once per frame


    public Vector2 RandomizePosition()
    {
        Bounds bounds = GameField.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x), y = Random.Range(bounds.min.y, bounds.max.y);

        Vector2 NuevaPosicion = new Vector3(Mathf.Round(x), Mathf.Round(y), 0);

        return NuevaPosicion;
    }

    public IEnumerator PowerUpSpeed()
    {
        Player.speed = 1.5f;
        SpeedUp.SetActive(false);
        yield return new WaitForSeconds(3f);
        Player.speed = 1f;
        yield return new WaitForSeconds(10f);
        SpeedUp.transform.position = RandomizePosition();
        SpeedUp.SetActive(true);
    }
}
