using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public GameObject[] NumberPrefabs;

    public void CreateNumber(int number, int numberOfCharacters)
    {
        int xPos = 0;
        var numberString = number.ToString();

        int diff = numberOfCharacters - numberString.Length;

        for(int i = 0; i < diff; i++)
        {
            var prefab = NumberPrefabs[0];
            var instance = Instantiate(prefab, new Vector3(xPos, 0, 0), transform.rotation);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = new Vector3(xPos, 0, 0); 
            xPos += 1;
        } 

        foreach(var character in numberString)
        {
            var prefab = NumberPrefabs[int.Parse(character.ToString())];
            var instance = Instantiate(prefab, new Vector3(xPos, 0, 0), transform.rotation);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = new Vector3(xPos, 0, 0); 
            xPos += 1;
        }
    }
}
