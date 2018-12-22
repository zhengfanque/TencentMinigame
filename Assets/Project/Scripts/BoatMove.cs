﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMove : MonoBehaviour {
    private Rigidbody2D rig;
    private float forceBegin_time;
    private bool force_count = false;//风力开始计时
    public static bool begin_count = false;//风力开始计时 
    private Vector2 wind_direction = Vector2.zero;//风向的单位向量
    private Vector2 drag = new Vector2(-40.0f, 0);
    
    // Use this for initialization
    void Start () {
        rig = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {


        if (GameController.wind_count && GameController.forceOnBoatReady)//风区存在并未加过力时
        {
            rig.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            if (!force_count)
            {
                forceBegin_time = Time.time;
                Debug.Log("forceBegin_time " + forceBegin_time);
                force_count = true;
                
                wind_direction = (DrawBlueLine.wind_end - DrawBlueLine.wind_start);//风向的单位向量
                rig.AddForce(wind_direction * GameController.windforce+drag, ForceMode2D.Force);
                Debug.Log("wind_direction * windforce" + wind_direction * GameController.windforce);
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag =="Player")
        {
            //撞到的是人,保持静止
            Debug.Log("1111");
            rig.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;

        }
    }
   
}
