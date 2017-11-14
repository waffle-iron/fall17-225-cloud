﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;
    public float speed;
    private bool isStarted = false;     // test if game has started
                                // variables for swipe input
    public float maxTime;
    public float minSwipeDist;
    float startTime;
    float endTime;
    Vector3 startPos;
    Vector3 endPos;
    float swipeDist;
    float swipeTime;

    public GameController gamecontroller;
    private int count;
    public Text countText;
    public Text highScore;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        gamecontroller = gameControllerObject.GetComponent<GameController>();

        highScore.text = PlayerPrefs.GetInt("HightScore", 0).ToString();
        Debug.Log(PlayerPrefs.GetInt("HightScore", 0));
        Debug.Log("Start!");
    }


    void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (!isStarted)
                {                   // if game hasn't started, player touch screen
                    rb.AddForce(new Vector3(0.0f, 0.0f, 100f) * speed);
                    isStarted = true;
                }
                else
                {                           // if game has started & player touch screen, start measuring to detect swipe
                    startTime = Time.time;
                    startPos = touch.position;
                }
            }

            else if (touch.phase == TouchPhase.Ended)
            {
                endTime = Time.time;
                endPos = touch.position;

                swipeDist = (endPos - startPos).magnitude;
                swipeTime = endTime - startTime;

                if (swipeTime < maxTime && swipeDist > minSwipeDist)
                {
                    Swipe();        // call method to move if player swipes
                }
            }
        }

    }




    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
    }


    void Swipe()
    {
        Vector2 distance = endPos - startPos;
        if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
        {           // check for horizontal swipes
            if (distance.x < 0)
            {
                Move("Left");
            }
            else if (distance.x > 0)
            {
                Move("Right");
            }
        }
        else if (Mathf.Abs(distance.x) < Mathf.Abs(distance.y))
        {
            if (distance.y > 0)
            {
                Move("Up");
            }
            else
            {
                Move("Down");
            }
        }
    }

    void Move(string dir)
    {
        if (dir == "Left")
        {
            rb.AddForce(new Vector3(rb.velocity.magnitude * -1f, 0.0f, 0.0f) * speed);
        }
        else if (dir == "Right")
        {
            rb.AddForce(new Vector3(rb.velocity.magnitude * 1f, 0.0f, 0.0f) * speed);
        }
        else if (dir == "Up")
        {
            rb.AddForce(new Vector3(rb.velocity.magnitude * 0.0f, 5.0f, 0.0f) * speed);
        }
        else if (dir == "Down")
        {
            rb.AddForce(new Vector3(rb.velocity.magnitude * 0.0f, -5.0f, 0.0f) * speed);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("collided ground!");
            gamecontroller.GameOver();
            gamecontroller.DeleteAll();
            Debug.Log("Game Over!");
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Boost"))
        {
            count += 1;
            SetCountText();
        }
        if (count > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", count);
            highScore.text = count.ToString();
        }


    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }

    // collision that ends the game
    void OnParticleCollision(GameObject other)
    {
        // cloud collision
        if (other.gameObject.CompareTag("Cloud"))
        {
            Debug.Log("collided cloud!");
            gamecontroller.GameOver();
            gamecontroller.DeleteAll();
            Debug.Log("Game Over!");
            Destroy(other.gameObject);
            Destroy(gameObject);
            // ground collision
        }
    }
}
