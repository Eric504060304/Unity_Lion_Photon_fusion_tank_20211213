using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Eric
{
    public class PlayerControl : NetworkBehaviour
    {
        #region 欄位
        [Header("移動速度"), Range(0, 100)]
        public float speed = 7.5f;
        [Header("發射子彈間隔"), Range(0, 1.5f)]
        public float intervalFire = 0.35f;
        [Header("子彈物件")]
        public Bullet bullet;
        [Header("子彈生成位置")]
        public Transform pointFire;
        [Header("砲塔")]
        public Transform traTower;

        /// <summary>
        /// 聊天輸入區域
        /// </summary>
        private InputField inputMessage;
        private Text textAllMessage;
        /// <summary>
        /// 連線角色控制
        /// </summary>
        private NetworkCharacterController ncc;
        #endregion

        #region 事件
        private void Awake()
        {
            ncc = GetComponent<NetworkCharacterController>();
            textAllMessage = GameObject.Find("聊天訊息").GetComponent<Text>();
            inputMessage = GameObject.Find("聊天輸入區域").GetComponent<InputField>();
            inputMessage.onEndEdit.AddListener((string message) => { InputMessage(message); });
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name.Contains("子彈")) Destroy(gameObject);
        }
        #endregion

        #region 屬性
        public TickTimer interval { get; set; }
        #endregion

        #region 方法

        /// <summary>
        /// Fusion 固定更新事件 約等於 Unity Fixed Update
        /// </summary>
        public override void FixedUpdateNetwork()
        {
            Move();
            Fire();
        }
        /// <summary>
        /// 移動
        /// </summary>
        private void Move()
        {
            // 如果 有 輸入資料
            if (GetInput(out NetworkInputData dataInput))
            {
                //連線角色控制器.移動(速度*方向*連線一震時間)
                ncc.Move(speed * dataInput.direction * Runner.DeltaTime);

                //取得滑鼠座標，並將Y指定與砲台一樣的高度避免砲塔歪掉
                Vector3 positionMouse = dataInput.postionMouse;
                positionMouse.y = traTower.position.y;
                //砲塔 的 前方軸向 = 滑鼠 -坦克(向量)
                traTower.forward = positionMouse - transform.position;
            }
        }

        private void Fire()
        {
            if (GetInput(out NetworkInputData dataInput))                                    //如果 玩家有輸入資料
            {
                if (interval.ExpiredOrNotRunning(Runner))                                   //如果 開槍間隔計時器 過期或者沒有在執行
                {
                    if (dataInput.inputFire)                                                //如果 輸入資料是開槍左鍵
                    {
                        interval = TickTimer.CreateFromSeconds(Runner, intervalFire);       //建立計時器

                        Runner.Spawn(                                                       //連線.生成(連線物件，座標，角度，輸入權限，匿名函式(執行器，生成物件)=>{})
                            bullet,
                            pointFire.position,
                            pointFire.rotation,
                            Object.InputAuthority,
                            (runner, objectSpawn) =>
                            {
                                objectSpawn.GetComponent<Bullet>().Init();
                            }
                            );
                    }
                }
            }
        }

        private void InputMessage(string message)
        {
            if (Object.HasInputAuthority)
            {
                RPC_SendMessage(message);
            }
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void RPC_SendMessage(string message, RpcInfo info = default)
        {
            textAllMessage.text += (message + "\n");
        }
        #endregion

    }
}

