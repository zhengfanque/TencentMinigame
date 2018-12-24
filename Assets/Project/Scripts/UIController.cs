﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public GameObject WindButton,FireButton,DeadUI, DeadInk,DeadText, HideDeadUIBtn,Helper;
    public Sprite PauseSprite, CloseSprite;
    private Material InkMelt;
    private bool isDeadUIShowing = false;
    private Text currentShowingText;
    public GameObject MiddleInklast;
    GameObject player;

    public Sprite blackInkTexture, RedInkTexture, BlueInkTexture,blackFishTexture,blueFishTexture,redFishTexture;


    /// <summary>
    /// 0:black, 1:blue, 2:red
    /// </summary>
    public static int CurrentBrushColor = 0;

    private Color redColor = new Color(0.74f,0.39f,0.39f), 
                  blueColor = new Color(0.74f, 0.39f, 0.39f), 
                  blackColor = new Color(0,0,0),
                  transColor = new Color(0,0,0,0),
                  whiteColor = new Color(1, 1, 1, 1);
    // Use this for initialization
    void Start () {
        InkMelt = MiddleInklast.GetComponent<Image>().material;
        WindButton.SetActive(false);
        FireButton.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        //print(PlayerController.DeadType);
        InkMelt.SetFloat("_Threshold", 1- GameController.InkDistance / GameController.InkTotalDistance);

        if (PlayerController.DeadType ==1|| PlayerController.DeadType == 2)
        {
            
            if (!isDeadUIShowing)
            {
                DeadInk.GetComponent<Animator>().SetBool("isDead", true);
                currentShowingText = getDeadTextFromPlayerState();
                StartCoroutine(showText(currentShowingText, 10f));
                StartCoroutine(EnDisableRestartButton(true, 0.5f));
                DeadUI.SetActive(true);
            }
            else
            {
                DeadInk.GetComponent<Animator>().SetBool("isDead", false);
                StartCoroutine(EnDisableRestartButton(false, 0.1f));
                player = GameObject.FindGameObjectWithTag("Player").gameObject;
                player.transform.position = GetNewestCheckPoint();
                currentShowingText.color = transColor;
                
            }
            DeadUI.SetActive(true);
        }
        if (PlayerController.DeadType == 3)
        {
            
            if (!isDeadUIShowing)
            {
                DeadInk.GetComponent<Animator>().SetBool("isDead", true);
                currentShowingText = getDeadTextFromPlayerState();
                StartCoroutine(showText(currentShowingText, 10f));
                StartCoroutine(EnDisableRestartButton(true, 0.5f));
            }
            else
            {
                DeadInk.GetComponent<Animator>().SetBool("isDead", false);
                StartCoroutine(EnDisableRestartButton(false, 0.1f));
                currentShowingText.color = transColor;
            }
            DeadUI.SetActive(true);

        }
        print("可以画黑画笔吗" + DrawLine2D.can_draw_black);

        //print("黑" + DrawLine2D.can_draw_black + "蓝" +
        // DrawBlueLine.can_draw_blue + "红" +
        // DrawRedLine.can_draw_red);
    }


    public void InkButton(GameObject ob) {
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //不在作画模式才能继续点
        if (!pcon.isPainting) {
            if (ob.name == "MiddleButton")
            {
                startDraw(CurrentBrushColor);
            }
            else
            {
                //将点击的鱼颜色为中间的颜色
                switch (CurrentBrushColor)
                {
                    case 0:
                        ob.GetComponent<Image>().sprite = blackFishTexture;
                        break;
                    case 1:
                        ob.GetComponent<Image>().sprite = blueFishTexture;
                        break;
                    case 2:
                        ob.GetComponent<Image>().sprite = redFishTexture;
                        break;
                    default:
                        break;
                }
                //将中间的颜色换为被点击的鱼的颜色
                switch (ob.GetComponent<InkButton>().ColorNum)
                {
                    case 0:
                        MiddleInklast.GetComponent<Image>().sprite = blackInkTexture;
                        break;
                    case 1:
                        MiddleInklast.GetComponent<Image>().sprite = BlueInkTexture;
                        break;
                    case 2:
                        MiddleInklast.GetComponent<Image>().sprite = RedInkTexture;
                        break;
                    default:
                        break;
                }

                int temp = ob.GetComponent<InkButton>().ColorNum;

                startDraw(temp);
                ob.GetComponent<InkButton>().ColorNum = CurrentBrushColor;
                CurrentBrushColor = temp;
            }
        }
       
        
    }


    public void ReCycle()
    {
        DrawLine2D DrawBlack = GameObject.Find("Draw_Line_black").GetComponent<DrawLine2D>();
        DrawBlack.ReGainBlackInk();
        DrawBlueLine DrawBlue = GameObject.Find("Draw_Line_blue").GetComponent<DrawBlueLine>();
        DrawBlue.DeleteLine();
        DrawRedLine DrawRed = GameObject.Find("Draw_Line_red").GetComponent<DrawRedLine>();
        DrawRed.DeleteLine();
    }


    //传入画笔编号，开始作画
    private void startDraw(int InkNum) {
        ///在此激活画笔工具
        ///显示确认按钮
        ///禁用人物控制
        PlayerController pcon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        pcon.startDraw(InkNum);
    }

    //根据玩家位置显示当前拥有的画笔
    public void showUIButton(int inkNum) {
        if (inkNum == 1)
        {
            WindButton.SetActive(true);
        }
        else if (inkNum ==2) {
            FireButton.SetActive(true);
        }
        else {
            print("UI show button has wrong input parameter");
        }
    }

    //控制重新开始键
    IEnumerator EnDisableRestartButton(bool isEnable,float waitime) {
        yield return new WaitForSeconds(waitime);
        HideDeadUIBtn.SetActive(isEnable);
    }

    //等待死亡墨水消失后，再隐藏黑屏画布
    IEnumerator DisableDeadUI(bool isEnable, float waitime)
    {
        yield return new WaitForSeconds(waitime);
        HideDeadUIBtn.SetActive(false);
    }

    //取消黑屏，重设各种状态
    public void HideDeadUI() {
        isDeadUIShowing = true;
        ReCycle();
        StartCoroutine(Changestate(1.5f));
    }

    public void ToggleHelp(GameObject go)
    {
        if (Helper.activeInHierarchy) {
            Helper.SetActive(false);
            go.GetComponent<Image>().sprite = PauseSprite;

            //开启相机跟随，画笔和人物控制
        }
        else
        {
            go.GetComponent<Image>().sprite = CloseSprite;

            Helper.SetActive(true);
            //禁用相机跟随，画笔和人物控制
        }
    }

    IEnumerator Changestate(float waitime)
    {
        yield return new WaitForSeconds(waitime);
        isDeadUIShowing = false;
        PlayerController.DeadType = 0;
        DeadUI.SetActive(false);
    }

    private Text getDeadTextFromPlayerState() {
        if (PlayerController.DeadType == 1)
        {
            DeadText.transform.GetChild(0).gameObject.SetActive(true);
            DeadText.transform.GetChild(1).gameObject.SetActive(false);
            DeadText.transform.GetChild(2).gameObject.SetActive(false);
            return DeadText.transform.GetChild(0).gameObject.GetComponent<Text>();
        }
        else if (PlayerController.DeadType == 2)
        {
            DeadText.transform.GetChild(0).gameObject.SetActive(false);
            DeadText.transform.GetChild(1).gameObject.SetActive(true);
            DeadText.transform.GetChild(2).gameObject.SetActive(false);
            return DeadText.transform.GetChild(1).gameObject.GetComponent<Text>();
        }
        else if (PlayerController.DeadType == 3)
        {
            DeadText.transform.GetChild(0).gameObject.SetActive(false);
            DeadText.transform.GetChild(1).gameObject.SetActive(false);
            DeadText.transform.GetChild(2).gameObject.SetActive(true);
            return DeadText.transform.GetChild(2).gameObject.GetComponent<Text>();
        }
        else {
            return null;
        }
    }

    IEnumerator showText(Text txt, float fadeSpeed)
    {

        while (txt.color.a <= 1)
        {
            txt.color = Color.Lerp(txt.color, whiteColor, fadeSpeed * Time.deltaTime);
            break;
        }
        yield return null;
    }


    public Vector3 GetNewestCheckPoint()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        Vector3 pos = Vector3.zero;
        switch (GameController.LevelNum)
        {
            //case 0:
            //    {
            //        if (player.transform.position.x > 30)
            //        {
            //            pos = GameObject.Find("/CheckPoints/CP1-1").transform.position;
            //        }
            //        if (player.transform.position.x > 50)
            //        {
            //            pos = GameObject.Find("/CheckPoints/CP1-2").transform.position;
            //        }
            //        if (player.transform.position.x > 126)
            //        {
            //            pos = GameObject.Find("/CheckPoints/CP1-3").transform.position;
            //        }
            //        if (player.transform.position.x > 190)
            //        {
            //            pos = GameObject.Find("/CheckPoints/CP1-4").transform.position;
            //        }
            //        if (player.transform.position.x > 224)
            //        {
            //            pos = GameObject.Find("/CheckPoints/CP1-5").transform.position;
            //        }
            //        if (player.transform.position.x > 277)
            //        {
            //            GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatMove>().reStartBoat();
            //            pos = GameObject.Find("/CheckPoints/CP1-6").transform.position;
            //        }
            //        if (player.transform.position.x > 322)
            //        {
            //            pos = GameObject.Find("/CheckPoints/CP1-7").transform.position;
            //        }
            //        if (player.transform.position.x > 415)
            //        {
            //            pos = GameObject.Find("/CheckPoints/CP1-8").transform.position;
            //        }
            //        if (player.transform.position.x > 462)
            //        {
            //            pos = GameObject.Find("/CheckPoints/CP1-9").transform.position;
            //        }
            //        break;
            //    }
            case 0:
                {
                    if (GameController.maxPlayerX> 30)
                    {
                        pos = GameObject.Find("/CheckPoints/CP1-1").transform.position;
                    }
                    if (GameController.maxPlayerX > 50)
                    {
                        pos = GameObject.Find("/CheckPoints/CP1-2").transform.position;
                    }
                    if (GameController.maxPlayerX > 126)
                    {
                        pos = GameObject.Find("/CheckPoints/CP1-3").transform.position;
                    }
                    if (GameController.maxPlayerX > 190)
                    {
                        pos = GameObject.Find("/CheckPoints/CP1-4").transform.position;
                    }
                    if (GameController.maxPlayerX > 224)
                    {
                        pos = GameObject.Find("/CheckPoints/CP1-5").transform.position;
                    }
                    if (GameController.maxPlayerX > 277)
                    {
                        GameObject.FindGameObjectWithTag("Boat").GetComponent<BoatMove>().reStartBoat();
                        pos = GameObject.Find("/CheckPoints/CP1-6").transform.position;
                    }
                    if (GameController.maxPlayerX > 322)
                    {
                        pos = GameObject.Find("/CheckPoints/CP1-7").transform.position;
                    }
                    if (GameController.maxPlayerX > 415)
                    {
                        pos = GameObject.Find("/CheckPoints/CP1-8").transform.position;
                    }
                    if (GameController.maxPlayerX > 462)
                    {
                        pos = GameObject.Find("/CheckPoints/CP1-9").transform.position;
                    }
                    break;
                }
            case 1:
                pos = Vector3.zero;
                break;
            case 2:
                pos = Vector3.zero;
                break;
            case 3:
                pos = Vector3.zero;
                break;
            case 4:
                pos = Vector3.zero;
                break;
            default:
                pos = Vector3.zero;
                break;
        }

        return pos;
    }

}
