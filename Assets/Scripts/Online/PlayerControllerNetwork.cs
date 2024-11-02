using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class PlayerControllerNetwork : NetworkBehaviour
{
    Vector2 _direction;
    readonly SyncList<Transform> _SnakeSegments = new SyncList<Transform>();
    float speed = 1;
    public Transform _SegmentsPrefab;


    private void Awake()
    {
        _SnakeSegments.OnChange += _SnakeSegments_OnChange;
        _SnakeSegments.Add(transform);
    }


    public override void OnStartNetwork()
    {
        if (Owner.IsLocalClient)
        {
            name += "(local)";
            GetComponent<Renderer>().material.color = Color.green;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;

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
        for (int i = _SnakeSegments.Count - 1; i > 0; i--)
        {
            _SnakeSegments[i].position = _SnakeSegments[i - 1].position;
        }
        transform.position = new Vector3(Mathf.Round(transform.position.x) + _direction.x, Mathf.Round(transform.position.y) + _direction.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServerStarted) return;

        if (collision.tag == "Food")
        {
            GrowRPC(Owner);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    void Grow()
    {
        Transform segment = Instantiate(_SegmentsPrefab);
        segment.position = _SnakeSegments[_SnakeSegments.Count - 1].position;
        _SnakeSegments.Add(segment);
        ServerManager.Spawn(segment.gameObject);
    }

    private void _SnakeSegments_OnChange(SyncListOperation op, int index, Transform oldItem, Transform newItem, bool asServer)
    {
        switch (op) { 
            case SyncListOperation.Add:
                print("Crecio");
                break;

        }
    }

    [TargetRpc]
    void GrowRPC(NetworkConnection conn)
    {
        Grow();
    }
}
