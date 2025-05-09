using UnityEngine;
using TMPro;

public class BobInsultChat : MonoBehaviour
{
    public GameObject chatBox; // BobChatCanvas
    public TextMeshProUGUI insultText; // InsultText TMP component
    public Transform player; // XR Camera or Player
    public float triggerDistance = 3f; // How close the player needs to be
    public float insultInterval = 2f; // Time between insults

    private string[] insults = { "Fopdoodle", "Vile cur", "Filthy heathen" };
    private int currentInsult = 0;
    private float timer = 0f;

    void Start()
    {
        chatBox.SetActive(false);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < triggerDistance)
        {
            chatBox.SetActive(true);
            timer += Time.deltaTime;

            if (timer >= insultInterval)
            {
                insultText.text = insults[currentInsult];
                currentInsult = (currentInsult + 1) % insults.Length;
                timer = 0f;
            }

            // Optional: face chat box toward player
            chatBox.transform.LookAt(player);
            chatBox.transform.Rotate(0, 180f, 0); // So text isn't backward
        }
        else
        {
            chatBox.SetActive(false);
            timer = 0f; // Reset timer when player leaves
        }
    }
}
