using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Location : MonoBehaviour
{
    [Header("Connections")]
    public List<RouteConnection> connections =
        new List<RouteConnection>();

    [Header("Requests")]
    public List<RequestPaper> activeRequests =
        new List<RequestPaper>();

    [Header("Location Data")]
    public LocationData locationType;

    [Header("Generated Map ID")]
    [Tooltip("Examples: A1, A2, B1, F3, ResourceDepot, Vanderbilt")]
    public string locationID;

    [Header("District Visual")]
    [Tooltip("TextMeshPro used to display A1, B2, F3, etc.")]
    public TMP_Text districtText;

    [Header("Non-District Visual")]
    [Tooltip("Plane/Renderer used for ResourceDepot, Vanderbilt, etc.")]
    public Renderer iconPlane;

    [Header("Material")]
    public Material locationMaterial;


    // --------------------------------------------------
    // AWAKE
    // --------------------------------------------------

    private void Awake()
    {
        // If no icon plane was assigned,
        // try to find a Renderer on this object.
        if (iconPlane == null)
        {
            iconPlane =
                GetComponent<Renderer>();
        }
    }


    // --------------------------------------------------
    // SET LOCATION DATA
    // --------------------------------------------------

    public void SetLocationType(
        LocationData type)
    {
        locationType = type;

        if (locationType == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " was given a null LocationData."
            );

            return;
        }

        SetLocationVisual();
    }


    // --------------------------------------------------
    // SET LOCATION ID
    // --------------------------------------------------

    public void SetLocationID(
        string newID)
    {
        locationID = newID;

        // Make the GameObject name match
        // the generated location ID.
        gameObject.name = locationID;

        // Update visual after the ID has been generated.
        SetLocationVisual();
    }


    // --------------------------------------------------
    // GET LOCATION ID
    // --------------------------------------------------

    public string GetLocationID()
    {
        return locationID;
    }


    // --------------------------------------------------
    // GET LOCATION TYPE
    // --------------------------------------------------

    public string GetLocationType()
    {
        if (locationType == null)
            return "";

        return locationType.LocationType;
    }


    // --------------------------------------------------
    // GET DISTRICT TYPE
    // --------------------------------------------------

    public string GetDistrictType()
    {
        if (locationType == null)
            return "";

        return locationType.DistrictType;
    }


    // --------------------------------------------------
    // IS DISTRICT
    // --------------------------------------------------

    public bool IsDistrict()
    {
        if (locationType == null)
            return false;

        return locationType.District;
    }


    // --------------------------------------------------
    // SET LOCATION VISUAL
    // --------------------------------------------------

    private void SetLocationVisual()
    {
        if (locationType == null)
        {
            return;
        }


        // ----------------------------------------------
        // DISTRICT LOCATION
        // ----------------------------------------------

        if (locationType.District)
        {
            SetDistrictVisual();
        }


        // ----------------------------------------------
        // NON-DISTRICT LOCATION
        // ----------------------------------------------

        else
        {
            SetNonDistrictVisual();
        }
    }


    // --------------------------------------------------
    // DISTRICT VISUAL
    // --------------------------------------------------

    private void SetDistrictVisual()
    {
        // Show the TextMeshPro.
        if (districtText != null)
        {
            districtText.gameObject.SetActive(true);

            districtText.text =
                locationID;
        }


        // Hide the icon plane.
        if (iconPlane != null)
        {
            iconPlane.gameObject.SetActive(false);
        }
    }


    // --------------------------------------------------
    // NON-DISTRICT VISUAL
    // --------------------------------------------------

    private void SetNonDistrictVisual()
    {
        // Hide the district text.
        if (districtText != null)
        {
            districtText.gameObject.SetActive(false);
        }


        // Show the icon plane.
        if (iconPlane == null)
        {
            Debug.LogError(
                gameObject.name +
                " has no Icon Plane Renderer assigned."
            );

            return;
        }


        iconPlane.gameObject.SetActive(true);


        // ----------------------------------------------
        // LOAD ICON
        // ----------------------------------------------

        string iconName =
            locationType.LocationType;


        Texture2D icon =
            Resources.Load<Texture2D>(
                "Icons/" + iconName
            );


        if (icon == null)
        {
            Debug.LogError(
                "Could not find location icon: " +
                "Resources/Icons/" +
                iconName
            );

            return;
        }


        // ----------------------------------------------
        // GET MATERIAL
        // ----------------------------------------------

        if (locationMaterial == null)
        {
            locationMaterial =
                iconPlane.material;
        }


        if (locationMaterial == null)
        {
            Debug.LogError(
                gameObject.name +
                " has no material for the icon plane."
            );

            return;
        }


        // ----------------------------------------------
        // SET ICON
        // ----------------------------------------------

        locationMaterial.mainTexture =
            icon;
    }


    // --------------------------------------------------
    // HIGHLIGHT
    // --------------------------------------------------

    public void Highlight()
    {
        if (iconPlane != null)
        {
            iconPlane.material.color =
                Color.yellow;
        }

        if (districtText != null)
        {
            districtText.color =
                Color.yellow;
        }
    }


    // --------------------------------------------------
    // CLEAR HIGHLIGHT
    // --------------------------------------------------

    public void ClearHighlight()
    {
        if (iconPlane != null)
        {
            iconPlane.material.color =
                Color.white;
        }

        if (districtText != null)
        {
            districtText.color =
                Color.white;
        }
    }


    // --------------------------------------------------
    // GET DISPLAY NAME
    // --------------------------------------------------

    public string GetDisplayName()
    {
        if (string.IsNullOrEmpty(locationID))
        {
            if (locationType != null)
            {
                return locationType.LocationType;
            }

            return gameObject.name;
        }

        return locationID;
    }
}