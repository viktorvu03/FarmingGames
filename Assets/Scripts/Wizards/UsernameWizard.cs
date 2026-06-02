using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsernameWizard : MonoBehaviour
{
    public GameObject block;
    public GameObject usernameWizard;
    public InputField usernameInput;
    public Button buttonOK;
    public Text usernameText;
    public Text gold;
    public Text diamond;
    public TitleMapManager titleMapManager;
    
    private FirebaseDatabaseManager databaseManager;
    
    // Start is called before the first frame update
    void Start()
    {
        databaseManager = GameObject.Find("DatabaseManager").GetComponent<FirebaseDatabaseManager>();
        if (LoadDataManager.userInGame.Name =="")
        {
            usernameWizard.SetActive(true);
            block.SetActive(true);
        }
        else
        {
            usernameText.text = LoadDataManager.userInGame.Name;
        }
        
        gold.text ="Gold:" + LoadDataManager.userInGame.Gold.ToString();
        diamond.text ="Diamond:" + LoadDataManager.userInGame.Diamond.ToString();
        
        buttonOK.onClick.AddListener(() =>  
        {
            titleMapManager.LoadMapForUser();
            block.SetActive(false);
            
            SetUsername();
        });
    }

    public void ReloadUI()
    {
        gold.text ="Gold:" + LoadDataManager.userInGame.Gold.ToString();
        diamond.text ="Diamond:" + LoadDataManager.userInGame.Diamond.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUsername()
    {
        if (usernameInput.text != "")
        {
            LoadDataManager.userInGame.Name = usernameInput.text;
            
            databaseManager.WriteDatabase("Users/" + LoadDataManager.firebaseUser.UserId,LoadDataManager.userInGame.ToString());
            usernameWizard.SetActive(false);
            usernameText.text = usernameInput.text;
        }
    }
}
