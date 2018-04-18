using System;
using System.Collections.Generic;

namespace Framework
{
    public interface ICustomTimeScale
    {
        bool Pause { set; }
        void Update(float timePreFrame);
    }
}