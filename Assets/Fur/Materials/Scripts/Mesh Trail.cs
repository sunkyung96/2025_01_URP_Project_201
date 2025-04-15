using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2.0f; //�ܻ� ȿ�� ���� �ð�
    public MovementInput moveScript; //ĳ������ �������� ���� �ϴ� ��ũ��Ʈ
    public float speedBoost = 6; //���� ȿ�� ���� ���� ������
    public Animator animator; //ĳ������ �ִϸ��̼��� �����ϴ� ������Ʈ
    public float animSpeedBoost = 1.5f; //�ܻ� ȿ�� ��� �� �ִϸ��̼� �ӵ� ������

    [Header("Mesh Releted")]
    public float meshBefrcshRate = 1.0f; //�ܻ��� �����Ǵ� �ð� ����
    public float meshDestoryDelay = 3.0f; //������ �ܻ��� ������� �� �ɸ��� �ð�
    public Transform positionTospawm; //�ܻ��� ������ ��ġ

    [Header("Shader Releted")]
    public Material mat; //�ܻ��� ����� ����
    public string shaderVerRef; //���̴����� ����� ���� �̸� (Alpha)
    public float shaderVarRate = 0.1f; //�ܻ��� ������ ��ġ
    public float shaderVarRetreshRate = 0.05f; //���̴� ȿ���� ������Ʈ �Ǵ� �ð� ����

    private SkinnedMeshRenderer[] SkinnedMeshRenderer; //ĳ������ 3D ���� ������ �ϴ� ������Ʈ��
    private bool isTrailActive; //���� �ܻ� ȿ���� Ȱ��ȭ �Ǿ� �ִ��� Ȯ�� �ϴ� ����

    private float normalSpeed; //���� �̵� �ӵ��� �����ϴ� ����
    private float normalAnimSpeed; //���� �ִϸ��̼� �ӵ��� �����ϴ� ����

    //������ ������ ������ �����ϴ� �ڷ�ƾ
    IEnumerator AnimatateMaterialFloat(Material m, float vakueGoal, float rate, float refresfRate)
    {
        float valueToAnimate = m.GetFloat(shaderVerRef); //���� ���� �����´�.

        //��ǥ ���� ���� �� �� ���� �ݺ�
        while (valueToAnimate > vakueGoal)
        {
            valueToAnimate -= rate;
            m.SetFloat(shaderVerRef, valueToAnimate);
            yield return new WaitForSeconds(refresfRate);

        }
     
    }


    IEnumerator ActivateTrail(float timeActivated) //�ܻ�ȿ�� �ߵ�
    {
        //���� ���� ������ ����
        normalSpeed = moveScript.movementSpeed; //���� �ӵ��� �����ϰ� ������ �ӵ� ����
        moveScript.movementSpeed = speedBoost;


        normalAnimSpeed = animator.GetFloat("animSpeed"); //���� �ִϸ��̼� �ӵ� �����ϰ� ������ �ӵ� ����
        animator.SetFloat("animSpeed", animSpeedBoost);


        while (timeActivated > 0)
        {
            timeActivated -= meshBefrcshRate; //�ð� ī��Ʈ�� �Ѵ�. 0������

            if (SkinnedMeshRenderer == null)
                SkinnedMeshRenderer = positionTospawm.GetComponentsInChildren<SkinnedMeshRenderer>(); // ������ ��ġ�� ������ ������Ʈ���� ������

            for (int i = 0; i < SkinnedMeshRenderer.Length; i++) //�� �޽� �������� �ܻ� ����
            {
                GameObject g0bj = new GameObject(); //���ο� ������Ʈ ����
                g0bj.transform.SetPositionAndRotation(positionTospawm.position, positionTospawm.rotation);

                MeshRenderer mr = g0bj.AddComponent<MeshRenderer>();
                MeshFilter mf = g0bj.AddComponent<MeshFilter>();

                Mesh m = new Mesh(); //���� ĳ������ ��� �޽÷� ��ȯ
                SkinnedMeshRenderer[i].BakeMesh(m);
                mf.mesh = m;
                mr.material = mat;
                //����� ���̵� �ƿ� ȿ�� ����
                StartCoroutine(AnimatateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRetreshRate));

                Destroy(g0bj, meshDestoryDelay); //���� �ð� �� �ܻ� ����
            }
            //���� �ܻ� �������� ���
            yield return new WaitForSeconds(meshBefrcshRate);
        }
        //���� �ӵ��� �ִϸ��̼� �ӵ��� ����
        moveScript.movementSpeed = normalAnimSpeed;
        animator.SetFloat("animSpeed", normalAnimSpeed);
        isTrailActive = false;
    }


    //Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTrailActive) // �����̽��ٸ� ������ ���� �ܻ� ȿ���� ��Ȱ��ȭ�϶�
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime)); //�ܻ� ȿ�� �ڷ�ƾ ����
        }


    }


}