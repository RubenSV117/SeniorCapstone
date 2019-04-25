using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data container for a favorite
/// </summary>
[Serializable]
public class favorite
{

    public string Name;
    public bool Lean;

    public favorite(string name,bool lean)
    {

        Name = name;
        Lean = lean;
    }
}
