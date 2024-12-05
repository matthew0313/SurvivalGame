using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowingCam : MonoBehaviour, ICutsceneTriggerReceiver, ISavable
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

    public void OnCutsceneEnter()
    {
        enabled = false;
    }

    public void OnCutsceneExit()
    {
        enabled = true;
    }

    public void Save(SaveData data)
    {
        data.cameraPos = transform.position;
    }

    public void Load(SaveData data)
    {
        transform.position = data.cameraPos;
        transform.position += new Vector3(0, 0, -10.0f);
    }
}
