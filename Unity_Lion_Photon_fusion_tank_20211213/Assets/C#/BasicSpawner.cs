using Fusion;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;

//INetworkCallbaks �s�u����I�s����
namespace Eric
{
    public class BasicSpawner : MonoBehaviour,INetworkRunnerCallbacks
    {
        #region ���
        [Header("�ЫػP�[�J�ж����")]
        public InputField inputFieldCreateRoom;
        public InputField inputFieldJoinRoom;
        [Header("���a����� - �s�u�w�m��")]
        public NetworkPrefabRef goPlayer;
        [Header("�e���s�u")]
        public GameObject goCanvas;
        [Header("������r")]
        public Text textVersion;
        public Transform[] traSpawnPoints;

        /// <summary>
        /// ���a��J���ж��W��
        /// </summary>
        private string roomNameInput;
        /// <summary>
        /// �s�u���澹
        /// </summary>
        private NetworkRunner runner;
        private string stringVersion = "Eric Copyright 2021. | Version";
        #endregion

        #region �ƥ�

        void Awake()
        {
            textVersion.text = stringVersion + Application.version;
        }
        
        #endregion

        #region ��k
        /// <summary>
        /// ���s�I���I�s�G�Ыةж�
        /// </summary>
        public void BtnCreateRoom()
        {
            roomNameInput = inputFieldCreateRoom.text;
            print("�Ыةж��G" + roomNameInput);
            StartGame(GameMode.Host);
        }
        /// <summary>
        /// ���s�I���I�s�G�[�J�ж�
        /// </summary>
        public void BtnJoinRoom()
        {
            roomNameInput = inputFieldJoinRoom.text;
            print("�[�J�ж��G" + roomNameInput);
            StartGame(GameMode.Client);
        }
        
        // async �D�P�B�B�z�G����t�ήɳB�z�s�u
        /// <summary>
        /// �}�l�s�u�C��
        /// </summary>
        /// <param name="mode">�s�u�Ҧ��G�D���B�Ȥ�</param>
        private async void StartGame(GameMode mode)
        {
            print("<color=yellow>�}�l�s�u</coloe>");
            runner =gameObject.AddComponent<NetworkRunner>();       //�s�u���澹 = �K�[����<�s�u���澹>                                             
            runner.ProvideInput = true;                             //�s�u���澹.�O�_���ѿ�J = �O;

            //���ݳs�u�G�C���s�u�Ҧ��B�ж��W�١B�s�u������B�����޲z��
            await runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomNameInput,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            print("<color=yellow>�s�u����</coloe>");
            goCanvas.SetActive(false);
        }

        #endregion

        #region Fusion �^�G�禡�ϰ�
        public void OnConnectedToServer(NetworkRunner runner)
        {

        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {

        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {

        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {

        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {

        }
        /// <summary>
        /// ���a�s�u��J�欰
        /// </summary>
        /// <param name="runner">�s�u���澹</param>
        /// <param name="input">��J��T</param>
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            NetworkInputData inputData = new NetworkInputData();                        //�s�W �s�u��J��� ���c
            #region �ۦ��J�ץ�P���ʸ�T
            if (Input.GetKey(KeyCode.W)) inputData.direction += Vector3.forward;        //  W �e
            if (Input.GetKey(KeyCode.S)) inputData.direction += Vector3.back;           //  S ��
            if (Input.GetKey(KeyCode.A)) inputData.direction += Vector3.left;           //  A ��
            if (Input.GetKey(KeyCode.D)) inputData.direction += Vector3.right;          //  D �k

            inputData.inputFire = Input.GetKey(KeyCode.Mouse0);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        /// <summary>
        /// ���a��ƶ��X�G���a�ѦҸ�T�A���a�s�u����
        /// </summary>
        private Dictionary<PlayerRef, NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>();

        /// <summary>
        /// ���a���\�[�J�ж���
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="player"></param>
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            //�H���ͦ��I = Unity ���H���d��(0,�ͦ���m�ƶq)
            int randomSpawnPoint = UnityEngine.Random.Range(0, traSpawnPoints.Length);
            //�s�u���澹.�ͦ�(����,�y��,����,���a��T
            NetworkObject playerNetworkObject = runner.Spawn(goPlayer, traSpawnPoints[randomSpawnPoint].position, Quaternion.identity, player);
            players.Add(player, playerNetworkObject);

        }
        /// <summary>
        /// ���a���}�ж���
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="player"></param>
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            //�p�G ���}�����a�s�u���� �s�b �N�R��
            if(players.TryGetValue(player,out NetworkObject playerNetworkObject))
            {
                runner.Despawn(playerNetworkObject);                //�s�u���澹,�����ͦ�(�Ӫ��a�s�u���󲾰�)
                players.Remove(player);                             //���a���X,����(�Ӫ��a)
            }
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {

        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {

        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {

        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {

        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {

        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {

        }
        #endregion
        #endregion
    }
}

