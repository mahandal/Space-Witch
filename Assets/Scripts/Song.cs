using UnityEngine;
using System.Collections.Generic;

public class Song
{
    [Header("Song")]
    public string myName = "default song";
    public float duration = 180f;
    public float bpm = 120f;

    // Song parts
    public List<SongPart> parts = new List<SongPart>();



    // Basic constructor
    public Song(string _myName, float _duration, float _bpm)
    {
        myName = _myName;
        duration = _duration;
        bpm = _bpm;
    }

    // Initialize GM's list of songs
    public static void InitializeSongs()
    {
        // Clear songs just in case?
        GM.I.songs.Clear();

        // Song 1 - I Miss You So Much
        // TBD: Test support for changing bpm
        Song song_1 = new Song("I Miss You So Much", 252, 105);
        song_1.parts.Add(new SongPart(5f, 0f, 0f));
        song_1.parts.Add(new SongPart(72f, 0f, 0.1f));
        song_1.parts.Add(new SongPart(73f, 0f, 0.2f));
        song_1.parts.Add(new SongPart(18f, 0f, 1f));
        song_1.parts.Add(new SongPart(84f, 13f, 0.1f, 120));
        GM.I.songs.Add(song_1);

        // Song 2
        Song song_2 = new Song("Picking Up a Cat", 161, 100);
        song_2.parts.Add(new SongPart(2f, 0f, 0f));
        song_2.parts.Add(new SongPart(19f, 1f, 0.1f));
        song_2.parts.Add(new SongPart(19f, 0f, 0.2f));
        song_2.parts.Add(new SongPart(19f, 1f, 0.2f));
        song_2.parts.Add(new SongPart(2f, 0f, 0f));
        song_2.parts.Add(new SongPart(19f, 8f, 0.2f));
        song_2.parts.Add(new SongPart(38f, 0f, 0.2f));
        song_2.parts.Add(new SongPart(2f, 0f, 0f));
        song_2.parts.Add(new SongPart(38f, 8f, 0.2f));
        GM.I.songs.Add(song_2);

        // Song 3
        Song song_3 = new Song("Sugar Daddy", 147, 98);
        song_3.parts.Add(new SongPart(9f, 0f, 0f));
        song_3.parts.Add(new SongPart(20f, 0f, 0.2f));
        song_3.parts.Add(new SongPart(40f, 7f, 0.1f));
        song_3.parts.Add(new SongPart(39f, 0f, 0.2f));
        song_3.parts.Add(new SongPart(39f, 7f, 0.1f));
        GM.I.songs.Add(song_3);

        // Song 4
        Song song_4 = new Song("If the trees go we all go", 160, 120);
        song_4.parts.Add(new SongPart(16f, 0f, 0f));
        song_4.parts.Add(new SongPart(48f, 0f, 0.1f));
        song_4.parts.Add(new SongPart(32f, 2f, 0.1f));
        song_4.parts.Add(new SongPart(16f, 0f, 0f));
        song_4.parts.Add(new SongPart(32f, 2f, 0.1f));
        song_4.parts.Add(new SongPart(16f, 0f, 0f));
        GM.I.songs.Add(song_4);

        // Song 5
        Song song_5 = new Song("A Garden of Your Own", 256, 120);
        song_5.parts.Add(new SongPart(32f, 0f, 0f));
        song_5.parts.Add(new SongPart(32f, 1f, 0.1f));
        song_5.parts.Add(new SongPart(32f, 2f, 0.1f));
        song_5.parts.Add(new SongPart(32f, 4f, 0.1f));
        song_5.parts.Add(new SongPart(32f, 0f, 0f));
        song_5.parts.Add(new SongPart(96f, 1f, 0.1f));
        GM.I.songs.Add(song_5);

        // Song 6
        Song song_6 = new Song("No Reason", 179, 113);
        song_6.parts.Add(new SongPart(8, 0f, 0f));
        song_6.parts.Add(new SongPart(8, 0f, 0.1f));
        song_6.parts.Add(new SongPart(16, 1f, 0.1f));
        song_6.parts.Add(new SongPart(19, 1f, 0.1f));
        song_6.parts.Add(new SongPart(17, 16f, -0.1f));
        song_6.parts.Add(new SongPart(17, 16f, -0.1f));
        song_6.parts.Add(new SongPart(17, 0f, 0.1f));
        song_6.parts.Add(new SongPart(8, 0f, 0f));
        song_6.parts.Add(new SongPart(34, 8f, 0.1f));
        song_6.parts.Add(new SongPart(34, 16f, -0.1f));
        GM.I.songs.Add(song_6);

        // Song 7
        Song song_7 = new Song("Quiero Jugar", 144, 120);
        song_7.parts.Add(new SongPart(8, 0f, 0f));
        song_7.parts.Add(new SongPart(16, 0f, 0.1f));
        song_7.parts.Add(new SongPart(16, 0f, 0.5f));
        song_7.parts.Add(new SongPart(16, 12f, 0f));
        song_7.parts.Add(new SongPart(12, 12f, 0.1f));
        song_7.parts.Add(new SongPart(4, 0f, 0f));
        song_7.parts.Add(new SongPart(8, 1f, 0f));
        song_7.parts.Add(new SongPart(16, 0f, 0.1f));
        song_7.parts.Add(new SongPart(16, 0f, 0.5f));
        song_7.parts.Add(new SongPart(32, 12f, 0.1f));
        GM.I.songs.Add(song_7);

        // Initialize song index
        //GM.I.songIndex = 0;
        //GM.I.songIndex = Random.Range(0, GM.I.songs.Count);
        //GM.I.unlockedSongIndex = GM.I.songIndex;

        // Initialize first song
        //GM.I.songTimer = GM.I.songs[GM.I.songIndex].duration;
    }
}
