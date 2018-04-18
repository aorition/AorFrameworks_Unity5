using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class Illustration : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {

        Vector3[] plist = {
            new Vector3(0, 0),
            new Vector3(100, 0),
            new Vector3(100, 100),
            new Vector3(0, 100)
        };

        //防止 顶点流 合并错误
        vh.Clear();

        //IllustraUtility.DrawClosedMultiLine(ref vh, plist, Color.black, 1, 1f, true);

        IllustraUtility.DrawMultiLine(ref vh, plist, color, 1, 1f, true);

        IllustraUtility.DrawLine(ref vh, new Vector3(50, 50), new Vector3(80, 50), color, 1,1);

        IllustraUtility.DrawCircularLine(ref vh, new Vector3(0, 0), 50, color, 48, 1, 0.5f);

        IllustraUtility.DrawRectangleLine(ref vh, new Vector3(50, 50), new Vector3(180, 180), color, 1, 0.5f);

       // base.OnPopulateMesh(vh);

    }
}
