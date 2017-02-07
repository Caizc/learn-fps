using UnityEngine;
using UnityEngine.Networking;

// 设置网络通信频道为0，发送间隔为0.033秒
[NetworkSettings(channel = 0, sendInterval = 0.033f)]
public class PlayerController : NetworkBehaviour
{
    public GameObject Spark;

    Transform leftTransform;
    Transform rightTransform;

    // 左右手柄的坐标和方向信息同步
    [SyncVar]
    public Vector3 synsLeftArm;
    [SyncVar]
    public Vector3 synsLeftQua;
    [SyncVar]
    public Vector3 synsRightArm;
    [SyncVar]
    public Vector3 synsRightQua;

    [SerializeField]
    int lerpRate = 15;

    void Start()
    {
        DontDestroyOnLoad(this);

        SteamVR_ControllerManager VRManager = GetComponent<SteamVR_ControllerManager>();

        GameObject left = VRManager.left;
        leftTransform = left.transform;

        GameObject right = VRManager.right;
        rightTransform = right.transform;

        // 不是本地玩家，则直接将所有控制组件全部删除，只留表现组件
        if (!isLocalPlayer)
        {
            GameObject head = transform.Find("Camera (head)").gameObject;

            Destroy(head);
            Destroy(GetComponent<SteamVR_ControllerManager>());

            // 销毁左手手柄的控制组件
            Destroy(left.GetComponent<SteamVR_TrackedController>());
            Destroy(left.GetComponent<SteamVR_TrackedObject>());

            // 销毁右手手柄的控制组件
            Destroy(right.GetComponent<SteamVR_TrackedController>());
            Destroy(right.GetComponent<SteamVR_TrackedObject>());
        }

        if (isLocalPlayer)
        {
            //通知管理类有新玩家加入
            Manager.Instance.NewPlayer(this, isServer);
        }
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            // 如果是本地玩家，则将自己的手柄数据上传
            CmdProvidePosition(rightTransform.position, rightTransform.forward, leftTransform.position, leftTransform.forward);
        }
        else
        {
            // 如果不是本地玩家，则将主机下发的数据设置给两个手柄
            LerpPosition();
        }
    }

    /// <summary>
    /// 将自己的位置上传给服务器
    /// </summary>
    /// <param name="right"></param>
    /// <param name="queRight"></param>
    /// <param name="left"></param>
    /// <param name="queLeft"></param>
    [Command]
    void CmdProvidePosition(Vector3 right, Vector3 queRight, Vector3 left, Vector3 queLeft)
    {
        synsLeftArm = left;
        synsLeftQua = queLeft;

        synsRightArm = right;
        synsRightQua = queRight;
    }

    /// <summary>
    /// 根据服务器同步的数据改变手柄的位置
    /// </summary>
    void LerpPosition()
    {
        if (null != leftTransform)
        {
            leftTransform.position = Vector3.Lerp(leftTransform.position, synsLeftArm, Time.deltaTime * lerpRate);
            leftTransform.forward = Vector3.Lerp(leftTransform.forward, synsLeftQua, Time.deltaTime * lerpRate);
        }

        if (null != rightTransform)
        {
            rightTransform.position = Vector3.Lerp(rightTransform.position, synsRightArm, Time.deltaTime * lerpRate);
            rightTransform.forward = Vector3.Lerp(rightTransform.forward, synsRightQua, Time.deltaTime * lerpRate);
        }
    }

    /// <summary>
    /// 所有客户端将开枪时间发送到服务器
    /// </summary>
    /// <param name="position"></param>
    /// <param name="forward"></param>
    [Command]
    public void CmdShoot(Vector3 position, Vector3 forward)
    {
        Debug.DrawRay(position, forward * 100, Color.red, 0.02f);

        Ray raycast = new Ray(position, forward);
        RaycastHit hit;

        // 根据 Layer 来判断是否有物体击中
        LayerMask layer = 1 << (LayerMask.NameToLayer("Enermy"));
        bool bHit = Physics.Raycast(raycast, out hit, 10000, layer.value);

        if (bHit)
        {
            Debug.Log(hit.collider.gameObject);

            EnermyController ec = hit.collider.gameObject.GetComponent<EnermyController>();

            if (null != ec)
            {
                ec.UnderAttack();
                RpcCreateSpark(hit.point);
            }
            else
            {
                Manager.Instance.ReStartGame();
            }
        }
    }

    /// <summary>
    /// 主机校验之后将需要产生火花的位置发送给客户端
    /// </summary>
    [ClientRpc]
    void RpcCreateSpark(Vector3 position)
    {
        GameObject go = GameObject.Instantiate(Spark);
        go.transform.position = position;
        Destroy(go, 3);
    }
}
