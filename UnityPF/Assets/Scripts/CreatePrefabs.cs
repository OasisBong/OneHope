using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePrefabs : GameFramework {
    

    GameObject m_buildObj = null;    //만들 오브젝트
    Material m_buildMtl = null;             //만들 오브젝트의 원 마테리얼
    public Camera mainCamera;               //캐릭터 카메라
    public Shader redShader;                //불가능시 쓰일 빨간색 쉐이더
    
    GameObject m_createObj = null;          //위치조정때 쓰일 보여주기식
    bool isCreating = false;                //만들겠다고 위치조정 중인지

    public UISprite FbtnImage;
    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update ()
    {
        //디버그용 c 눌러서 캠파만들기
        //if (!isCreating && Input.GetKeyDown(KeyCode.C))
        //{
        //    isCreating = true;
        //    this.OnObjDecide(m_buildObj, publicMtl);
        //    m_createObj = Instantiate(m_buildObj);
        //    m_createObj.transform.position = new Vector3(0f, -500f, 0f);
        //}

        //오브젝트 생성
        if (isCreating)
            CreateObject();
        else
        {
            CheckObject();
        }
        //오브젝트 활성화
        
	}
    

    //=======================#  오브젝트 상호작용 #==========================

    void CheckObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] rhit;

        rhit = Physics.RaycastAll(ray, 10f);
        float minLength = 10.0f;
        int Number = -1;
        for (int i = 0; i < rhit.Length; i++)
        {
            if ("ActiveObj" == rhit[i].collider.tag)
            {
                if (rhit[i].distance < minLength)
                {
                    minLength = rhit[i].distance;
                    Number = i;
                }
            }
        }

        if (Number < 0)
        {
            FbtnImage.enabled = false;
            return;
        }

        FbtnImage.enabled = true;
        if (Input.GetKeyDown(KeyCode.F))
        {
            ActiveObjInfo info = rhit[Number].collider.GetComponentInChildren<ActiveObjInfo>();
            if (info == null) info = rhit[Number].collider.GetComponent<ActiveObjInfo>();

            info.OnActive();
        }
    }

    //==========================================================================





    //==========================#  오브젝트 생성  #=============================

    void CreateObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] rhit;

        rhit = Physics.RaycastAll(ray, 10f);
        float minLength = 10.0f;
        int Number = -1;
        for (int i = 0; i < rhit.Length;i++)
        {
            if("Ground" == rhit[i].collider.tag)
            {
                if (rhit[i].distance < minLength)
                {
                    minLength = rhit[i].distance;
                    Number = i;
                }
            }
        }

        if (Number >= 0)
        {
            m_createObj.transform.position = rhit[Number].point;
        }
        
        //물건회전
        if (Input.GetKey(KeyCode.Q))
            m_createObj.transform.Rotate(Vector3.up, -2.0f);
        else if (Input.GetKey(KeyCode.E))
            m_createObj.transform.Rotate(Vector3.up, 2.0f);

        bool impossible = false;


        //기울기체크 , 다른 오브젝트와 충돌 체크
        if(!this.PosDecide() || !m_createObj.GetComponentInChildren<ActiveObjInfo>().CheckBuildPos())
        {
            m_createObj.GetComponentInChildren<MeshRenderer>().material.shader = redShader;
            impossible = true;
        }
        else
        {
            m_createObj.GetComponentInChildren<MeshRenderer>().material = m_buildMtl;
        }

        //클릭시 생성
        if(Input.GetMouseButtonDown(0) && !impossible)
        {
            //충돌처리 활성화
            BoxCollider col = m_createObj.GetComponent<BoxCollider>();
            if(col != null) col.enabled = true;
            else m_createObj.GetComponentInChildren<BoxCollider>().enabled = true;

            SEND_USER_CREATE_OBJ(m_createObj.name, m_createObj.transform.position);

            isCreating = false;
            m_buildMtl = null;
            m_createObj = null;
        }
    }

    bool PosDecide()
    {
        BoxCollider col = m_createObj.GetComponent<BoxCollider>();
        if (col == null) col = m_createObj.GetComponentInChildren<BoxCollider>();

        Vector3 center = col.center + m_createObj.transform.position;
        center.y += col.size.z * 2;
        Vector3[] arrPt = new Vector3[4] { center, center, center, center };
        float[] arrLength = new float[4] { 20f, 20f, 20f, 20f };

        arrPt[0].x += col.size.x / 2f;
        arrPt[0].z += col.size.y / 2f;

        arrPt[1].x += col.size.x / 2f;
        arrPt[1].z -= col.size.y / 2f;

        arrPt[2].x -= col.size.x / 2f;
        arrPt[2].z += col.size.y / 2f;

        arrPt[3].x -= col.size.x / 2f;
        arrPt[3].z -= col.size.y / 2f;

        Ray ray = new Ray();

        for (int k = 0; k < 4; k++)
        {
            RaycastHit[] rayHit;
            
            ray.origin = arrPt[k];
            ray.direction = new Vector3(0, -1, 0);

            rayHit = Physics.RaycastAll(ray, 1.5f);

            float minLength = 4f;
            int Number = -1;
            for (int i = 0; i < rayHit.Length; i++)
            {
                if ("Ground" == rayHit[i].collider.tag)
                {
                    if (rayHit[i].distance < minLength)
                    {
                        minLength = rayHit[i].distance;
                        Number = i;
                    }
                }
            }

            if (Number >= 0)
                arrLength[k] = rayHit[Number].distance;
        }

        float _min, _max;
        _min = Mathf.Min(arrLength);
        _max = Mathf.Max(arrLength);
        
        Debug.Log(_min.ToString() + " , " + _max.ToString());
        if (_min >= 3f) return false;

        if (_max - _min <= Mathf.Epsilon)
            return true;

        if (_max - _min >= 1f)
            return false;

        return true;
    }
    
    public void OnObjDecide(object buildObj , Material buildMtl)
    {
        m_buildMtl = buildMtl;
        m_buildObj = buildObj as GameObject;

        isCreating = true;
        m_createObj = Instantiate(m_buildObj);
        m_createObj.transform.position = new Vector3(0f, -500f, 0f);
        m_createObj.name = m_buildObj.name;
    }

    public void OnCreateStart()
    {
    }

    //==========================================================================
}
