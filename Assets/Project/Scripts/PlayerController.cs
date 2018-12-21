using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rig;
    public float speed;
    public float windforce;
   // private Vector2 previous_v = Vector2.zero;
    private int face;//记录朝向
    private Vector2 wind_direction = Vector2.zero;//风向的单位向量
    /// <summary>
    /// 0：未死亡
    /// 1：摔死
    /// 2：淹死
    /// 3：烧死
    /// </summary>
    public static int DeadType = 0;
    public static bool can_draw_blue;//是否青画笔
    public static bool can_draw_red;//是否用红画笔
    public static bool can_draw_black;//是否用黑色画笔
    public static bool fire_count=false;//火开始生效，开始计时
    public static bool wind_count = false;//风力开始生效，开始计时
    public static float begin_time=0.0f;
    public GameObject Draw_blue;
    public Material NormalMat, CaveMat;

    private float forceBegin_time;
    public float forceDuration = 1.0f;//力的持续时间
    private bool force_count = false;//风力开始计时

    public static bool forceReady = false;//是否加过风力了

  
   // public static bool wind_ready = false;//风生效
    //private bool fire_ready = false;//火生效
   // public static float begin_time=0.0f;
    //public GameObject Draw_blue;
//>>>>>>> Stashed changes
    // Use this for initialization
    private Animator characterAnimator;//人物动画机

    private Vector2 touchPosition;  //触摸点坐标
    private float screenWeight; //屏幕宽度

    public bool isPainting; //是否正在画


    void Start () {
        rig = GetComponent<Rigidbody2D>();     
        face = 1;//开始向右
        forceBegin_time = 0.0f;

        //begin_count = false;
        touchPosition = new Vector2();
        screenWeight = Screen.width;
        characterAnimator = GetComponent<Animator>();
        isPainting = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //设置人物动画机参数 ，动画机共两个bool参数，一个isMoving(是否走路)，一个isPainting(是否绘画)，绘画动画优先。
        if (rig.velocity.x != 0)
        {
            characterAnimator.SetBool("isMoving", true);  //人物速度不为零，设置变量，播放走路动画
        }
        else
        {
            characterAnimator.SetBool("isMoving", false);
        }

        //设置人物动画机参数
        if (isPainting==true)
        {
            characterAnimator.SetBool("isPainting", true);  //正在绘画，设置动画机参数
        }
        else
        {
            characterAnimator.SetBool("isPainting", false);  //不在绘画，设置动画机参数
        }

        if (isPainting == false)//不在绘画方能移动
        {
            //触摸屏幕控制左右移动
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.touches[i];
                    //手指触摸但没有移动/滑动
                    if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                    {
                        touchPosition = touch.position;
                        //对比屏幕坐标进行移动
                        if (touchPosition.x > screenWeight / 2)
                        {
                            rig.velocity = new Vector2(speed, rig.velocity.y);
                        }
                        else if (touchPosition.x < screenWeight / 2)
                        {
                            rig.velocity = new Vector2(-speed, rig.velocity.y);
                        }

                        /*对比人物坐标进行移动
                        if (touchPosition.x > this.transform.position.x + 0.1f)
                        {
                            rig.velocity = new Vector2(speed, rig.velocity.y);
                        }
                        else if (touchPosition.x < this.transform.position.x - 0.1f)
                        {
                            rig.velocity = new Vector2(-speed, rig.velocity.y);
                        }
                        */
                    }
                }
            }
        }
        //Debug.Log(Input.touchCount);

        //人物进山洞的材质改变
        if (transform.position.x > 346f && transform.position.x < 494.31f)
        {
            GetComponent<SpriteRenderer>().material = CaveMat;
        }
        else {
            GetComponent<SpriteRenderer>().material = NormalMat;
        }
    }

    void FixedUpdate()
    {
        if (isPainting == false)
        {
            if (Input.GetKey(KeyCode.A))
            {
                rig.velocity = new Vector2(-speed, rig.velocity.y);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rig.velocity = new Vector2(speed, rig.velocity.y);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {//进入青色画笔
            DrawRedLine DrawBlue = GameObject.Find("Draw_Line_red").GetComponent<DrawRedLine>();
            DrawBlue.DeleteLine();
        }
        //if (Input.GetKeyDown(KeyCode.E))
        //{//进入青色画笔
        //    DrawBlueLine.can_draw_blue = !DrawBlueLine.can_draw_blue;
        //    DrawRedLine.can_draw_red = false;
        //    DrawLine2D.can_draw_black = false;
        //    wind_count = false;

        //}
        //else if (Input.GetKeyDown(KeyCode.T))
        //{//进入红画笔
        //    DrawRedLine.can_draw_red = !DrawRedLine.can_draw_red;
        //    DrawLine2D.can_draw_black = false;
        //    DrawBlueLine.can_draw_blue = false;
        //    fire_count = false;
        //}
        //else if (Input.GetKeyDown(KeyCode.Y))
        //{//进入黑画笔
        //    DrawLine2D.can_draw_black = !DrawLine2D.can_draw_black;
        //    DrawBlueLine.can_draw_blue = false;
        //    DrawRedLine.can_draw_red = false;
        //}
        //else {

        //}
        if (rig.velocity.x < 0)
            this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,180,this.transform.localEulerAngles.z);
        else if (rig.velocity.x > 0)
            this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,0, this.transform.localEulerAngles.z);
    }


    public void startDraw(int InkNum) {
        switch (InkNum) {
            case 0:
                //black
                DrawLine2D.can_draw_black = !DrawLine2D.can_draw_black;
                DrawBlueLine.can_draw_blue = false;
                DrawRedLine.can_draw_red = false;
                break;
            case 1:
                //blue
                DrawBlueLine.can_draw_blue = !DrawBlueLine.can_draw_blue;
                DrawRedLine.can_draw_red = false;
                DrawLine2D.can_draw_black = false;
                wind_count = false;
                break;
            case 2:
                //red
                DrawRedLine.can_draw_red = !DrawRedLine.can_draw_red;
                DrawLine2D.can_draw_black = false;
                DrawBlueLine.can_draw_blue = false;
                fire_count = false;
                break;
            default:
                break;
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("烧到我了，下面的代码跟我没关系");
        if (wind_count&& !forceReady )//风区存在并未加过力时
        {
            if(!force_count)
            {
                
                forceBegin_time = Time.time;
                Debug.Log("forceBegin_time " + forceBegin_time);
                force_count = true;                
            }
            wind_direction = (DrawBlueLine.wind_end - DrawBlueLine.wind_start);//风向的单位向量
            wind_direction = wind_direction.normalized;         
            rig.AddForce(wind_direction * windforce,ForceMode2D.Force);
            Debug.Log("wind_direction * windforce" + wind_direction * windforce);
            if(Time.time-forceBegin_time>=forceDuration)
            {
                Debug.Log("力的作用时间到了"+ Time.time);
                forceReady = true;//加过力了,不用加了
                force_count = false;//风力计时结束
            }
            
        }



    }
    
}