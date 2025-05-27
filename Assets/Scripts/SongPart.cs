using UnityEngine;

public class SongPart
{
    public float duration;
    public float baseHype;
    public float growingHype;
    public int bpm = -1;
    
    public SongPart(float _duration, float _baseHype, float _growingHype)
    {
        duration = _duration;
        baseHype = _baseHype;
        growingHype = _growingHype;
    }

    public SongPart(float _duration, float _baseHype, float _growingHype, int _bpm)
    {
        duration = _duration;
        baseHype = _baseHype;
        growingHype = _growingHype;
        bpm = _bpm;
    }
}
