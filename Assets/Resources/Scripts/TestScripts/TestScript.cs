using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Test script for sample scene. </summary>
public class TestScript : MonoBehaviour
{
    private List<IDamageable> characters = new List<IDamageable>();
    private Camera cam;
    private Vector3 camPosition;

    private void Awake()
    {
        IDamageable[] damageables = FindObjectsOfType<BasicCharacter>() as IDamageable[];
        foreach (IDamageable damageable in damageables)
            characters.Add(damageable);
        cam = Camera.main;
        camPosition = cam.transform.position;
    }

    public void Attack()
    {
        foreach (IDamageable character in characters)
            character.GetHit(350);
    }

    public void Bomb()
    {
        foreach (IDamageable character in characters)
            character.GetBomb(1000);
    }

    public void Dodge()
    {
        foreach (IDamageable character in characters)
            character.GetDodge();
    }

    public void Heal()
    {
        foreach (IDamageable character in characters)
            character.GetHeal(500);
    }

    public void Revive()
    {
        BasicCharacter[] characters = FindObjectsOfType<BasicCharacter>();
        foreach(BasicCharacter character in characters)
        {
            character.gameObject.SetActive(false);
            character.gameObject.SetActive(true);
        }
    }

    public void Turn(Slider slider)
    {
        cam.transform.position = new Vector3((slider.value - 0.5f) * 10, cam.transform.position.y, cam.transform.position.z);
        cam.transform.LookAt(Vector3.zero);
    }

    public void Zoom(Slider slider)
    {
        cam.transform.position = camPosition - cam.transform.forward * slider.value * 10;
        cam.transform.LookAt(Vector3.zero);
    }

}
