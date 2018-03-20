using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YoukiaUnity.View;

namespace YoukiaUnity
{
    public interface IViewController
    {
        ObjectView View { set; get; }

        void Update();

        void OnEnable();

        void OnDisable();
    }
}
