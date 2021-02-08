using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollCredits : MonoBehaviour
{

    public GameObject creditsRun;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RollCredits());
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator RollCredits()
    {
        creditsRun.SetActive(true);

        yield return new WaitForSeconds(25);

        SceneManager.LoadScene("MainMenu");
    }
}
