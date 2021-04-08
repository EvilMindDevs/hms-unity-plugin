using HmsPlugin;
using HuaweiMobileServices.Nearby;
using HuaweiMobileServices.Nearby.Discovery;
using HuaweiMobileServices.Nearby.Transfer;
using System;
using UnityEngine;

public class NearbyDemoManager : MonoBehaviour
{
    private HMSNearbyServiceManager nearbyManager;
    private NearbyManagerListener nearbyManagerListener;
    readonly private static String scanInfo = "testInfo", remoteEndpointId = "RemoteEndpointId", transmittingMessage = "Receive Success",
          myNameStr = "MyNameTest", mEndpointName = "testName", mFileServiceId = "testID";
    public Action<string> OnDisconnected { get; set; }
    public Action<string, ConnectInfo> OnEstablish { get; set; }
    public Action<string, ConnectResult> OnResult { get; set; }
    public Action<string, ScanEndpointInfo> OnFound { get; set; }
    public Action<string> OnLost { get; set; }

    void Start()
    {
        nearbyManager = HMSNearbyServiceManager.Instance;
        initilizeValues();
    }

    private void initilizeValues()
    {
        nearbyManager.scanInfo = scanInfo;
        nearbyManager.remoteEndpointId = remoteEndpointId;
        nearbyManager.transmittingMessage = transmittingMessage;
        nearbyManager.myNameStr = myNameStr;
        nearbyManager.mEndpointName = mEndpointName;
        nearbyManager.mFileServiceId = mFileServiceId;
    }

    public void SendFilesInner()
    {
        NearbyManagerListener connectCallback = new NearbyManagerListener(this);
        nearbyManager.SendFilesInner(nearbyManagerListener);
    }
    public void OnScanResult()
    {
        NearbyManagerListener scanCallback = new NearbyManagerListener(this);
        nearbyManager.OnScanResult(nearbyManagerListener);
    }
    public void StopBroadCasting()
    {
        nearbyManager.StopBroadCasting();
    }
    public void StopScanning()
    {
        nearbyManager.StopScanning();
    }
    public void DisconnectAllConnection()
    {
        nearbyManager.DisconnectAllConnection();
    }
    public void AcceptConnectionRequest(string endpointId, ConnectInfo connectInfo)
    {
        NearbyManagerListener callBack = new NearbyManagerListener(this);
        nearbyManager.AcceptConnectionRequest(endpointId, connectInfo, callBack);
    }
    public void InitiateConnection(string endpointId, ScanEndpointInfo scanEndpointInfo)
    {
        NearbyManagerListener callBack = new NearbyManagerListener(this);
        nearbyManager.InitiateConnection(endpointId, scanEndpointInfo, callBack);
    }
    public class NearbyManagerListener : IConnectCallBack, IScanEndpointCallback, IDataCallback
    {
        private readonly NearbyDemoManager nearbyManagerObject;
        private static string receivedMessage = "Receive Success";
        public NearbyManagerListener(NearbyDemoManager nearbyManager)
        {
            nearbyManagerObject = nearbyManager;
        }
        //ConnectCallBack
        public void onDisconnected(string p0)
        {
            Debug.Log("[HMS] NearbyManager onDisconnected");
            nearbyManagerObject.OnDisconnected?.Invoke(p0);
        }

        public void onEstablish(string endpointId, ConnectInfo connectionInfo)
        {
            // Authenticating the Connection
            Debug.Log("[HMS] NearbyManager onEstablish");

            nearbyManagerObject.AcceptConnectionRequest(endpointId, connectionInfo);
            nearbyManagerObject.OnEstablish?.Invoke(endpointId, connectionInfo);
        }

        public void onResult(string p0, ConnectResult resultObject)
        {
            Debug.Log("[HMS] NearbyManager onResult");

            if (resultObject.Status.StatusCode == StatusCode.STATUS_SUCCESS)
            {
                Debug.Log("[HMS] NearbyManager Connection Established. Stop discovery. Start to send file.");
            }
            nearbyManagerObject.OnResult?.Invoke(p0, resultObject);
        }
        //ScanEndpointCallback
        public void onFound(string endpointId, ScanEndpointInfo discoveryEndpointInfo)
        {
            Debug.Log("[HMS] NearbyManager OnFound");
            nearbyManagerObject.InitiateConnection(endpointId, discoveryEndpointInfo);
            nearbyManagerObject.OnFound?.Invoke(endpointId, discoveryEndpointInfo);
        }

        public void onLost(string endpointId)
        {
            Debug.Log("[HMS] NearbyManager OnLost");
            nearbyManagerObject.OnLost?.Invoke(endpointId);
        }

        // DataCallback
        public void onReceived(string endpointId, Data dataReceived)
        {
            if (dataReceived.DataType == Data.Type.BYTES)
            {
                string msg = System.Text.Encoding.UTF8.GetString(dataReceived.AsBytes);
                if (msg.Equals(receivedMessage))
                {
                    Debug.Log("[HMS] NearbyManager Received ACK. Send next.");
                }
            }
        }
        public void onTransferUpdate(string endpointId, TransferStateUpdate update)
        {
            Debug.Log("[HMS] NearbyManager onTransferUpdate");
        }
    }
}

