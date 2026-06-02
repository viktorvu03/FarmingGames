using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DayAndNightManager : MonoBehaviour
{
   [SerializeField] 
   private Text textTime;
   public float dayDuration = 60f;
   
   [SerializeField] 
   private Light2D light2D;
   [SerializeField] 
   private Gradient gradient;

   void Update()
   {
      DateTime realtime = DateTime.Now;
      
      float realSecondsInDay = (realtime.Hour * 3600) + (realtime.Minute *60) + realtime.Second;
      realSecondsInDay = realSecondsInDay * dayDuration % 86400;

      // float gameTimeSeconds = (realSecondsInDay / (24 * 3600)) * (dayDuration * 60);
      
      int gameHours = Mathf.FloorToInt(realSecondsInDay / 3600);
      int gameMinutes = Mathf.FloorToInt(realSecondsInDay % 3600)/60 ;
      
      string timeFormatted = string.Format("{0:00}:{1:00}", gameHours, gameMinutes);
      textTime.text = timeFormatted;
      
      ChangeColorByTime(realSecondsInDay);
   }

   public void ChangeColorByTime(float seconds)
   {
      light2D.color = gradient.Evaluate(seconds / 86400);
   }
}
