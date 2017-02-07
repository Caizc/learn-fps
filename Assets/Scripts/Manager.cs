using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // Manager 单例
    public static Manager Instance;

    // 已加入的玩家列表
    public Dictionary<PlayerController, int> currentPlayerList = new Dictionary<PlayerController, int>();

    // 每个玩家的最大血量
    public int maxHP = 10;
    // 当前玩家 HP
    public int currentHP = 0;

    // 传送门
    public GameObject Portals;
    // 游戏重玩提示
    public GameObject Replay;

    // 玩家自己的控制类
    public PlayerController selfController;
    GameObject currentPortal;

    void Awake()
    {
        // 实现单例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Only One Manager is Allowed");
        }
    }


    /// <summary>
    /// 受到攻击
    /// </summary>
    public void UnderAttack()
    {

        currentHP--;

        // 如果血量小于0，游戏结束
        if (currentHP < 0)
        {
            EndGame();
        }
    }


    /// <summary>
    /// 游戏结束逻辑，删除传送门（怪物）然后显示重玩
    /// </summary>
    void EndGame()
    {
        // 删除传送门
        Destroy(Portals);

        // 非服务器不显示重新开始游戏
        if (selfController.isServer)
        {
            Replay.SetActive(true);
        }
        else
        {
            Replay.SetActive(false);
        }
    }


    /// <summary>
    /// 开始/重新开始游戏
    /// </summary>
    public void ReStartGame()
    {
        //SceneManager.LoadScene("FPS");

        if(null == selfController)
        {
            return;
        }

        // 只有服务器能重新开始游戏
        if (!selfController.isServer)
        {
            return;
        }

        // 根据玩家数设置血量
        currentHP = maxHP * currentPlayerList.Count;
        // 重新创建传送门
        currentPortal = Instantiate(Portals);

        //// 非服务器不显示重新开始游戏
        //if (selfController.isServer)
        //{
        //    Replay.SetActive(true);
        //}
        //else
        //{
        //    Replay.SetActive(false);
        //}

        Replay.SetActive(false);
    }


    /// <summary>
    /// 新玩家加入，更新当前玩家列表
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="isServer"></param>
    public void NewPlayer(PlayerController pc,bool isServer)
    {
        // 存储玩家信息
        currentPlayerList.Add(pc,maxHP);

        if (isServer)
        {
            selfController = pc;
        }

        if(1 == currentPlayerList.Count)
        {
            ReStartGame();
        }

        currentHP += maxHP;
    }


    /// <summary>
    /// 获取自己当前的 transform
    /// </summary>
    /// <returns></returns>
    public Transform GetTargetTransform()
    {
        return selfController.transform;
    }
}
