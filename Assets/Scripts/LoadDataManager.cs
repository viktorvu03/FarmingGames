using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

public class LoadDataManager : MonoBehaviour
{
    public static FirebaseUser firebaseUser;
    public static User userInGame;
    
    private DatabaseReference _reference;
    
    // Start is called before the first frame update
    void Awake()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        _reference = FirebaseDatabase.DefaultInstance.RootReference;
        firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
        GetUserInGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetUserInGame()
    {
        _reference.Child("Users").Child(firebaseUser.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("That bai");
            }
            else
            {
                DataSnapshot snapshot = task.Result;
                userInGame = JsonConvert.DeserializeObject<User>(snapshot.Value.ToString());
            }
        });
    }
}
