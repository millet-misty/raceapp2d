using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MyMachine : MonoBehaviour
{
    public float vWorld = 5f;      // 実速度
    public float rWorld = 0f;      // 転回速度
    private float vMaximum = 16f;   // 最高速度
    private GameObject gameMaster;
    private Vector3 rot_s;

    private Queue<Vector3> m_LastPosition;
    private int qSize;
    private int nFlg = 0;

    public int nowLap = 0;
    public int nowSeq = 0;      // 周回内のセクション
    public int nowNpc = 10;     // 順位

    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameObject.Find("GameMaster");

        qSize = (int)(4.0 / Time.deltaTime);
        m_LastPosition = new Queue<Vector3>(qSize);
        for(var i=0; i<qSize; i++) { m_LastPosition.Enqueue(new Vector3()); }
    }

    // Update is called once per frame
    void Update()
    {
        // 左右
        if (gameMaster.GetComponent<GameMaster>().m_State > 0)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rWorld = 50f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                rWorld = -50f * Time.deltaTime;
            }
            else
            { rWorld = 0f; }        
        }

        //
        transform.Rotate(new Vector3(0f, 0f, rWorld), Space.Self);
        rot_s = this.transform.rotation.eulerAngles;

        // 前後
        if (gameMaster.GetComponent<GameMaster>().m_State > 0)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                //transform.position += transform.up * 0.1f;
                vWorld += (15f * (vMaximum - vWorld) / vMaximum) * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                //transform.position -= transform.up * 0.1f;
                vWorld -= 60f * Time.deltaTime;
            }
            else
            { vWorld -= 15f * Time.deltaTime; }
        }
        else { vWorld = 5.0f; }

        if (vWorld < 0f) { vWorld = 0f; }        

        //
        var vx = this.vWorld * Time.deltaTime * (float)Math.Cos(transform.rotation.eulerAngles.z * Math.PI / 180.0);
        var vy = this.vWorld * Time.deltaTime * (float)Math.Sin(transform.rotation.eulerAngles.z * Math.PI / 180.0);
        var vz = 0f;
        transform.position += new Vector3(vx, vy, vz);

        // 衝突時のロールバック
        if(nFlg <= 0)
        {
            m_LastPosition.Dequeue();
            m_LastPosition.Enqueue(transform.position);
        }
        else {
            nFlg -= 1;      // 巻き戻りのフレーム
        }
    }

    //当たった時の処理 (Use Full Kinematic ContactsがON)
    void OnCollisionEnter2D(Collision2D item)
    {
        nFlg = qSize;   // 突き抜け軽減のため、巻き戻り中はキューしない
        transform.position = m_LastPosition.Peek();

        vWorld *= 0.7f;        // 失速ペナルティ
    }

    //当たった時の処理 (is TriggerがON)
    void OnTriggerEnter2D(Collider2D item)
    {
        if(item.name == "Head")     // 順位判定Collider
        {
            int rank = 0;
            if(int.TryParse(item.transform.parent.name.Replace("NpcMachine",""), out rank))
            {
                nowNpc = rank;
            }
        }
        else if(item.name == "Tail")     // 順位判定Collider
        {
            int rank = 0;
            if(int.TryParse(item.transform.parent.name.Replace("NpcMachine",""), out rank))
            {
                nowNpc = rank + 1;
            }
        }
        else if(item.name == "Checker")
        {
            var sid = gameMaster.GetComponent<GameMaster>().seqNum;
            if(nowSeq == sid || nowSeq == 0)
            {
                nowLap += 1;
                nowSeq = 1;
                // セクションタイムの記録・表示は後日対応

                if(nowLap >= 4) {
                    gameMaster.GetComponent<GameMaster>().m_State = 2;     // ゴール
                    this.vMaximum = 5f;
                    gameMaster.GetComponent<GameMaster>().GameOver();
                }
            }
        }
        else if(item.name.Contains("Section"))
        {
            int sid = 0;
            int.TryParse(item.name.Replace("Section",""), out sid);
            if(nowSeq == sid)
            {
                nowSeq += 1;
                // セクションタイムの記録・表示は後日対応
            }
        }
    }
}
