using System.Text;
using HuaweiMobileServices.AuthService;
using HuaweiMobileServices.Location.Geofence.Geofences;
using HuaweiMobileServices.Location.Location;
using UnityEngine;
using UnityEngine.UI;

public class GeocodingDemo : MonoBehaviour
{
    private static string TAG = "GeocodingDemo";
    private GeocoderService geocoderService;
    [SerializeField] private Text resultText;

    public void Start()
    {
        Locale locale = new Locale("tr", "TR");
        geocoderService = LocationServices.GetGeocoderService(locale);
    }

    public void ObtainReverseGeocoding()
    {
        Debug.Log($"{TAG} ObtainReverseGeocoding called");

        GetFromLocationRequest getFromLocationRequest = new GetFromLocationRequest(40.9816806, 29.1229218, 5);
        StringBuilder stringBuilder = new StringBuilder();

        geocoderService.GetFromLocation(getFromLocationRequest)
            .AddOnSuccessListener(
                (hwLocationList) =>
                {
                    Debug.Log($"{TAG} geocoderService GetFromLocation success count is {hwLocationList.Count}");
                    foreach (var hwLocation in hwLocationList)
                    {
                        stringBuilder.Append("\nLatitude = " + hwLocation.GetLatitude())
                            .Append("\nLongitude = " + hwLocation.GetLongitude())
                            .Append(hwLocation.GetCountryName())
                            .Append("\ncountryCode=")
                            .Append(hwLocation.GetCountryCode())
                            .Append("\nstate=")
                            .Append(hwLocation.GetState())
                            .Append("\ncity=")
                            .Append(hwLocation.GetCity())
                            .Append("\ncounty=")
                            .Append(hwLocation.GetCounty())
                            .Append("\nstreet=")
                            .Append(hwLocation.GetStreet())
                            .Append("\nfeatureName=")
                            .Append(hwLocation.GetFeatureName())
                            .Append("\npostalCode=")
                            .Append(hwLocation.GetPostalCode())
                            .Append("\nphone=")
                            .Append(hwLocation.GetPhone())
                            .Append("\nurl=");
                    }

                    resultText.text = stringBuilder.ToString();
                })
            .AddOnFailureListener((exception) =>
            {
                Debug.LogError($"{TAG} geocoderService GetFromLocation Fail" + exception.WrappedCauseMessage + " " +
                               exception.WrappedExceptionMessage + $"{TAG} RequestLocationUpdates Error code: " +
                               exception.ErrorCode);
            });
    }

    public void ObtainForwardGeocoding()
    {
        GetFromLocationNameRequest getFromLocationNameRequest =
            new GetFromLocationNameRequest(
                "Changjiang Community, Huannan Road, Binjiang District, Hangzhou City, Zhejiang Province", 5, 30.077365,
                120.180277, 30.181908, 120.187648);
        StringBuilder stringBuilder = new StringBuilder();

        geocoderService.GetFromLocationName(getFromLocationNameRequest)
            .AddOnSuccessListener(
                (hwLocationList) =>
                {
                    Debug.Log($"{TAG} geocoderService GetFromLocationName success count is {hwLocationList.Count}");
                    foreach (var hwLocation in hwLocationList)
                    {
                        stringBuilder.Append("\nLatitude = " + hwLocation.GetLatitude())
                            .Append("\nLongitude = " + hwLocation.GetLongitude());
                    }

                    resultText.text = stringBuilder.ToString();
                })
            .AddOnFailureListener((exception) =>
            {
                Debug.LogError($"{TAG} geocoderService GetFromLocation Fail" + exception.WrappedCauseMessage + " " +
                               exception.WrappedExceptionMessage + $"{TAG} RequestLocationUpdates Error code: " +
                               exception.ErrorCode);
            });
    }
}