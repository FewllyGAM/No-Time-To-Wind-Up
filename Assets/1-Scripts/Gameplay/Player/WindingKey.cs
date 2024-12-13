using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindingKey : MonoBehaviour
{
    [SerializeField] float valueRate = 30;
    [SerializeField] float interactableY = .1f;

    public void UpdateRotation(float currentWind)
    {
       // Quaternion newRot = new Quaternion()
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentWind * valueRate, transform.localEulerAngles.z);
    }
    public void UpdateContinuousRotation(float value)
    {
        // Quaternion newRot = new Quaternion()
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + (value * valueRate), transform.localEulerAngles.z);
    }

    public void SetInteractable(bool value)
    {
        if (value) transform.localPosition = new Vector3(0, interactableY, 0);
        else transform.localPosition = Vector3.zero;
    }
}
