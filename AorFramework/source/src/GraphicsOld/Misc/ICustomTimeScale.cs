using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ICustomTimeScale
{
    bool Pause { set; }
    void Update(float timePreFrame);
}
