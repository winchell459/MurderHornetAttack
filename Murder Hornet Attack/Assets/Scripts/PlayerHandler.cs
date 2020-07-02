using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public static int BeesMurderedCount;
    public static int HornetMurderedCount;

    public void ResetBeesMurderedCount()
    {
        BeesMurderedCount = 0;
    }
    public void ResetHornetMurderedCount()
    {
        HornetMurderedCount = 0;
    }
    public void ResetStats()
    {
        ResetBeesMurderedCount();
        ResetHornetMurderedCount();
    }
}
