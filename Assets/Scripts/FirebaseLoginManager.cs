using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirebaseLoginManager : MonoBehaviour
{
   //Đăng kí
   
   public InputField inputEmail;
   public InputField inputPassword;
   public Button registerButton;
   
   //Đăng nhập 
   public InputField inputLoginEmail;
   public InputField inputLoginPassword;
   public Button loginButton;
   
   // Đổi qua lại 
   public Button buttonMoveToSignIn;
   public Button buttonMoveToRegister;
   
   public GameObject loginForm;
   public GameObject registerForm;
   
   FirebaseAuth auth;

   void Start()
   {
      auth = FirebaseAuth.DefaultInstance;
      registerButton.onClick.AddListener(() =>
         {
            OnClickRegisterButton();
         }
      );
      loginButton.onClick.AddListener(() =>
         {
            OnClickLoginButton();
         }
      );
      buttonMoveToRegister.onClick.AddListener(() =>
         {
            SwitchForm();
         }
      );
      buttonMoveToSignIn.onClick.AddListener(() =>
         {
            SwitchForm();
         }
      );
   }

   public void SwitchForm()
   {
      loginForm.SetActive(!loginForm.activeSelf);
      registerForm.SetActive(!registerForm.activeSelf);
   }

   public void OnClickRegisterButton()
   {
      string email = inputEmail.text;
      string password = inputPassword.text;
      auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
      {
         if (task.IsCanceled)
         {
            Debug.Log("Registration Cancelled");
            return;
         }
         if (task.IsFaulted)
         {
            Debug.Log("Registration Fauled");

         }

         if (task.IsCompleted)
         {
            Debug.Log("Registration Completed");
            
         }
      });
   }
   
   public void OnClickLoginButton()
   {
      string email = inputLoginEmail.text;
      string password = inputLoginPassword.text;
      auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
      {
         if (task.IsCanceled)
         {
            Debug.Log("Login Cancelled");
            return;
         }
         if (task.IsFaulted)
         {
            Debug.Log("Login Fauled");

         }

         if (task.IsCompleted)
         {
            Debug.Log("Login Completed");
            FirebaseUser user = task.Result.User;
            
            //Chuyển màn chơi sau khi đăng nhập thành công
            SceneManager.LoadScene("PlayScene");
         }
      });
   }
}
