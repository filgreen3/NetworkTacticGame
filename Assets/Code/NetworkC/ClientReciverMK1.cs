using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientReciverMK1 : MonoBehaviour {
    public static int port = 8888;
    public static string server = "127.0.0.1";
    private static NetworkStream mainstream;
    public UnityEngine.UI.InputField inputBox;
    string[] msges;

    public MangerNetwork Manger;
    public ChoosePackUI ChoosePackManger;
    public int waitsecond;

    public void StartReading () {
        StartCoroutine (StartReadConction ());
    }

    public static void Connect (string namePlayer = "player") {
        TcpClient client = new TcpClient ();
        client.Connect (server, port);

        NetworkStream stream = client.GetStream ();
        mainstream = stream;

        WriteMsg ("/Login" + namePlayer);
    }

    void ReadConection () {
        byte[] data = new byte[256];
        if (mainstream != null && mainstream.DataAvailable) {
            data = new byte[256];
            int bytes = mainstream.Read (data, 0, data.Length);
            msges = Encoding.UTF8.GetString (data, 0, bytes).Split ('/');
            foreach (string mesg in msges) {
                Debug.Log (mesg);
                if (mesg.StartsWith ("Set")) {
                    if (mesg.EndsWith ("TeamB"))
                        Manger.TeamName = MangerNetwork.TeamType.TeamB;
                }
                if (mesg.StartsWith ("List"))
                    Manger.GetUnitArray (mesg);
                if (mesg.StartsWith ("Move")) {
                    string[] lines = mesg.Remove (0, 4).Split (':');
                    NetworkElement targetUnit = Manger.ElementsInGame[Convert.ToInt32 (lines[0])];
                    targetUnit.MoveToCord (Convert.ToInt32 (lines[1]), Convert.ToInt32 (lines[2]));
                    if (targetUnit == Manger.CorrectUnit)
                        Manger.EndTurn ();
                }
                if (mesg.StartsWith ("UnitIs")) {
                    string lines = mesg.Remove (0, 7);
                    ChoosePackManger.SignNewUnit( Convert.ToInt32 (lines));
                }

                if (mesg.StartsWith ("Shoot")) {
                    string[] lines = mesg.Remove (0, 5).Split (':');
                    NetworkElement targetUnit = Manger.ElementsInGame[Convert.ToInt32 (lines[0])];
                    targetUnit.ShootToСord (Convert.ToInt32 (lines[1]), Convert.ToInt32 (lines[2]));
                    if (targetUnit == Manger.CorrectUnit)
                        Manger.EndTurn ();
                }
                if (mesg.StartsWith ("VSBoom")) {
                    string[] lines = mesg.Remove (0, 6).Split (':');
                    Manger.VisualManger.VisualGranade (Convert.ToInt32 (lines[0]), Convert.ToInt32 (lines[1]), Convert.ToInt32 (lines[2]));
                }
                if (mesg.StartsWith ("Damage")) {
                    string[] lines = mesg.Remove (0, 6).Split (':');
                    NetworkElement targetUnit = Manger.ElementsInGame[Convert.ToInt32 (lines[0])];
                    targetUnit.unit.GetDamage (Convert.ToInt32 (lines[1]), Convert.ToInt32 (lines[2]), Convert.ToInt32 (lines[3]));
                }
                if (mesg == "EndTurn") {
                    Manger.EndTurn ();
                }

            }
        }
    }
    public static void WriteMsg (string msgtoserv) {
        if (mainstream == null) return;
        byte[] datas = Encoding.UTF8.GetBytes (msgtoserv);
        mainstream.Write (datas, 0, msgtoserv.Length);
    }
    IEnumerator StartReadConction () {
        WaitForSeconds sm = new WaitForSeconds (0.01f);
        while (true) {
            ReadConection ();
            yield return sm;
        }

    }

}