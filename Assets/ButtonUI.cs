using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] int sceneIndex;

    public void SwitchToScene()
    {
        SceneManager.LoadScene(sceneIndex);
        print("Hello");
    }
}
