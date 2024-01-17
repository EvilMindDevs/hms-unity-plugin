using HuaweiMobileServices.AuthService;
using HuaweiMobileServices.Utils;
using System;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSAuthServiceManager : HMSManagerSingleton<HMSAuthServiceManager>
    {
        public const string TAG = "[HMS AuthServiceManager]";
        public Action<SignInResult> OnSignInSuccess { get; set; }
        public Action<HMSException> OnSignInFailed { get; set; }

        public Action<SignInResult> OnCreateUserSuccess { get; set; }
        public Action<HMSException> OnCreateUserFailed { get; set; }

        public Action<bool> OnResetPasswordSuccess { get; set; }
        public Action<HMSException> OnResetPasswordFailed { get; set; }

        AGConnectAuth _AGConnectAuth = null;

        public HMSAuthServiceManager()
        {
            HMSManagerStart.Start(OnAwake, TAG);
        }

        private void OnAwake()
        {
            Debug.Log($"{TAG}: AuthService OnAwake");
            _AGConnectAuth = AGConnectAuth.GetInstance();
        }

        public void SignIn(AGConnectAuthCredential paramAGConnectAuthCredential)
        {
            if (_AGConnectAuth == null) return;
            _AGConnectAuth.SignIn(paramAGConnectAuthCredential)
                .AddOnSuccessListener((signInResult) =>
                {
                    OnSignInSuccess?.Invoke(signInResult);
                })
                .AddOnFailureListener((error) =>
                {
                    Debug.LogError($"{TAG}: Sign in failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnSignInFailed?.Invoke(error);
                });
        }

        public void SignInAnonymously()
        {
            if (_AGConnectAuth == null) return;
            _AGConnectAuth.SignInAnonymously()
                .AddOnSuccessListener((signInResult) =>
                {
                    OnSignInSuccess?.Invoke(signInResult);
                })
                .AddOnFailureListener((error) =>
                {
                    Debug.LogError($"{TAG}: Sign in Anonymously failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnSignInFailed?.Invoke(error);
                });
        }

        public void SignOut()
        {
            if (_AGConnectAuth == null) return;
            _AGConnectAuth.SignOut();
        }

        public void DeleteUser()
        {
            if (_AGConnectAuth == null) return;
            _AGConnectAuth.DeleteUser();
        }

        public AGConnectUser GetCurrentUser()
        {
            if (_AGConnectAuth == null) return null;
            return _AGConnectAuth.GetCurrentUser();
        }

        public void CreateUser(EmailUser emailUser)
        {
            if (_AGConnectAuth == null) return;
            _AGConnectAuth.CreateUser(emailUser)
                .AddOnSuccessListener(signInResult =>
                {
                    OnCreateUserSuccess?.Invoke(signInResult);
                })
                .AddOnFailureListener(error =>
                {
                    Debug.LogError($"{TAG}: Create User failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnCreateUserFailed?.Invoke(error);
                });
        }

        public void CreateUser(PhoneUser phoneUser)
        {
            if (_AGConnectAuth == null) return;

            _AGConnectAuth.CreateUser(phoneUser)
                .AddOnSuccessListener(signInResult =>
                {
                    OnCreateUserSuccess?.Invoke(signInResult);
                })
                .AddOnFailureListener(error =>
                {
                    Debug.LogError($"{TAG}: Create User failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnCreateUserFailed?.Invoke(error);
                });
        }

        public void ResetPassword(string email, string newPassword, string verifyCode)
        {
            if (_AGConnectAuth == null) return;
            _AGConnectAuth.ResetPassword(email, newPassword, verifyCode)
                .AddOnSuccessListener(Void =>
                {
                    OnResetPasswordSuccess?.Invoke(true);
                })
                .AddOnFailureListener(error =>
                {
                    Debug.LogError($"{TAG}: Reset Password failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnResetPasswordFailed?.Invoke(error);
                });
        }

        public void ResetPassword(string countryCode, string phoneNumber, string newPassword, string verifyCode)
        {
            if (_AGConnectAuth == null) return;
            _AGConnectAuth.ResetPassword(countryCode, phoneNumber, newPassword, verifyCode)
                .AddOnSuccessListener(Void =>
                {
                    OnResetPasswordSuccess?.Invoke(true);
                })
                .AddOnFailureListener(error =>
                {
                    Debug.LogError($"{TAG}: Reset Password failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnResetPasswordFailed?.Invoke(error);
                });
        }
    }
}
