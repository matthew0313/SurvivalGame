using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowingCam : MonoBehaviour
{
    [SerializeField] float lerpRate = 0.5f;
    [SerializeField] BoxCollider2D collider;
    Rigidbody2D rb;
    Player target;
    private void OnValidate()
    {
        if(collider != null)
        {
            collider.size = new Vector2(Camera.main.orthographicSize * 2 * Camera.main.aspect, Camera.main.orthographicSize * 2);
        }
    }
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (collider != null)
        {
            collider.size = new Vector2(Camera.main.orthographicSize * 2 * Camera.main.aspect, Camera.main.orthographicSize * 2);
        }
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        target.onTeleport += Teleport;
    }
    private void Start()
    {
        Teleport(target.transform.position);
    }
    void FixedUpdate()
    {
        if (TimelineCutsceneManager.inCutscene) return;
        rb.MovePosition(transform.position + (target.transform.position - transform.position) * lerpRate * Time.fixedDeltaTime);
    }
    void Teleport(Vector2 position)
    {
        transform.position = position;
        transform.position += new Vector3(0, 0, -10.0f);
    }
}
