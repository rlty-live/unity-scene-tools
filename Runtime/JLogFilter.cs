using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JLogFilter", menuName = "RLTY/JLogFilter", order = 1)]
public class JLogFilter : ScriptableObject
{
    public bool logUnfiltered = true;
    public List<Entry> filters;

    [System.Serializable]
    public class Entry
    {
        public bool log = true;
        public Color color = Color.green;
        public List<string> list;
        public List<TextAsset> scriptsList;
    }

    public bool Filter(string inString, out string color)
    {
        foreach (Entry e in filters)
            foreach (string s in e.list)
                if (inString.Contains(s))
                {
                    color  = ColorUtility.ToHtmlStringRGB(e.color);
                    return e.log;
                }
        
        foreach (Entry e in filters)
        foreach (TextAsset t in e.scriptsList)
            if (t && inString.Contains(t.name))
            {
                color  = ColorUtility.ToHtmlStringRGB(e.color);
                return e.log;
            }
        
        color= "FFFFFF";
        return logUnfiltered;
    }
}
