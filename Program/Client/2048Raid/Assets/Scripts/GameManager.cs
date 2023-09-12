using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Board board;

    private void Awake()
    {
        board.BoardReset();
    }
}
