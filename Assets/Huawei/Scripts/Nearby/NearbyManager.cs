using HuaweiMobileServices.Nearby;
using HuaweiMobileServices.Nearby.Discovery;
using HuaweiMobileServices.Nearby.Transfer;
using System;
using UnityEngine;

namespace HmsPlugin
{

    public class NearbyManager : MonoBehaviour
    {
        readonly static String scanInfo = "testInfo", remoteEndpointId = "RemoteEndpointId",
                            receivedMessage = "Receive Success", transmittingMessage = "Receive Success";
        readonly private String myNameStr = "MyNameTest";
        readonly private String myServiceId = "NearbySnakeServiceid";
        public Action<string> OnDisconnected { get; set; }
        public Action<string, ConnectInfo> OnEstablish { get; set; }
        public Action<string, ConnectResult> OnResult { get; set; }
        public Action<string, ScanEndpointInfo> OnFound { get; set; }
        public Action<string> OnLost { get; set; }

        private void Start()
        {

        }
        private class NearbyManagerListener : IConnectCallBack, IScanEndpointCallback, IDataCallback
        {
            private readonly NearbyManager nearbyManagerObject;
            public NearbyManagerListener(NearbyManager nearbyManager)
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
            public void onReceived(string endpointId, AndroidJavaObject data)
            {
                Data dataReceived = new Data(data);
                if (dataReceived.DataType == Data.Type.BYTES)
                {
                    String msg = System.Text.Encoding.UTF8.GetString(dataReceived.AsBytes);
                    if (msg.Equals(receivedMessage))
                    {
                        Debug.Log("[HMS] NearbyManager Received ACK. Send next.");
                    }
                }
            }
            public void onTransferUpdate(string endpointId, AndroidJavaObject update)
            {
                Debug.Log("[HMS] NearbyManager onTransferUpdate");
            }

            public void onReceived(string endpointId, Data data)
            {
                throw new NotImplementedException();
            }

            public void onTransferUpdate(string endpointId, TransferStateUpdate update)
            {
                throw new NotImplementedException();
            }
        }


        //Starting Broadcasting
        public void SendFilesInner()
        {
            Action mOnFailureListener;

            // Obtain the endpoint information.
            String mEndpointName = "testName";//Android.os.Build.DEVICE;
            String mFileServiceId = "testID";
            // Select a broadcast policy.
            NearbyManagerListener nearbyManagerListener = new NearbyManagerListener(this);
            BroadcastOption advBuilder = new BroadcastOption.Builder().SetPolicy(Policy.POLICY_P2P).Build();

            // Start broadcasting.
            Nearby.DiscoveryEngine.StartBroadcasting(mEndpointName, mFileServiceId, nearbyManagerListener, advBuilder);
            Debug.Log("Nearby: Start Broadcasting");
        }
        //Starting Scanning

        public void OnScanResult()
        {
            String mFileServiceId = "yusuf";
            NearbyManagerListener nearbyManagerListener = new NearbyManagerListener(this);
            Debug.Log("Nearby: OnScanResult1 Start Scan");
            ScanOption scanBuilder = new ScanOption.Builder().SetPolicy(Policy.POLICY_P2P).Build(); ;
            Debug.Log("Nearby: OnScanResult2 Start Scan");
            // Start scanning.
            Nearby.DiscoveryEngine.StartScan(mFileServiceId, nearbyManagerListener, scanBuilder);
            Debug.Log("Nearby: OnScanResult3 Start Scan");
        }
        //Stopping Broadcasting
        public void StopBroadCasting()
        {
            Nearby.DiscoveryEngine.StopBroadcasting();

        }
        //Stopping Scanning
        public void StopScanning()
        {
            // Nearby.GetDiscoveryEngine(context).StopScan();
            HuaweiMobileServices.Nearby.Message.GetOption reportPolicy = HuaweiMobileServices.Nearby.Message.GetOption.DEFAULT;
            Debug.Log("Nearby: Start reportPolicy workes" + reportPolicy);


        }
        private void InitiateConnection(string endpointId, ScanEndpointInfo scanEndpointInfo)
        {
            if (scanEndpointInfo.Name.Equals(scanInfo))
            {
                Debug.Log("[HMS] Nearby Client found Server and request connection. Server id:" + scanEndpointInfo.Name);
                NearbyManagerListener nearbyManagerListener = new NearbyManagerListener(this);
                // Initiate a connection.
                Nearby.DiscoveryEngine.RequestConnect(myNameStr, endpointId, nearbyManagerListener);
            }
        }

        // Confirming the Connection
        // Accept the connection request
        private void AcceptConnectionRequest(string endpointId, ConnectInfo connectInfo)
        {
            NearbyManagerListener nearbyManagerListener = new NearbyManagerListener(this);
            Nearby.DiscoveryEngine.AcceptConnect(endpointId, nearbyManagerListener);
        }
        //Disconnecting from a Remote Endpoint
        public void DisconnectAllConnection()
        {
            Nearby.DiscoveryEngine.DisconnectAll();
        }
        //////////////////////////
        //   Data Transmission  //
        //////////////////////////
        //Transmitting Byte Arrays
        private void TransmittingBytes()
        {
            var message = System.Text.Encoding.UTF8.GetBytes(transmittingMessage);
            Nearby.TransferEngine.SendData(remoteEndpointId, Data.FromBytes(message));
        }
    }
}

