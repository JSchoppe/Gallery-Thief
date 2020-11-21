using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SequenceSkipper : MonoBehaviour
{
    [SerializeField] private PlayableDirector director = null;
    [SerializeField] private GameObject linkedCanvas = null;

    // Start is called before the first frame update
    void Start()
    {
        director.played += FinishedSequence;
    }

    private void FinishedSequence(PlayableDirector obj)
    {
        StartCoroutine(DeleteNextFrame());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            director.time = 100f;
            StartCoroutine(DeleteNextFrame());
        }
    }

    IEnumerator DeleteNextFrame()
    {
        yield return null;
        Destroy(gameObject);
        Destroy(linkedCanvas);
    }
}
