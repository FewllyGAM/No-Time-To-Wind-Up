using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObjectUI : MonoBehaviour
{
    [SerializeField] Image fillImage;
    BaseWindow window;

    Camera mainCam;

    private void Awake()
    {
        window = GetComponentInChildren<BaseWindow>();
    }

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(mainCam.transform.position);
    }

    public void Toggle(bool value)
    {
        if (value) window.Show();
        else window.Hide();
    }

    public void Fill(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }
    public void Fill(float rate)
    {
        fillImage.fillAmount = rate;
    }
}
