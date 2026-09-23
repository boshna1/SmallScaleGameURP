using UnityEngine;

public static class GlobalValues 
{
    public static CheckpointManage cm;
    public static GameObject player;


    public static void SetPlayer(GameObject value)
    {
        player = value;
    }

    public static void SetCM(CheckpointManage value)
    {
        cm = value;
    }

}
