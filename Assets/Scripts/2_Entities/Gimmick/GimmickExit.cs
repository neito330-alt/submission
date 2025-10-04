using System.Collections;
using System.Collections.Generic;
using Rooms.RoomSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GimmickExit : GimmickController
{
    [SerializeField]
    private InteractiveObject _interactiveObject;

    [SerializeField]
    private AudioSource _audioSource;


    [SerializeField]
    private ParticleSystem[] exitParticles;



    // Start is called before the first frame update
    void Start()
    {
        _interactiveObject.Interact += Interact;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Interact()
    {
        foreach (var particle in exitParticles)
        {
            if (particle.isPlaying) continue;
            particle.Play();
        }
        _audioSource.Play();
        StartCoroutine(ToNextScene());
    }

    IEnumerator ToNextScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(GameManager.stageManager.stageDataController.NextScene);
    }
}
