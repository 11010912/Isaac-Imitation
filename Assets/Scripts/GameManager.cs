using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Codes_player; // PlayerHead�� �ִ� ���ӽ����̽� ����

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerHead playerhead; // PlayerHead Ŭ���� ���

    void Awake()
    {
        instance = this;
    }
}
