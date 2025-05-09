using System.Collections;
using UnityEngine;
using TMPro;

public class NPCInsultCycler : MonoBehaviour
{
    public GameObject chatBox; // Canvas GameObject
    public TextMeshProUGUI insultText; // Text component inside the canvas
    public float insultInterval = 2.5f; // Time between insult changes

    private string[] insults = { "fopdoodle", "vile cur", "filthy heathen" };
    private Coroutine insultRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chatBox.SetActive(true);
            insultRoutine = StartCoroutine(CycleInsults());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (insultRoutine != null) StopCoroutine(insultRoutine);
            chatBox.SetActive(false);
        }
    }

    IEnumerator CycleInsults()
    {
        int index = 0;
        while (true)
        {
            insultText.text = insults[index];
            index = (index + 1) % insults.Length;
            yield return new WaitForSeconds(insultInterval);
        }
    }
}