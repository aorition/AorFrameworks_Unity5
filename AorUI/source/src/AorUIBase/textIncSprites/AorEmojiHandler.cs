using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AorEmojiHandler {

    private static AorEmojiHandler _instance;
    public static AorEmojiHandler instance {
        get { return _instance; }
    }

    public static AorEmojiHandler Initialization(Sprite[] emojiSpritess) {
        _instance = new AorEmojiHandler(emojiSpritess);
        return _instance;
    }

    private static string GetConvertedString(string inputString) {
        string[] converted = inputString.Split('-');
        for (int j = 0; j < converted.Length; j++) {
            converted[j] = char.ConvertFromUtf32(Convert.ToInt32(converted[j], 16));
        }
        return string.Join(string.Empty, converted);
    }

    private Dictionary<string, Sprite> _emojiSpriteDic = new Dictionary<string, Sprite>();

    private AorEmojiHandler(Sprite[] emojiSpritess) {
        int i, len = emojiSpritess.Length;
        for (i = 0; i < len; i++) {
            _emojiSpriteDic.Add(GetConvertedString(emojiSpritess[i].name), emojiSpritess[i]);
        }
    }

    public int FillQuadDefaultSize = 16;
    public string FillQuadTempStr = "<quad name={name} size={size} />";

    private string getTagStr(string spriteName, int QuadSize) {
        string str = FillQuadTempStr.Replace("{name}", spriteName);
        str = str.Replace("{size}", (QuadSize == -1 ? FillQuadDefaultSize.ToString() : QuadSize.ToString()));
        return str;
    }

    public string DoEmojiTextToAorRichText(string inputString, int QuadSize = -1) {
        StringBuilder sb = new StringBuilder();
        
        int i = 0;
        while (i < inputString.Length) {
            string singleChar = inputString.Substring(i, 1);
            string doubleChar = "";
            string fourChar = "";

            if (i < (inputString.Length - 1)) {
                doubleChar = inputString.Substring(i, 2);
            }

            if (i < (inputString.Length - 3)) {
                fourChar = inputString.Substring(i, 4);
            }

            if (_emojiSpriteDic.ContainsKey(fourChar)) {
                // Check 64 bit emojis first
                //加入转译后的字段;
                sb.Append(getTagStr(_emojiSpriteDic[fourChar].name, QuadSize));
                i += 4;
            } else if (_emojiSpriteDic.ContainsKey(doubleChar)) {
                // Then check 32 bit emojis
                sb.Append(getTagStr(_emojiSpriteDic[doubleChar].name, QuadSize));
                i += 2;
            } else if (_emojiSpriteDic.ContainsKey(singleChar)) {
                // Finally check 16 bit emojis
                sb.Append(getTagStr(_emojiSpriteDic[singleChar].name, QuadSize));
                i++;
            } else {
                sb.Append(inputString[i]);
                i++;
            }

        }

        return sb.ToString();
    }

}
