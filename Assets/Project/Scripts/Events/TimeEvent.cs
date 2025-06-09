using System;
using System.Drawing;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

/*
List of Relevant Time Events:
- Update Clock (not necessary)
- Update Time Of Day???
- Update Time of Week
- Spend time?



*/


[Title("On Update Clock")]
[Description("This is an example event")]
[Image(typeof(IconClock), ColorTheme.Type.Yellow)]
[Category("Koshaki/Time Events/On Update Clock")]
[Serializable]
public class TimeEvent : Event
{
    protected override void OnUpdate(Trigger trigger)    
    { 
        (int hours, int minutes) = TimeManager.Instance.GetTime();

        if(false)
        {
            //_ = trigger.Execute(this.Self);
        }
    }
}
