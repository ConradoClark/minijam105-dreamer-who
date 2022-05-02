using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UI;

public enum RoomEvents
{
    OnRoomGenerated
}

public class RoomEventArgs
{
    public string Seed;
}

