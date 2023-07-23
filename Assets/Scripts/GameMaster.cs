using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    // public static GameMaster Instance => s_Instance;
    // static GameMaster s_Instance;

    // STEP1: 自車と経過時間
    private GameObject myMachine;
    public float nowTime;

    // STEP2: 周回・セクション管理とゴール・タイムアップ判定
    public int seqNum = 0;          // セクション数
    public int m_State = 0;         // 0..スタート前 1..走行中 2..ゴール
    public string m_maskText = "";
    public float restTime = 200f; 

    // STEP3: ローリングスタート・ゴール後演出
    private float nowCtDown = 2.2f;

    // Start is called before the first frame update
    void Start()
    {
        myMachine = GameObject.Find("MyMachine");

        GameObject[] sections = GameObject.FindGameObjectsWithTag("Section");
        seqNum = sections.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_State == 0) 
        { 
            nowCtDown -= Time.deltaTime;
            if (nowCtDown > 0f) { m_maskText = $"{(int)nowCtDown + 1}"; }
            else { m_State = 1; m_maskText = "Go!"; }
        }
        else if (m_State == 1 && myMachine.GetComponent<MyMachine>().nowLap >= 1)
        {
            nowTime += Time.deltaTime; 
            if (nowTime > 1.0f) { m_maskText = ""; }
        }
    }

    public void GameOver()
    {
        //SceneManager.LoadScene("Result");
        if (m_State == 2) { m_maskText = "Finish"; }
        if (m_State == 3) { m_maskText = "TimeUp Failed"; }
    }
}
