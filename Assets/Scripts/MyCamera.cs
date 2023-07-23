using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    private GameObject player;   //プレイヤー情報格納用
    // private Vector3 offset;      //相対距離取得用
    // private Quaternion offrot;

    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.Find("MyMachine");
    }

    // Update is called once per frame
    void Update()
    {
        //新しいトランスフォームの値を代入する
        Vector3 nowpos = transform.position;
        nowpos.x = this.player.transform.position.x;
        nowpos.y = this.player.transform.position.y;
        transform.position = nowpos;
    }
}
