using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Bird : MonoBehaviour
{
    private const float JUMP_AMOUNT = 80f;

    private static Bird instance;

    public static Bird GetInstance()
    {
        return instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private Rigidbody2D birdRigidbody2D;

    private State state;
    private enum State
    {
        WaitingToStart,
        Playing,
        Dead
    }
    
    private void Awake()
    {
        instance = this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }


    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    state = State.Playing;
                    Jump();
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }
                break;
            case State.Dead:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }
                break;
        }

    }

    private void Jump()
    {
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }
}
