using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
   private DatabaseReference _reference;

   private void Awake()
   {
      FirebaseApp app = FirebaseApp.DefaultInstance;
      _reference = FirebaseDatabase.DefaultInstance.RootReference;
      //FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
      
   }
   

   public void WriteDatabase(string path, string message)
   {
      _reference.Child(path).SetValueAsync(message).ContinueWithOnMainThread(task =>
      {
         if (task.IsFaulted)
         {
            Debug.Log("That bai");
         }
         else
         {
            Debug.Log("Thanh cong");
         }
      });
   }

   public void ReadDatabase(string id)
   {
      _reference.Child("Users").Child("id").GetValueAsync().ContinueWithOnMainThread(task =>
      {
         if (task.IsFaulted)
         {
            Debug.Log("That bai");
         }
         else
         {
             DataSnapshot snapshot = task.Result;
             Debug.Log(snapshot.Value.ToString());
         }
      });
   }
}
