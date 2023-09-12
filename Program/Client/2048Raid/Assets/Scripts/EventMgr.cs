using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMgr : Singleton<EventMgr>
{
    public int AchieveCount = 128;

    public Action AchieveEvent;
    public Action SpecialEvent;

    public void AchieveCombine(int value)
    {
        if (value <= AchieveCount)
        {
            AchieveCount = value;
            AchieveEvent?.Invoke();
        }

        if (value == 2048)
        {
            SpecialEvent?.Invoke();
        }
    }
}
