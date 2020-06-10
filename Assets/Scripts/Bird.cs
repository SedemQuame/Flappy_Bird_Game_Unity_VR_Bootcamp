using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Bird : MonoBehaviour
{
    private Rigidbody2D birdRigidbody2D;
    private const float JUMP_AMOUNT = 80f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        birdRigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }
    }

    private void Jump()
    {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        CMDebug.TextPopupMouse("Dead!");
    }
}
