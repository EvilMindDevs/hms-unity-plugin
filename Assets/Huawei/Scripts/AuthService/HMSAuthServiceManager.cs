using HuaweiMobileServices.AuthService;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSAuthServiceManager : HMSManagerSingleton<HMSAuthServiceManager>
    {
        public Action<SignInResult> OnSignInSuccess { get; set; }
        public Action<HMSException> OnSignInFailed { get; set; }

        public Action<SignInResult> OnCreateUserSuccess { get; set; }
        public Action<HMSException> OnCreateUserFailed { get; set; }

        public Action<bool> OnResetPasswordSuccess { get; set; }
        public Action<HMSException> OnResetPasswordFailed { get; set; }

        AGConnectAuth _AGConnectAuth = null;

        public HMSAuthServiceManager()
        {
            if (!HMSDispatcher.InstanceExists)
                HMSDispatcher.CreateDispatcher();
            HMSDispatcher.InvokeAsync(OnAwake);
        }

        private void OnAwake()
        {
            Debug.Log("[HMSAuthServiceManager]: AuthService OnAwake");
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
                    Debug.LogError("[HMSAuthServiceManager]: Sign in failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
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
                    Debug.LogError("[HMSAuthServiceManager]: Sign in Anonymously failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
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
                .AddOnSuccessListener(signInResult => {
                    OnCreateUserSuccess?.Invoke(signInResult);
                })
                .AddOnFailureListener(error => {
                    Debug.LogError("[HMSAuthServiceManager]: Create User failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnCreateUserFailed?.Invoke(error);
                });
        }

        public void CreateUser(PhoneUser phoneUser)
        {
            if (_AGConnectAuth == null) return;

            _AGConnectAuth.CreateUser(phoneUser)
                .AddOnSuccessListener(signInResult => {
                    OnCreateUserSuccess?.Invoke(signInResult);
                })
                .AddOnFailureListener(error => {
                    Debug.LogError("[HMSAuthServiceManager]: Create User failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnCreateUserFailed?.Invoke(error);
                });
        }

        public void ResetPassword(string email, string newPassword, string verifyCode)
        {
            if (_AGConnectAuth == null) return;
            _AGConnectAuth.ResetPassword(email, newPassword, verifyCode)
                .AddOnSuccessListener(Void => {
                    OnResetPasswordSuccess?.Invoke(true);
                })
                .AddOnFailureListener(error => {
                    Debug.LogError("[HMSAuthServiceManager]: Reset Password failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnResetPasswordFailed?.Invoke(error);
                });
        }

        public void ResetPassword(string countryCode, string phoneNumber, string newPassword, string verifyCode)
        {
            if (_AGConnectAuth == null) return;
            _AGConnectAuth.ResetPassword(countryCode, phoneNumber, newPassword, verifyCode)
                .AddOnSuccessListener(Void => {
                    OnResetPasswordSuccess?.Invoke(true);
                })
                .AddOnFailureListener(error => {
                    Debug.LogError("[HMSAuthServiceManager]: Reset Password failed. CauseMessage: " + error.WrappedCauseMessage + ", ExceptionMessage: " + error.WrappedExceptionMessage);
                    OnResetPasswordFailed?.Invoke(error);
                });
        }
    }
}