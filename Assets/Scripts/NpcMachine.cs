using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NpcMachine : MonoBehaviour
{
    public float vWorld = 5f;      // 実速度
    public float rWorld = 0f;      // 転回速度
    private float vMaximum = 16f;   // 最高速度

    private Queue<GameObject> m_Beacon = new Queue<GameObject>();
    private GameObject nextBeacon;
    
    //private int nowNpc = 10;           // 順位
    private float tBoostTime = 0f;

    //public Vector3 diff;
    private Vector3 rot_s;
    private float angleDiff;
    //public int m_Count;
    //public string m_unk = "";

    // Start is called before the first frame update
    void Start()
    {
        int rank = 0;
        if(int.TryParse(this.name.Replace("NpcMachine",""), out rank)) 
        {  
            tBoostTime = (9 - rank) * 3f;
        }

        // ビーコンを一周分読み込み
        GameObject[] beacons = GameObject.FindGameObjectsWithTag("Beacon");
        foreach(var s in beacons) { m_Beacon.Enqueue(s); }
        nextBeacon = m_Beacon.Dequeue();
        //m_Count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        var gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();

        // ビーコンに向くよう回転
        if(gameMaster.m_State > 0)
        {
            var diff = nextBeacon.transform.position - this.transform.position;
            angleDiff = Vector3.Angle( new Vector3(1,0,0), diff ) * (diff.y < 0f ? -1f : 1f);
            rot_s = this.transform.rotation.eulerAngles;
            var angleRot = (rot_s.z >= 180f ? rot_s.z - 360f : rot_s.z);
            var leftRight = 
                (angleDiff - angleRot < -180f ? 360f + (angleDiff - angleRot) : 
                (angleDiff - angleRot > 180f ? (angleDiff - angleRot) - 360f :
                angleDiff - angleRot));    // HOSEI

            if (leftRight > 0f)
            {
                rWorld = 50f * Time.deltaTime;
            }
            else
            {
                rWorld = -50f * Time.deltaTime;
            }
        }
        transform.Rotate(new Vector3(0f, 0f, rWorld), Space.Self);

        // 最高速度は以下の条件で変動する
        // 1) ビーコンまでの距離に対する、進行方向とビーコンまでの角度の差
        // 2) ブースト
        // 3) MyMachineの現在順位
        if(gameMaster.m_State > 0)
        {
            var diffEmergency = 0.2f; // Math.Abs(leftRight) / diff.magnitude;
            var myMachine = GameObject.Find("MyMachine");
            var vMaxGross =
                vMaximum 
                * ( diffEmergency > 0.5f ? (0.5f / diffEmergency) : 1f )
                * ( tBoostTime > 0 ? 1.5f : 1f )
                * ( 0.98f - (float)(myMachine.GetComponent<MyMachine>().nowNpc - 1) * 0.02f );

            var vAccele = 15f * ( tBoostTime > 0 ? 2f : 1f );

            vWorld += (vAccele * (vMaxGross - vWorld) / vMaxGross) * Time.deltaTime;

            //
            if (gameMaster.m_State >= 2) { this.vMaximum = 5.0f; }  // ゴール後
            tBoostTime -= Time.deltaTime;   // スタート後
        }
        else { vWorld = 5.0f; }

        //
        var vx = this.vWorld * Time.deltaTime * (float)Math.Cos(transform.rotation.eulerAngles.z * Math.PI / 180.0);
        var vy = this.vWorld * Time.deltaTime * (float)Math.Sin(transform.rotation.eulerAngles.z * Math.PI / 180.0);
        var vz = 0f;
        transform.position += new Vector3(vx, vy, vz);
    }

    //当たった時の処理 (is TriggerがON)
    void OnTriggerEnter2D(Collider2D item)
    {
        if(item.name.Contains("Circle"))
        {
            var diff = nextBeacon.transform.position - this.transform.position;
            if(diff.magnitude < 10.0f)
            {
                nextBeacon = m_Beacon.Dequeue();
            }

            if(m_Beacon.Count == 0)
            {
                GameObject[] beacons = GameObject.FindGameObjectsWithTag("Beacon");
                foreach(var s in beacons) { m_Beacon.Enqueue(s); }                
            }
            //m_Count += 1;
        }
        //m_unk = item.name;
    }
}
