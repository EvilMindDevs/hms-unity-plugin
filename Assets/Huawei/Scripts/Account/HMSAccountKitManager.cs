using HuaweiMobileServices.Base;
using HuaweiMobileServices.Drive;
using HuaweiMobileServices.Game;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSAccountKitManager : HMSManagerSingleton<HMSAccountKitManager>
    {
        private static AccountAuthService DefaultAuthService
        {
            get
            {
                Debug.Log("[HMS]: GET AUTH");
                var authParams = new AccountAuthParamsHelper(AccountAuthParams.DEFAULT_AUTH_REQUEST_PARAM).SetIdToken().SetAccessToken().CreateParams();
                Debug.Log("[HMS]: AUTHPARAMS AUTHSERVICE" + authParams);
                var result = AccountAuthManager.GetService(authParams);
                Debug.Log("[HMS]: RESULT AUTHSERVICE" + result);
                return result;
            }
        }
        private static AccountAuthService DefaultGameAuthService
        {
            get
            {
                IList<Scope> scopes = new List<Scope>();
                scopes.Add(GameScopes.DRIVE_APP_DATA);
                Debug.Log("[HMS]: GET AUTH GAME");
                var authParams = new AccountAuthParamsHelper(AccountAuthParams.DEFAULT_AUTH_REQUEST_PARAM_GAME).SetScopeList(scopes).CreateParams();
                Debug.Log("[HMS]: AUTHPARAMS GAME" + authParams);
                var result = AccountAuthManager.GetService(authParams);
                Debug.Log("[HMS]: RESULT GAME" + result);
                return result;
            }
        }

        private static AccountAuthService DefaultDriveAuthService
        {
            get
            {
                List<Scope> scopeList = new List<Scope>();
                scopeList.Add(new Scope(Drive.DriveScopes.SCOPE_DRIVE_FILE)); // Permissions to upload and store app data. 
                var authParams = new AccountAuthParamsHelper(AccountAuthParams.DEFAULT_AUTH_REQUEST_PARAM).SetAccessToken().SetIdToken().SetScopeList(scopeList).CreateParams();
                Debug.Log("[HMS]: AUTHPARAMS DRIVE" + authParams);
                var result = AccountAuthManager.GetService(authParams);
                Debug.Log("[HMS]: RESULT DRIVE" + result);
                return result;
            }
        }

        public AuthAccount HuaweiId { get; set; }
        public Action<AuthAccount> OnSignInSuccess { get; set; }
        public Action<HMSException> OnSignInFailed { get; set; }
        public Action<AuthAccount> OnIndependentSignInSuccess { get; set; }
        public Action<HMSException> OnIndependentSignInFailed { get; set; }
        public bool IsSignedIn { get { return HuaweiId != null; } }

        private AccountAuthService authService;

        public HMSAccountKitManager()
        {
            if (!HMSDispatcher.InstanceExists)
                HMSDispatcher.CreateDispatcher();
            HMSDispatcher.InvokeAsync(OnAwake);
        }       

        private void OnAwake()
        {
            Debug.Log("[HMSAccountManager]: AuthService OnAwake");
            authService = DefaultAuthService;
        }

        //Game Service authentication
        public AccountAuthService GetGameAuthService()
        {
            return DefaultGameAuthService;
        }

        public void SignIn()
        {
            Debug.Log("[HMSAccountManager]: Sign in " + authService);
            authService.StartSignIn((authId) =>
            {
                HuaweiId = authId;
                OnSignInSuccess?.Invoke(authId);

            }, (error) =>
            {
                HuaweiId = null;
                Debug.LogError("[HMSAccountManager]: Sign in failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);

                //Invoke the GMS Login when the HMS Core APK not installed Example
                        if (error.ErrorCode == AvailableCode.USER_ALREADY_KNOWS_SERVICE_UNAVAILABLE || 
                            error.ErrorCode == AvailableCode.CURRENT_SHOWING_SERVICE_UNAVAILABLE ||
                            error.ErrorCode == CommonCode.ErrorCode.CLIENT_API_INVALID) 
                        {
                             Debug.LogError("[HMSAccountManager]: Sign in failed. HMS Core APK not installed");
                             //GMS.signIn ... call GMS interface
                        }

                OnSignInFailed?.Invoke(error);
            });
        }

        public void SignInDrive(AccountAuthService authServiceDrivee)
        {
            Debug.Log("[HMS]: Sign in Drive " + authServiceDrivee);
            authServiceDrivee.StartSignIn((authId) =>
            {
                HuaweiId = authId;
                OnSignInSuccess?.Invoke(authId);
            }, (error) =>
            {
                HuaweiId = null;
                Debug.LogError("[HMSAccountManager]: Sign in Drive failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                OnSignInFailed?.Invoke(error);
            });
        }

        /*public void SignInDrive()
        {
            Debug.Log("[HMS]: Sign in Drive " + authServiceDrive);
            authServiceDrive.StartSignIn((authId) =>
            {
                HuaweiId = authId;
                OnSignInSuccess?.Invoke(authId);
            }, (error) =>
            {
                HuaweiId = null;
                Debug.LogError("[HMSAccountManager]: Sign in Drive failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                OnSignInFailed?.Invoke(error);
            });
        }*/

        public void SilentSignIn()
        {
            ITask<AuthAccount> taskAuthHuaweiId = authService.SilentSignIn();
            taskAuthHuaweiId.AddOnSuccessListener((result) =>
            {
                HuaweiId = result;
                OnSignInSuccess?.Invoke(result);
            }).AddOnFailureListener((exception) =>
            {
                HuaweiId = null;
                Debug.LogError("[HMSAccountManager]: Silent Sign in failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnSignInFailed?.Invoke(exception);
            });
        }

        public void SignOut()
        {
            authService.SignOut();
            HuaweiId = null;
        }

        public void CancelAuthorization()
        {
            ITask<HuaweiMobileServices.Utils.Void> taskAuthHuaweiId = authService.CancelAuthorization();
            taskAuthHuaweiId.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMSAccountManager]: CancelAuthorization onSuccess ");
            }).AddOnFailureListener((exception) =>
            {
                Debug.LogError("[HMSAccountManager]: Cancel Authorization failed. CauseMessage: " + exception.WrappedCauseMessage + ", ExceptionMessage: " + exception.WrappedExceptionMessage);
                OnSignInFailed?.Invoke(exception);
            });
        }

        // check this link for different scopes: https://developer.huawei.com/consumer/en/doc/development/HMSCore-Guides/independentsignin-0000001140395573
        public void IndependentSignIn(string accessToken)
        {
            AccountAuthParams authParams = new AccountAuthParamsHelper().SetEmail().CreateParams();
            AccountAuthService authService = AccountAuthManager.GetService(authParams);
            Debug.Log("[HMSAccountManager]: Independent Sign in ");
            authService.StartIndependentSignIn(accessToken,
            (success) =>
            {
                Debug.LogWarning("[HMSAccountManager]: Independent Sign in Success. Auth Code: " + success.AuthorizationCode);
                OnIndependentSignInSuccess?.Invoke(success);
            },
            (error) =>
            {
                Debug.LogError("[HMSAccountManager]: Independent Sign in failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                OnIndependentSignInFailed?.Invoke(error);
            });
        }
    }
}
