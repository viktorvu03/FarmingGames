using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FakeLoad : MonoBehaviour
{
    public Button loadButton;
    // Start is called before the first frame update
    void Start()
    {
        loadButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("PlayScene");
            }
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
