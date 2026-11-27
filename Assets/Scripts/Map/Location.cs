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

    private Material runtimeMaterial;


    // --------------------------------------------------
    // AWAKE
    // --------------------------------------------------

    private void Awake()
    {
        if (iconPlane == null)
        {
            iconPlane = GetComponent<Renderer>();
        }
    }


    // --------------------------------------------------
    // START
    // --------------------------------------------------

    private void Start()
    {
        RefreshVisuals();
    }


    // --------------------------------------------------
    // REFRESH VISUALS
    // --------------------------------------------------

    [ContextMenu("Refresh Visuals")]
    public void RefreshVisuals()
    {
        if (locationType == null)
            return;

        // Use the GameObject name if no ID was assigned.
        if (string.IsNullOrEmpty(locationID))
        {
            locationID = gameObject.name;
        }

        SetLocationVisual();
    }


    // --------------------------------------------------
    // SET LOCATION DATA
    // --------------------------------------------------

    public void SetLocationType(LocationData type)
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

    public void SetLocationID(string newID)
    {
        locationID = newID;

        gameObject.name = locationID;

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
            return;

        if (locationType.District)
        {
            SetDistrictVisual();
        }
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
        // Show the district text.
        if (districtText != null)
        {
            districtText.gameObject.SetActive(true);
            districtText.enabled = true;

            districtText.text = locationID;
        }

        // Hide only the Renderer.
        // Do not disable the entire location.
        if (iconPlane != null)
        {
            iconPlane.enabled = false;
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
            districtText.enabled = false;
        }

        if (iconPlane == null)
        {
            Debug.LogError(
                gameObject.name +
                " has no Icon Plane Renderer assigned."
            );

            return;
        }

        // Enable the icon plane if it is a child object.
        if (iconPlane.gameObject != gameObject)
        {
            iconPlane.gameObject.SetActive(true);
        }

        iconPlane.enabled = true;


        // ----------------------------------------------
        // LOAD ICON
        // ----------------------------------------------

        string iconName = locationType.LocationType;

        Texture2D icon = Resources.Load<Texture2D>(
            "Icons/" + iconName
        );

        if (icon == null)
        {
            Debug.LogError(
                "Could not find location icon: " +
                "Resources/Icons/" + iconName
            );

            return;
        }


        // ----------------------------------------------
        // SET MATERIAL
        // ----------------------------------------------

        if (Application.isPlaying)
        {
            Material material = GetRuntimeMaterial();

            if (material == null)
                return;

            material.mainTexture = icon;
        }
        else
        {
            // Preview the texture without modifying
            // the original material asset.
            MaterialPropertyBlock properties =
                new MaterialPropertyBlock();

            iconPlane.GetPropertyBlock(properties);

            properties.SetTexture("_MainTex", icon);
            properties.SetTexture("_BaseMap", icon);

            iconPlane.SetPropertyBlock(properties);
        }
    }


    // --------------------------------------------------
    // GET RUNTIME MATERIAL
    // --------------------------------------------------

    private Material GetRuntimeMaterial()
    {
        if (iconPlane == null)
            return null;

        if (runtimeMaterial == null)
        {
            Material sourceMaterial =
                locationMaterial != null
                ? locationMaterial
                : iconPlane.sharedMaterial;

            if (sourceMaterial == null)
            {
                Debug.LogError(
                    gameObject.name +
                    " has no material for the icon plane."
                );

                return null;
            }

            runtimeMaterial = new Material(sourceMaterial);

            iconPlane.material = runtimeMaterial;
        }

        return runtimeMaterial;
    }


    // --------------------------------------------------
    // HIGHLIGHT
    // --------------------------------------------------

    public void Highlight()
    {
        if (iconPlane != null && !IsDistrict())
        {
            Material material = GetRuntimeMaterial();

            if (material != null)
                material.color = Color.yellow;
        }

        if (districtText != null)
        {
            districtText.color = Color.yellow;
        }
    }


    // --------------------------------------------------
    // CLEAR HIGHLIGHT
    // --------------------------------------------------

    public void ClearHighlight()
    {
        if (iconPlane != null && !IsDistrict())
        {
            Material material = GetRuntimeMaterial();

            if (material != null)
                material.color = Color.white;
        }

        if (districtText != null)
        {
            districtText.color = Color.white;
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


    // --------------------------------------------------
    // CLEANUP
    // --------------------------------------------------

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }
}