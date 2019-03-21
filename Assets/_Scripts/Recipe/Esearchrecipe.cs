using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Shards
{
    public int total { get; set; }
    public int successful { get; set; }
    public int skipped { get; set; }
    public int failed { get; set; }
}

public class Source
{
    public string name { get; set; }
    public List<string> ingredients { get; set; }
    public List<string> tags { get; set; }
}

public class Hit
{
    public string _index { get; set; }
    public string _type { get; set; }
    public string _id { get; set; }
    public int _score { get; set; }
    public Source _source { get; set; }
}

public class Hits
{
    public int total { get; set; }
    public int max_score { get; set; }
    public List<Hit> hits { get; set; }
}

public class ElasticSearchResult
{
    public int took { get; set; }
    public bool timed_out { get; set; }
    public Shards _shards { get; set; }
    public Hits hits { get; set; }
}