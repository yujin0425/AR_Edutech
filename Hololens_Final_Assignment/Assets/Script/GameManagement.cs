using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManagement : MonoBehaviour
{
    public static GameManagement instance = null;


    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ReStartGame()
    {
        StartCoroutine(WaitStart());
    }

    IEnumerator WaitStart()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(0);
    }
}
