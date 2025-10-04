using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



public class InteractiveExit : InteractiveObject
{
    ParticleSystem exitParticles;

    // public override void Interact()
    // {
    //     exitParticles.Play();
    //     StartCoroutine(ToNextScene());
    // }

    IEnumerator ToNextScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(GameManager.stageManager.stageDataController.NextScene);
    }
}
