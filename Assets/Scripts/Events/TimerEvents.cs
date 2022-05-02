using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum TimerEvents
{
    OnTimerChanged, 
    OnTimerUp
}

public class TimerChangedEventArgs
{
    public float AmountInSeconds;
}

