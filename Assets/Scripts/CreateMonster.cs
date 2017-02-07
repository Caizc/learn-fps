using UnityEngine;
using UnityEngine.Networking;

public class CreateMonster : MonoBehaviour
{

    // 目标位置
    public Transform TargetPosition;
    // 产生怪物的预制体
    public GameObject Monster;
    // 创建时播放的声音文件
    public AudioClip CreateClip;
    AudioSource AudioSource;

    // 最长刷怪时长
    float MaxColdDownTime = 10;
    // 最短刷怪时长
    float MinColdDownTime = 4;
    float CurrentColdDownTime;

    void Start()
    {
        // 初始化下次刷怪时间
        CurrentColdDownTime = MinColdDownTime;

        // 获取 AudioSource 属性
        AudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 通过 Time.deltaTime 来获取上一帧执行的时间
        CurrentColdDownTime = CurrentColdDownTime - Time.deltaTime;

        // 当冷却时间完成后创建怪物并减少刷怪冷却时间
        if (CurrentColdDownTime <= 0)
        {
            InstantiateMonster();

            CurrentColdDownTime = MaxColdDownTime;
            if (MaxColdDownTime > MinColdDownTime)
            {
                MaxColdDownTime = MaxColdDownTime - 0.5f;
            }
        }
    }

    void InstantiateMonster()
    {
        // 播放怪物吼叫的声音
        AudioSource.PlayOneShot(CreateClip);

        // 根据预制体创建怪物
        GameObject go = Instantiate(Monster);
        // 将创建出来的怪物放置在自己下面，方便管理
        go.transform.parent = this.transform;
        // 通过 Random 类随机将怪物放置在传送门口
        go.transform.position = this.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-2.5f, 2.5f));
        // 设置怪物的攻击目标点
        //go.GetComponent<EnermyController> ().TargetTransform = TargetPosition;

        NetworkServer.Spawn(go);
    }

}
