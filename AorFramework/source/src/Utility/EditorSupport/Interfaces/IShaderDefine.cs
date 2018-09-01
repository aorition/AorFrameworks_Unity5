using System;
using System.Collections.Generic;

namespace Framework
{
    public interface IShaderDefine
    {
        string ShaderName { get; }
        string ShaderLabel { get; }
        string SavePath { get; }
        string ShdaerCode { get; }
    }
}
