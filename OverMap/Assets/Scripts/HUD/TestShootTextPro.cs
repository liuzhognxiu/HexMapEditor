using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestShootTextPro : MonoBehaviour
{
    ShootTextProController shootTextProController;

    void Start()
    {
        shootTextProController = GetComponent<ShootTextProController>();
    }


    void Update()
    {
        #region obsolete
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            shootTextProController.DelayMoveTime = 10f;
            shootTextProController.textAnimationType = TextAnimationType.Burst;
            shootTextProController.CreateShootText("+12345", transform);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            shootTextProController.DelayMoveTime = 0.0f;
            shootTextProController.textAnimationType = TextAnimationType.Normal;
            shootTextProController.CreateShootText("+678910", transform);
        }
        #endregion
    }
}