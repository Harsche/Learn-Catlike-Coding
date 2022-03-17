using UnityEngine;
using System;

public class Clock : MonoBehaviour {
    [SerializeField] private Transform hoursArmPivot, minutesArmPivot, secondsArmPivot;
    private const int hoursAngle = 360 / 12 * -1;
    private const int minutesAngle = 360 / 60 * -1;
    private const int secondsAngle = 360 / 60 * -1;

    private void Update() {
        UpdateClock();
    }

    private void UpdateClock() {
        TimeSpan time = DateTime.Now.TimeOfDay;
        hoursArmPivot.localRotation = Quaternion.Euler(0f, 0f, hoursAngle * (float)time.TotalHours);
        minutesArmPivot.localRotation = Quaternion.Euler(0f, 0f, minutesAngle * (float)time.TotalMinutes);
        secondsArmPivot.localRotation = Quaternion.Euler(0f, 0f, secondsAngle * (float)time.TotalSeconds);
    }
}
