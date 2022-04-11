using HuaweiMobileServices.Nearby;
using HuaweiMobileServices.Nearby.Discovery;
using HuaweiMobileServices.Nearby.Transfer;
using System;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSNearbyServiceManager : HMSManagerSingleton<HMSNearbyServiceManager>
    {
        public string scanInfo, remoteEndpointId, transmittingMessage, myNameStr, mEndpointName, mFileServiceId;

        //Starting Broadcasting
        public void SendFilesInner(IConnectCallBack connectCallBack)
        {
            Action mOnFailureListener;
            // Obtain the endpoint information.
            // Select a broadcast policy.
            BroadcastOption advBuilder = new BroadcastOption.Builder().SetPolicy(Policy.POLICY_P2P).Build();

            // Start broadcasting.
            Nearby.DiscoveryEngine.StartBroadcasting(mEndpointName, mFileServiceId, connectCallBack, advBuilder);
            Debug.Log("Nearby: Start Broadcasting");
        }

        //Starting Scanning
        public void OnScanResult(IScanEndpointCallback scanEndpointCallback)
        {
            Debug.Log("Nearby: OnScanResult1 Start Scan");
            ScanOption scanBuilder = new ScanOption.Builder().SetPolicy(Policy.POLICY_P2P).Build(); ;
            // Start scanning.
            Nearby.DiscoveryEngine.StartScan(mFileServiceId, scanEndpointCallback, scanBuilder);
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
        public void InitiateConnection(string endpointId, ScanEndpointInfo scanEndpointInfo, IConnectCallBack connectCallBack)
        {
            if (scanEndpointInfo.Name.Equals(scanInfo))
            {
                Debug.Log("[HMS] Nearby Client found Server and request connection. Server id:" + scanEndpointInfo.Name);
                // Initiate a connection.
                Nearby.DiscoveryEngine.RequestConnect(myNameStr, endpointId, connectCallBack);
            }
        }

        // Confirming the Connection
        // Accept the connection request
        public void AcceptConnectionRequest(string endpointId, ConnectInfo connectInfo, IDataCallback dataCallback)
        {
            Debug.Log("[HMS] Accept Connection Request, Endpoint Name:" + connectInfo.EndpointName);
            Nearby.DiscoveryEngine.AcceptConnect(endpointId, dataCallback);
        }

        //Disconnecting from a Remote Endpoint
        public void DisconnectAllConnection()
        {
            Nearby.DiscoveryEngine.DisconnectAll();
        }

        //Transmitting Byte Arrays
        private void TransmittingBytes()
        {
            var message = System.Text.Encoding.UTF8.GetBytes(transmittingMessage);
            Nearby.TransferEngine.SendData(remoteEndpointId, Data.FromBytes(message));
        }
    }
}

