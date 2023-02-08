using HuaweiMobileServices.AuthService;
using HuaweiMobileServices.CloudDB;
using HuaweiMobileServices.Common;
using HuaweiMobileServices.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;
namespace HmsPlugin
{
    public class HMSCloudDBManager : HMSManagerSingleton<HMSCloudDBManager>
    {
        string TAG = "HMSCloudDBManager";

        AGConnectCloudDB mCloudDB = null;
        CloudDBZoneConfig mConfig = null;
        CloudDBZone mCloudDBZone = null;
        ListenerHandler mRegister = null;

        public CloudDBZone MCloudDBZone { get => mCloudDBZone; set => mCloudDBZone = value; }
        public ListenerHandler MRegister { get => mRegister; set => mRegister = value; }

        public bool IsCloudDBActive
        {
            get
            {
                if (mCloudDBZone == null)
                {
                    Debug.Log($"[{TAG}]: CloudDBZone is null, try re-open it");
                    return false;
                }

                return true;
            }
        }

        public Action<CloudDBZone> OnOpenCloudDBZone2Success { get; set; }
        public Action<HMSException> OnOpenCloudDBZone2Failed { get; set; }

        public Action<int> OnExecuteUpsertSuccess { get; set; }
        public Action<HMSException> OnExecuteUpsertFailed { get; set; }

        public Action<int> OnExecuteDeleteSuccess { get; set; }
        public Action<HMSException> OnExecuteDeleteFailed { get; set; }

        public Action<double> OnExecuteAverageQuerySuccess { get; set; }
        public Action<HMSException> OnExecuteAverageQueryFailed { get; set; }

        public Action<int> OnExecuteSumQuerySuccess { get; set; }
        public Action<HMSException> OnExecuteSumQueryFailed { get; set; }

        public Action<int> OnExecuteMaximumQuerySuccess { get; set; }
        public Action<HMSException> OnExecuteMaximumQueryFailed { get; set; }

        public Action<int> OnExecuteMinimalQuerySuccess { get; set; }
        public Action<HMSException> OnExecuteMinimalQueryFailed { get; set; }

        public Action<long> OnExecuteCountQuerySuccess { get; set; }
        public Action<HMSException> OnExecuteCountQueryFailed { get; set; }

        public void Initialize()
        {
            AGConnectCloudDB.Initialize();
            Debug.Log($"[{TAG}]: Initialize()");
        }

        public void GetInstance(AGConnectInstance instance, AGConnectAuth auth)
        {
            if (mCloudDB == null)
                mCloudDB = AGConnectCloudDB.GetInstance(instance, auth);
            Debug.Log($"[{TAG}]: GetInstance() ");
        }

        public void CreateObjectType(string ObjectTypeInfoHelperPath)
        {
            mCloudDB.CreateObjectType(ObjectTypeInfoHelper.GetObjectTypeInfo(ObjectTypeInfoHelperPath));
            Debug.Log($"[{TAG}]: CreateObjectType()");
        }

        public IList<CloudDBZoneConfig> GetCloudDBZoneConfigs()
        {
            return mCloudDB.GetCloudDBZoneConfigs();
        }

        public void OpenCloudDBZone(string CloudDBZoneName, CloudDBZoneConfig.CloudDBZoneSyncProperty CloudDBZoneSyncProperty, CloudDBZoneConfig.CloudDBZoneAccessProperty CloudDBZoneAccessProperty)
        {
            mConfig = new CloudDBZoneConfig(CloudDBZoneName, CloudDBZoneSyncProperty, CloudDBZoneAccessProperty);
            mConfig.PersistenceEnabled = true;

            try
            {
                mCloudDBZone = mCloudDB.OpenCloudDBZone(mConfig, true);
            }
            catch (Exception e)
            {
                Debug.LogError($"[{TAG}]: CloudDBZoneConfig() exception " + e.Message);
            }

            Debug.Log($"[{TAG}]: OpenCloudDBZone");
        }

        public void OpenCloudDBZone2(string CloudDBZoneName, CloudDBZoneConfig.CloudDBZoneSyncProperty CloudDBZoneSyncProperty, CloudDBZoneConfig.CloudDBZoneAccessProperty CloudDBZoneAccessProperty)
        {
            mConfig = new CloudDBZoneConfig(CloudDBZoneName, CloudDBZoneSyncProperty, CloudDBZoneAccessProperty);
            mConfig.PersistenceEnabled = true;

            mCloudDB.OpenCloudDBZone2(mConfig, true)
                .AddOnSuccessListener(cloudDBZone =>
                {
                    mCloudDBZone = cloudDBZone;
                    OnOpenCloudDBZone2Success?.Invoke(cloudDBZone);
                    Debug.Log($"[{TAG}]: mCloudDB.OpenCloudDBZone2 success ");
                }).AddOnFailureListener(exception =>
                {
                    OnOpenCloudDBZone2Failed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDB.OpenCloudDBZone2 error " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage);
                });
        }

        public void CloseCloudDBZone(CloudDBZone zone)
        {
            mCloudDB.CloseCloudDBZone(zone);
            Debug.Log($"[{TAG}]: CloseCloudDBZone()");
        }

        public void DeleteCloudDBZone(string zoneName)
        {
            mCloudDB.DeleteCloudDBZone(zoneName);
            Debug.Log($"[{TAG}]: DeleteCloudDBZone()");
        }

        public void EnableNetwork(string zoneName)
        {
            mCloudDB.EnableNetwork(zoneName);
            Debug.Log($"[{TAG}]: EnableNetwork()");
        }

        public void DisableNetwork(string zoneName)
        {
            mCloudDB.DisableNetwork(zoneName);
            Debug.Log($"[{TAG}]: DisableNetwork()");
        }

        public void SetUserKey(string userKey, string userReKey)
        {
            mCloudDB.SetUserKey(userKey, userReKey, false).AddOnSuccessListener(result => { }).AddOnFailureListener(error => { });
        }

        public void UpdateDataEncryptionKey()
        {
            mCloudDB.UpdateDataEncryptionKey().AddOnSuccessListener(result => { }).AddOnFailureListener(error => { });
        }

        public void ExecuteUpsert(ICloudDBZoneObject cloudDBZoneObject)
        {
            if (!IsCloudDBActive)
                return;

            mCloudDBZone.ExecuteUpsert(cloudDBZoneObject)
                .AddOnSuccessListener(cloudDBZoneResult =>
                {
                    OnExecuteUpsertSuccess?.Invoke(cloudDBZoneResult);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteUpsert AddOnSuccessListener " + cloudDBZoneResult);
                })
                .AddOnFailureListener(exception =>
                {
                    OnExecuteUpsertFailed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteUpsert AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");
                });
        }

        public void ExecuteUpsert(IList<AndroidJavaObject> obj)
        {
            if (!IsCloudDBActive)
                return;

            mCloudDBZone.ExecuteUpsert(obj)
                .AddOnSuccessListener(result =>
                {
                    OnExecuteUpsertSuccess?.Invoke(result);
                })
                .AddOnFailureListener(exception =>
                {
                    OnExecuteUpsertFailed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteUpsert AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");
                });
        }

        public void ExecuteDelete(ICloudDBZoneObject cloudDBZoneObject)
        {
            if (!IsCloudDBActive)
                return;

            mCloudDBZone.ExecuteDelete(cloudDBZoneObject)
                .AddOnSuccessListener(cloudDBZoneResult =>
                {
                    OnExecuteDeleteSuccess?.Invoke(cloudDBZoneResult);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteDelete AddOnSuccessListener " + cloudDBZoneResult);
                })
                .AddOnFailureListener(exception =>
                {
                    OnExecuteDeleteFailed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteDelete AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");
                });
        }

        public void ExecuteDelete(IList<AndroidJavaObject> obj)
        {
            if (!IsCloudDBActive)
                return;

            mCloudDBZone.ExecuteDelete(obj)
                .AddOnSuccessListener(result =>
                {
                    OnExecuteDeleteSuccess?.Invoke(result);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteDelete AddOnSuccessListener " + result);
                })
                .AddOnFailureListener(exception =>
                {
                    OnExecuteDeleteFailed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteDelete AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");
                });
        }

        public void ExecuteAverageQuery(CloudDBZoneQuery query, string fieldName, CloudDBZoneQuery.CloudDBZoneQueryPolicy CloudDBZoneQueryPolicy)
        {
            if (!IsCloudDBActive)
                return;

            mCloudDBZone.ExecuteCountQuery(query, fieldName, CloudDBZoneQueryPolicy)
                .AddOnSuccessListener(result =>
                {
                    OnExecuteCountQuerySuccess?.Invoke(result);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteAverageQuery AddOnSuccessListener " + result);
                })
                .AddOnFailureListener(exception =>
                {
                    OnExecuteCountQueryFailed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteAverageQuery AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");
                });
        }

        public void ExecuteSumQuery(CloudDBZoneQuery query, string fieldName, CloudDBZoneQuery.CloudDBZoneQueryPolicy CloudDBZoneQueryPolicy)
        {
            if (!IsCloudDBActive)
                return;

            mCloudDBZone.ExecuteSumQuery(query, fieldName, CloudDBZoneQueryPolicy)
                .AddOnSuccessListener(result =>
                {
                    OnExecuteSumQuerySuccess?.Invoke(result);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteSumQuery AddOnSuccessListener " + result);
                })
                .AddOnFailureListener(exception =>
                {
                    OnExecuteSumQueryFailed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteSumQuery AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");
                });
        }

        public void ExecuteMaximumQuery(CloudDBZoneQuery query, string fieldName, CloudDBZoneQuery.CloudDBZoneQueryPolicy CloudDBZoneQueryPolicy)
        {
            if (!IsCloudDBActive)
                return;

            mCloudDBZone.ExecuteMaximumQuery(query, fieldName, CloudDBZoneQueryPolicy)
                .AddOnSuccessListener(result =>
                {
                    OnExecuteMaximumQuerySuccess?.Invoke(result);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteMaximumQuery AddOnSuccessListener " + result);
                })
                .AddOnFailureListener(exception =>
                {
                    OnExecuteMaximumQueryFailed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteMaximumQuery AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");
                });
        }

        public void ExecuteMinimalQuery(CloudDBZoneQuery query, string fieldName, CloudDBZoneQuery.CloudDBZoneQueryPolicy CloudDBZoneQueryPolicy)
        {
            if (!IsCloudDBActive)
                return;

            mCloudDBZone.ExecuteMinimalQuery(query, fieldName, CloudDBZoneQueryPolicy)
                .AddOnSuccessListener(result =>
                {
                    OnExecuteMinimalQuerySuccess?.Invoke(result);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteMinimalQuery AddOnSuccessListener " + result);
                })
                .AddOnFailureListener(exception =>
                {
                    OnExecuteMinimalQueryFailed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteMinimalQuery AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");
                });
        }

        public void ExecuteCountQuery(CloudDBZoneQuery query, string fieldName, CloudDBZoneQuery.CloudDBZoneQueryPolicy CloudDBZoneQueryPolicy)
        {
            if (!IsCloudDBActive)
                return;

            mCloudDBZone.ExecuteCountQuery(query, fieldName, CloudDBZoneQueryPolicy)
                .AddOnSuccessListener(result =>
                {
                    OnExecuteCountQuerySuccess?.Invoke(result);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteCountQuery AddOnSuccessListener " + result);
                })
                .AddOnFailureListener(exception =>
                {
                    OnExecuteCountQueryFailed?.Invoke(exception);
                    Debug.Log($"[{TAG}]: mCloudDBZone.ExecuteCountQuery AddOnFailureListener " +
                        exception.WrappedCauseMessage + " - " +
                        exception.WrappedExceptionMessage + " - ");
                });
        }

    }
}