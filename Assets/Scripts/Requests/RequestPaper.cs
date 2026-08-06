using TMPro;
using UnityEngine;

public class RequestPaper : MonoBehaviour
{
    [Header("UI Text")]
    public TMP_Text titleText;
    public TMP_Text factionText;
    public TMP_Text resourceText;
    public TMP_Text rewardText;
    public TMP_Text dialogueText;


    [Header("Request Location")]
    public Location targetLocation;


    private RequestData request;


    public void DisplayRequest(RequestData data)
    {
        request = data;

        titleText.text = data.Title;
        factionText.text = "Faction: " + data.Faction;
        resourceText.text = "Resource: " + data.RequestedResources;
        rewardText.text = "Reward: " + data.Reward;
        dialogueText.text = data.Dialogue;
    }


    public void AssignLocation(Location location)
    {
        targetLocation = location;

        // Highlight destination
        targetLocation.Highlight();
    }


    public void OnShipArrived(Location location)
    {
        // Ignore if ship did not arrive at this request location
        if (location != targetLocation)
            return;


        CompleteRequest();
    }


    void CompleteRequest()
    {
        Debug.Log("Completed Request: " + request.Title);


        // Remove yellow highlight
        if (targetLocation != null)
        {
            targetLocation.ClearHighlight();
        }


        // Remove paper
        Destroy(gameObject);
    }
}