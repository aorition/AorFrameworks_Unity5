using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public static class UGUI_FontUpdateTracker_SC
    {
        static Dictionary<Font, HashSet<UGUI_Text_SC>> m_Tracked = new Dictionary<Font, HashSet<UGUI_Text_SC>>();

        public static void TrackText(UGUI_Text_SC t)
        {
            if (t.font == null)
                return;

            HashSet<UGUI_Text_SC> exists;
            m_Tracked.TryGetValue(t.font, out exists);
            if (exists == null)
            {
                // The textureRebuilt event is global for all fonts, so we add our delegate the first time we register *any* UGUI_Text_SC
                if (m_Tracked.Count == 0)
                    Font.textureRebuilt += RebuildForFont;

                exists = new HashSet<UGUI_Text_SC>();
                m_Tracked.Add(t.font, exists);
            }

            if (!exists.Contains(t))
                exists.Add(t);
        }

        private static void RebuildForFont(Font f)
        {
            HashSet<UGUI_Text_SC> texts;
            m_Tracked.TryGetValue(f, out texts);

            if (texts == null)
                return;

            foreach (var UGUI_Text_SC in texts)
                UGUI_Text_SC.FontTextureChanged();
        }

        public static void UntrackText(UGUI_Text_SC t)
        {
            if (t.font == null)
                return;

            HashSet<UGUI_Text_SC> texts;
            m_Tracked.TryGetValue(t.font, out texts);

            if (texts == null)
                return;

            texts.Remove(t);

            if (texts.Count == 0)
            {
                m_Tracked.Remove(t.font);

                // There is a global textureRebuilt event for all fonts, so once the last UGUI_Text_SC reference goes away, remove our delegate
                if (m_Tracked.Count == 0)
                    Font.textureRebuilt -= RebuildForFont;
            }
        }
    }
}
