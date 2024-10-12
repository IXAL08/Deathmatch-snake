using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    Vector2 _direction;
    public float speed = 1, duration = 0;
    List<Transform> _SnakeSegments;
    bool isActive;
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

        if (isActive)
        {
            duration += Time.deltaTime;

            if (duration >= 5f) { 
                speed = 1;
                duration = 0;
                isActive = false;
            }
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
   }

    void Speed(Collider2D collision)
    {
        speed = 1.5f;
        collision.gameObject.transform.position = new Vector3(30, 30, 0);
        isActive = true;
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
            Speed(collision);
        }
    }
}
