using System;
using System.Collections.Generic;

namespace AorFramework
{
    public interface ICustomTimeScale
    {
        bool Pause { set; }
        void Update(float timePreFrame);
    }
}