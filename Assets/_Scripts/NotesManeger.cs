using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManeger : MonoBehaviour {

    private List<List<Note>> activeNotes;
    static public OscController controller;

    public int[] ch0;
    public int[] ch1;
    public int[] ch2;
    public int[] ch3;
    public int[] ch4;
    public int[] ch5;
    public int[] ch6;
    public int[] ch7;

    private List<int[]> notes;
    
    // Use this for initialization
    void Start () {

        notes = new List<int[]>();

        //SetChromaticScale(ch0);
        SetChromaticScale(ch1);
        setMajourScale(ch2);
        setMajourScale(ch3);
        setMajourScale(ch4);
        setMajourScale(ch5);
        setMajourScale(ch6);
        setMajourScale(ch7);

        notes.Add(ch0);
        notes.Add(ch1);
        notes.Add(ch2);
        notes.Add(ch3);
        notes.Add(ch4);
        notes.Add(ch5);
        notes.Add(ch6);
        notes.Add(ch7);

        activeNotes = new List<List<Note>>();
        for (int i = 0; i < 16; i++) {
            List<Note> l = new List<Note>();
            activeNotes.Add(l);
        }

        controller = GetComponent<OscController>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddNote(int x, int y, int z, float v, Cuboctahedron _co) {

        int ch = x+1;
        int nnum = MapMidiNumber(x, y);

        Note note = new Note(x, y, z, ch, nnum, v, _co);
        activeNotes[z].Add(note);
    }

    public void DeleteNote(int x, int y, int z) {

        List<Note> notesInBeat = activeNotes[z];
        int i = notesInBeat.FindIndex(n => n.x == x && n.y == y);

        notesInBeat.RemoveAt(i);

    }

    public List<Note> GetNotesInBeat(int beat) {
        return activeNotes[beat];
    }

    public Note GetNote(int x, int y, int z) {
        //Debug.Log("Search Note: " + beat +"/"+ ch +"/"+ note + " in " + activeNotes[beat].Count);

        Note result = activeNotes[z].Find(n => n.x == x && n.y == y);

        //if (result != null) Debug.Log("Result Note: " + result.ch +"/"+ result.note);

        return result;

    }

    int MapMidiNumber(int x, int y) {

        int noteNum = notes[x][y];

        return noteNum;

    }

    void SetChromaticScale(int[] note) {

        note[1] = note[0] + 1;
        note[2] = note[0] + 2;
        note[3] = note[0] + 3;
        note[4] = note[0] + 4;
        note[5] = note[0] + 5;
        note[6] = note[0] + 6;
        note[7] = note[0] + 7;

    }

    void setMajourScale(int[] note) {

        note[1] = note[0] + 2;
        note[2] = note[0] + 4;
        note[3] = note[0] + 5;
        note[4] = note[0] + 7;
        note[5] = note[0] + 9;
        note[6] = note[0] + 11;
        note[7] = note[0] + 12;
    }

    void setMinorScale(int[] note) {

        note[1] = note[0] + 2;
        note[2] = note[0] + 3;
        note[3] = note[0] + 5;
        note[4] = note[0] + 7;
        note[5] = note[0] + 8;
        note[6] = note[0] + 10;
        note[7] = note[0] + 12;

    }

}
public class Note {
    public int ch, note;
    public float velocity;
    public int x, y, z;
    Cuboctahedron co;

    public Note(int _x, int _y, int _z, int _ch, int _note, float _vel, Cuboctahedron _co) {

        x = _x;
        y = _y;
        z = _z;

        ch = _ch;
        note = _note;

        velocity = _vel;
        co = _co;

        //Bang();

    }

    public void SetVeloctiy(float vel) {
        velocity = vel;
    }

    public void Bang() {

        NotesManeger.controller.SendOscToMac(x, y, note, velocity);
        co.Glow();
       
    }
}
