using System;
using System.Drawing;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

/*
List of Relevant Time Events:
- Update Clock (not necessary)
- Update Time Of Day???
- Update Time of Week
- Spend time?



*/


[Title("On Day Time Event")]
[Description("Trigger when a certain day time is achieved")]
[Image(typeof(IconClock), ColorTheme.Type.Yellow)]
[Category("Time Events/On Day Time Event")]
[Serializable]
public class TimeEvent : GameCreator.Runtime.VisualScripting.Event
{
    [SerializeField] private DAYTIME DAYTIME = DAYTIME.Morning;
    protected override void OnUpdate(Trigger trigger)    
    {
        base.OnUpdate(trigger);

        if(TimeManager.Instance.GetDayTime() == DAYTIME)
        {
            _ = trigger.Execute(this.Self);
        }
    }
}
