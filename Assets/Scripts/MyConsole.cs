using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyConsole : MonoBehaviour
{
    public Text velocityGUI;
    public Text timeGUI;
    public Text rankGUI;
    public Text maskGUI;
    private GameObject myMachine;
    private GameObject gameMaster;

    // Start is called before the first frame update
    void Start()
    {
        myMachine = GameObject.Find("MyMachine");
        gameMaster = GameObject.Find("GameMaster");
    }

    // Update is called once per frame
    void Update()
    {
        var vWorld = myMachine.GetComponent<MyMachine>().vWorld;
        var vRated = (int)(vWorld * 10 + 0.5);
        velocityGUI.text = $"/10\r\n {vRated} km/h";

        var lap = myMachine.GetComponent<MyMachine>().nowLap;
        var mm = (int)(gameMaster.GetComponent<GameMaster>().nowTime / 60f);
        var ss = (int)(gameMaster.GetComponent<GameMaster>().nowTime) % 60;
        var ff = (int)(gameMaster.GetComponent<GameMaster>().nowTime * 100f) % 100;
        timeGUI.text = $"Lap {lap:0}/3 \r\n{mm:00}'{ss:00}\"{ff:00}";

        var rank = myMachine.GetComponent<MyMachine>().nowNpc;
        rankGUI.text = $"{rank}";

        maskGUI.text = gameMaster.GetComponent<GameMaster>().m_maskText;
    }
}
