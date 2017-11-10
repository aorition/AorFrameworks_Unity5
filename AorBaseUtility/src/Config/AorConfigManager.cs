using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AorBaseUtility;

namespace AorFramework.core
{
    public class AorConfigManager
    {

        private static AorConfigManager _instance;
        private static bool _init = false;
        private static object _iLock = new object();
        
        public static AorConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_iLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AorConfigManager();
                        }
                    }
                }
                if (!_init)
                {
                    _init = true;
                }
                return _instance;
            }
        }

        private AorConfigManager() { }

        private Dictionary<string, Dictionary<long, Config>> _cfgDic;


    }
}
