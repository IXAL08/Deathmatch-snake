using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] GameManagerTest manager;
    Vector2 _direction = Vector2.down;
    public float speed = 1;
    List<Transform> _SnakeSegments;
    public Transform _SegmentsPrefab;

    private void Start()
    {
        _SnakeSegments = new List<Transform>();
        _SnakeSegments.Add(transform);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
        {
            _direction = Vector2.up * speed;
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
        {
            _direction = Vector2.down * speed;
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
        {
            _direction = Vector2.left * speed;
        }
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
        {
            _direction = Vector2.right * speed;
        }

    }

    private void FixedUpdate()
    {
        for (int i = _SnakeSegments.Count - 1 ; i > 0; i--)
        {
            _SnakeSegments[i].position = _SnakeSegments[i - 1].position;
        }

        transform.position = new Vector3(Mathf.Round(transform.position.x) + _direction.x, Mathf.Round(transform.position.y) + _direction.y, 0);
    }

   void Grow()
   {
        Transform segment = Instantiate(_SegmentsPrefab);
        segment.position = _SnakeSegments[_SnakeSegments.Count - 1].position;

        _SnakeSegments.Add(segment);
        manager.Food.transform.position = manager.RandomizePosition();
   }

    void Speed()
    {
        StartCoroutine(manager.PowerUpSpeed());
    }

    void ResetState()
    {
        for (int i = 1; i < _SnakeSegments.Count; i++)
        {
            Destroy(_SnakeSegments[i].gameObject);
        }

        _SnakeSegments.Clear();
        _SnakeSegments.Add(transform);

        transform.position = Vector3.zero;
        speed = 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Food")
        {
            Grow();
        }
        else if(collision.tag == "Obstacle")
        {
            ResetState();
        }
        else if (collision.tag == "Speed")
        {
            Speed();
        }
    }
}
