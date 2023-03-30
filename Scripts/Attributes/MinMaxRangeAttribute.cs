using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MinMaxRangeAttribute : PropertyAttribute {
    public float MinLimit { get; set; }
    public float MaxLimit { get; set; }

    public MinMaxRangeAttribute(float minLimit, float maxLimit) {
        MinLimit = minLimit;
        MaxLimit = maxLimit;
    }
}

[Serializable]
public struct MinMaxRange {
    [SerializeField]
    private float _rangeStart;
    
    [SerializeField]
    private float _rangeEnd;

    public float GetRandomValue() {
        return Random.Range(_rangeStart, _rangeEnd);
    }
}