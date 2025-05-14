using UnityEngine;

public class Song
{
    [Header("Song")]
    public string myName = "default song";
    public float duration = 180f;
    public float bpm = 120f;

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

        // Song 1
        // TBD: Add support for changing bpm
        Song song_1 = new Song("I Miss You So Much", 252, 105);
        GM.I.songs.Add(song_1);

        // Song 2
        Song song_2 = new Song("Picking Up a Cat", 161, 100);
        GM.I.songs.Add(song_2);

        // Song 3
        Song song_3 = new Song("Sugar Daddy", 147, 98);
        GM.I.songs.Add(song_3);

        // Song 4
        Song song_4 = new Song("If the trees go we all go", 160, 120);
        GM.I.songs.Add(song_4);

        // Song 5
        Song song_5 = new Song("A Garden of Your Own", 256, 120);
        GM.I.songs.Add(song_5);

        // Song 6
        Song song_6 = new Song("No Reason", 179, 113);
        GM.I.songs.Add(song_6);

        // Song 7
        Song song_7 = new Song("Quiero Jugar", 144, 120);
        GM.I.songs.Add(song_7);

        // Initialize song index
        //GM.I.songIndex = 0;
        //GM.I.songIndex = Random.Range(0, GM.I.songs.Count);
        //GM.I.unlockedSongIndex = GM.I.songIndex;

        // Initialize first song
        //GM.I.songTimer = GM.I.songs[GM.I.songIndex].duration;
    }
}
