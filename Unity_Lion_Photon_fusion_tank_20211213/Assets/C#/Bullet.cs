using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �l�u���ʳt�סB�s���ɶ�
/// </summary>
public class Bullet : NetworkBehaviour
{
    #region ���
    [Header("���ʳt��"),Range(0,100)]
    public float speed = 5;
    [Header("�s���ɶ�"),Range(0,10)]
    public float lifeTime = 5;
    #endregion

    #region �ݩ�
    //Networked �s�u���ݩʸ��
    /// <summary>
    /// �s���p�ɾ�
    /// </summary>
    [Networked]
    private TickTimer life { get; set; }
    #endregion

    #region ��k

    public void Init()
    {
        //�s���p�ɾ� = �p�ɾ�.�q��ƫإ�(�s�u���澹, �s���ɶ�)
        life = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }
    /// <summary>
    /// Network Behaviour �����O���Ѫ��ƥ�
    /// �s�u�ΩT�w��s 50 FPS
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        //Runner �s�u�����
        //Expired() �O�_���
        //Despawn() �R��
        //Object �s�u����
        //�p�G �p�ɾ� �L��(���s) �N�R�����s�u����
        //�_�h �N����
        if (life.Expired(Runner)) Runner.Despawn(Object);
        else transform.Translate(0, 0, speed * Runner.DeltaTime);
    }
    #endregion
}
