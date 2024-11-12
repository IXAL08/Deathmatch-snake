using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using FishNet.Demo.AdditiveScenes;
using FishNet.Managing.Logging;
using FishNet;

public class PlayerControllerNetwork : NetworkBehaviour
{
    Vector2 _direction;
    readonly SyncList<Transform> _SnakeSegments = new SyncList<Transform>();
    public float speed = 1.2f;
    public readonly SyncVar<bool> isAlive = new SyncVar<bool>(true);
    public Transform _SegmentsPrefab;
    public GameObject canvas;

    private void Awake()
    {
        _SnakeSegments.OnChange += _SnakeSegments_OnChange;
        isAlive.OnChange += IsAlive_OnChange;
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

        if (isAlive.Value)
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
        else {
            _direction = Vector2.zero;

        }

        
    }

   
    private void FixedUpdate()
    {
        if (!IsOwner)
            return;
        for (int i = _SnakeSegments.Count - 1; i > 0; i--)
        {
            _SnakeSegments[i].position = _SnakeSegments[i - 1].position;
        }

        if (_SnakeSegments.Count == 2)
        {
            first();
        }
        transform.position = new Vector3(Mathf.Round(transform.position.x) + _direction.x, Mathf.Round(transform.position.y) + _direction.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServerStarted) return;

        if (collision.tag == "Food")
        {
            Grow();
        }

        else if (collision.tag == "Speed")
        {
            SpeedUpRPC(Owner);

        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServerStarted) return;
        if (collision.gameObject.tag == "Obstacle")
        {
            Dead();
            print(collision.gameObject.name);
        }
    }


    void Grow()
    {
        Transform segment = Instantiate(_SegmentsPrefab);
        segment.position = _SnakeSegments[_SnakeSegments.Count - 1].position;
        _SnakeSegments.Add(segment);
        ServerManager.Spawn(segment.gameObject, Owner);

    }

    [ServerRpc (RequireOwnership = false)]
    void Dead()
    {
        CambiarBoolRPC();
        StartCoroutine(DeadAnimation());
    }

    private void _SnakeSegments_OnChange(SyncListOperation op, int index, Transform oldItem, Transform newItem, bool asServer)
    {
        switch (op) { 
            case SyncListOperation.Add:
                print("Crecio");
            break;

            case SyncListOperation.Complete:
            break;
            
        }
    }



    [ServerRpc(RequireOwnership = false)]
    void SpeedUpRPC(NetworkConnection conn)
    {
        StartCoroutine(PowerUpSpeed());
    }

    
    public IEnumerator PowerUpSpeed()
    {
        UpdateClientSpeed(1.5f);
        print(speed);
        yield return new WaitForSeconds(3f);
        UpdateClientSpeed(1f);
        print(speed);
    }

    [ObserversRpc]
    private void UpdateClientSpeed(float newSpeed)
    {
        speed = newSpeed; // Actualiza localmente la velocidad del cliente
    }

    [ServerRpc]
    void first()
    {
        BoxCollider2D collider = _SnakeSegments[1].transform.GetComponent<BoxCollider2D>();

        collider.enabled = false;
    }

    public IEnumerator DeadAnimation() {

        speed = 0;
        yield return new WaitForSeconds(2f);
        for (int i = 1; i < _SnakeSegments.Count; i++)
        {
            ServerManager.Despawn(_SnakeSegments[i].gameObject);
            //Destroy(_SnakeSegments[i].gameObject);
        }
        _SnakeSegments.Clear();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        canvas.SetActive(true);
    }

    private void IsAlive_OnChange(bool prev, bool next, bool asServer)
    {
        print("Cambio");
    }

    [ServerRpc(RunLocally = false)]
    void CambiarBoolRPC()
    {
        isAlive.Value = false;
    }
}
