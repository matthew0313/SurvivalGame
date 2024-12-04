using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public abstract class Interaction : MonoBehaviour
{
    [SerializeField] Sprite m_interactImage;
    [SerializeField] string m_interactText;
    public Sprite interactImage => m_interactImage;
    public string interactText => m_interactText;
    public virtual bool canInteract => true;
    public virtual bool removeUponInteract => false;
    Player player;
    new Collider2D collider;

    readonly int selectedID = Animator.StringToHash("Selected");
    protected virtual void Awake()
    {
        collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
    }
    public virtual void OnInteract()
    {

    }
    public virtual void OnSelect()
    {

    }
    public virtual void OnDeselect()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player == null) player = collision.GetComponent<Player>();
            player.AddInteraction(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.RemoveInteraction(this);
        }
    }
}