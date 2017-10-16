using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(NotesManeger))]
public class SequenceManager : MonoBehaviour {

    [Range(60, 240)]
    public int bpm;
    private float time16beat;
    private int currentBeatNum; // 0-15
    private NotesManeger data;

    private SequencerBar bar;

	void Start () {

        GameObject o = GameObject.FindGameObjectWithTag("Bar");
        bar = o.GetComponent<SequencerBar>();

        data = GetComponent<NotesManeger>();

        currentBeatNum = 0;
        time16beat = 60f / (float)bpm / 4f;

        StartCoroutine(UpdateBeat());

    }
	
	void Update () {

        time16beat = 60f / (float)bpm / 4f;

    }

    private IEnumerator UpdateBeat() {
        while (true) {

            yield return new WaitForSeconds(time16beat);

            currentBeatNum++;
            if (currentBeatNum == 16) currentBeatNum = 0;

            bar.UpdatePos(currentBeatNum);
            Bang(currentBeatNum);

        }
    }

    void Bang(int beatNum) {

        List<Note> l = data.GetNotesInBeat(beatNum);

        for (int i = 0; i < l.Count; i++) {

            l[i].Bang();

        }

    }
    

}
