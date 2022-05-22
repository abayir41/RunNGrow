using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
   public static Action Vibrate;
   public static Action<HapticTypes> VibrationSpecific;
   public static bool VibrationEnabled => PlayerPrefs.GetInt("Vibration") == 0;
   public void OnEnable()
   {
      Vibrate += DoVibrate;
      VibrationSpecific += DoVibrate;
   }
   
   private void OnDisable()
   {
      Vibrate -= DoVibrate;
      VibrationSpecific -= DoVibrate;
   }
   
   private void DoVibrate()
   {
      if(VibrationEnabled)
         MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
   }

   private void DoVibrate(HapticTypes type)
   {
      if(VibrationEnabled)
         MMVibrationManager.Haptic(type);
   }
   
}
