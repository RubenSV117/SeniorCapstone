using System;
using UnityEngine;
using Firebase.Storage;
using System.Collections.Generic;

/// <summary>
/// Data container for a recipe
/// </summary>
[Serializable]
public class Review
{
    public string content;
    public DateTime timestamp;
    public string userId;
    public Review(string content, DateTime timestamp, string userId)
    {
        this.content = content;
        this.timestamp = timestamp;
        this.userId = userId;
    }


    public override string ToString()
    {
        return this.content;
    }
}
