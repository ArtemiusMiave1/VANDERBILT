
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class RouteManifestUI : MonoBehaviour
{
    [Header("Route References")]
    public ShipRouteSystem routeSystem;
    public ShipMovement shipMovement;

    [Header("Route Information")]
    public TMP_Text destinationsText;
    public TMP_Text distanceText;
    public TMP_Text fuelText;
    public TMP_Text dangerText;
    public TMP_Text routeListText;

    [Header("Delivery Information")]
    public TMP_Text deliveriesText;
    public TMP_Text resourcesText;

    [Header("Route Status")]
    public TMP_Text statusText;

    [Header("Fuel Estimate")]
    [Tooltip("Temporary estimate until connected to ShipMovement's fuel calculation.")]
    public float fuelPerDistance = 1f;

    [Header("Update Settings")]
    public bool updateContinuously = true;

    private void Start()
    {
        if (routeSystem == null)
            routeSystem = FindObjectOfType<ShipRouteSystem>();

        if (shipMovement == null)
            shipMovement = FindObjectOfType<ShipMovement>();

        UpdateManifest();
    }

    private void Update()
    {
        if (updateContinuously)
            UpdateManifest();
    }

    public void UpdateManifest()
    {
        if (routeSystem == null)
            return;

        List<Location> route = routeSystem.route;

        if (route == null || route.Count == 0)
        {
            DisplayEmptyRoute();
            return;
        }

        int destinations = Mathf.Max(0, route.Count - 1);

        float totalDistance = 0f;
        int dangerousSegments = 0;
        int blockedSegments = 0;

        StringBuilder routeList = new StringBuilder();

        for (int i = 0; i < route.Count; i++)
        {
            Location location = route[i];

            if (location == null)
                continue;

            routeList.Append(location.GetDisplayName());

            if (i < route.Count - 1)
                routeList.Append(" -> ");

            if (i == 0)
                continue;

            Location previousLocation = route[i - 1];

            if (previousLocation == null)
                continue;

            if (LocationManager.Instance == null)
                continue;

            RouteConnection connection =
                LocationManager.Instance.GetConnection(
                    previousLocation,
                    location
                );

            if (connection == null)
            {
                blockedSegments++;
                continue;
            }

            // Distance measured between location positions.
            totalDistance += Vector3.Distance(
                previousLocation.transform.position,
                location.transform.position
            );

            // Check for dangerous connections.
            if (connection.dangerLevel > 0)
                dangerousSegments++;

            if (connection.blocked)
                blockedSegments++;
        }

        float estimatedFuel = totalDistance * fuelPerDistance;

        // ROUTE INFORMATION

        if (destinationsText != null)
            destinationsText.text = "Destinations: " + destinations;

        if (distanceText != null)
            distanceText.text =
                "Distance: " + totalDistance.ToString("F1");

        if (fuelText != null)
            fuelText.text =
                "Fuel: " + estimatedFuel.ToString("F0");

        if (dangerText != null)
            dangerText.text =
                "Hazards: " + dangerousSegments;

        if (routeListText != null)
            routeListText.text = routeList.ToString();

        // DELIVERY INFORMATION
        // These will be connected to RequestPaper and Cargo.

        if (deliveriesText != null)
            deliveriesText.text = "Deliveries: --";

        if (resourcesText != null)
            resourcesText.text = "Resources: --";

        // ROUTE STATUS

        if (statusText != null)
        {
            if (blockedSegments > 0)
            {
                statusText.text = "Status: BLOCKED";
            }
            else if (destinations == 0)
            {
                statusText.text = "Status: NO DESTINATIONS";
            }
            else if (dangerousSegments > 0)
            {
                statusText.text = "Status: HAZARDOUS";
            }
            else
            {
                statusText.text = "Status: READY";
            }
        }
    }

    private void DisplayEmptyRoute()
    {
        if (destinationsText != null)
            destinationsText.text = "Destinations: 0";

        if (distanceText != null)
            distanceText.text = "Distance: 0";

        if (fuelText != null)
            fuelText.text = "Fuel: 0";

        if (dangerText != null)
            dangerText.text = "Hazards: 0";

        if (routeListText != null)
            routeListText.text = "No route charted";

        if (deliveriesText != null)
            deliveriesText.text = "Deliveries: --";

        if (resourcesText != null)
            resourcesText.text = "Resources: --";

        if (statusText != null)
            statusText.text = "Status: NO ROUTE";
    }
}
