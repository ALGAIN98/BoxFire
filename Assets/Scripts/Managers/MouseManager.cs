using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;

    RaycastHit hitInfo;

    public event Action<Vector3> OnMouseClicked;

    public event Action<GameObject> OnEnemyClicked;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    //mobile game就不换鼠标贴图了，所以下面这个SetCursorTexture没用
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //切换鼠标贴图
        }


    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);

        }
        
    }

}



