﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMove : MonoBehaviour {
    private Rigidbody2D rig;
    private float forceBegin_time;
    private bool force_count = false;//风力开始计时
    public static bool begin_count = false;//风力开始计时 
    private Vector2 wind_direction = Vector2.zero;//风向的单位向量
    private Vector2 add = new Vector2(200.0f, 0);
    private GameObject player;
    float playerDis = 10;
    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").gameObject;

    }

    private void Update()
    {
        if (player.transform.position.x < 308 && player.transform.position.x > 250)
        {
            GetComponent<BoxCollider2D>().enabled = true;
            float playerDis = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y), new Vector2(transform.position.x, transform.position.y));
            if (playerDis < 3)
            {
                player.transform.SetParent(transform);
                PlayerController.PlayerMove = false;
                PlayerController.canControl = false;
            }
            else
            {
                player.transform.SetParent(null);
                PlayerController.PlayerMove = true;
                PlayerController.canControl = true;
            }
        }
        else if (player.transform.position.x > 308 && player.transform.position.x < 320) {
            GetComponent<BoxCollider2D>().enabled = false;
            player.transform.SetParent(null);
            PlayerController.PlayerMove = true;
            PlayerController.canControl = true;
        }

       if (transform.position.x > 310f)
        {
            if (transform.childCount != 0)
            {
                if (transform.GetChild(0).gameObject.name == "Player") ;
                transform.GetChild(0).transform.SetParent(null);
                PlayerController.PlayerMove = true;
            }
        }
        if (GameController.wind_count && GameController.forceOnBoatReady)//风区存在并未加过力时
        {
            rig.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            if (!force_count)
            {
                forceBegin_time = Time.time;
                Debug.Log("forceBegin_time " + forceBegin_time);
                force_count = true;

                wind_direction = (DrawBlueLine.wind_end - DrawBlueLine.wind_start);//风向的单位向量
                rig.AddForce(wind_direction * GameController.windforce + add, ForceMode2D.Force);
                Debug.Log("wind_direction * windforce" + wind_direction * GameController.windforce + add);
            }

            if (Time.time - forceBegin_time >= GameController.forceDuration)
            {
                Debug.Log("力的作用时间到了" + Time.time);
                GameController.forceOnBoatReady = false;//加过力了,不用加了
                //rig.AddForce(-wind_direction * GameController.windforce * wind, ForceMode2D.Force);
                force_count = false;//风力计时结束
            }
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (!GameController.wind_count) return;
        Debug.Log("111");
        if(other.tag=="Wind")
        {
            if(!begin_count)
            {
                Debug.Log("风吹到我了");
                GameController.forceOnBoatReady = true;
                begin_count = true;
            }
           
        }

    }

    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    //if (other.gameObject.name == "BoatDestination")
    //    //{
    //    //    GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;
    //    //    Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>());
    //    //    player.transform.SetParent(null);
    //    //}

    //    if (other.gameObject.tag =="Player")
    //    {
    //        ////撞到的是人,保持静止
    //        //if (transform.position.x < 283f)
    //        //{

    //        //    rig.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    //        //}
    //        //else
    //        //{
    //        //    rig.constraints = RigidbodyConstraints2D.None;
    //        //    rig.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    //        //}
    //        if (transform.childCount==0) {
    //            other.transform.SetParent(gameObject.transform);
    //        }

    //    }
    //}
    //private void OnCollisionExit2D(Collision2D other)
    //{

    //    if (other.gameObject.tag == "Player")
    //    {
    //        if (transform.GetChild(0).gameObject.name == "Player") ;
    //        other.transform.SetParent(null);
    //    }
    //}
    public void reStartBoat() {
        if (transform.childCount != 0) {
            transform.GetChild(0).transform.SetParent(null);
        }
        transform.localPosition = Vector3.zero;
        rig.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }
}
