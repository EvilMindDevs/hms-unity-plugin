using HuaweiMobileServices.Drive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    class CredentialManager
    {
        private DriveCredential mDriveCredential;

        private CredentialManager() { }

        private static class InnerHolder
        {
            public static CredentialManager sInstance = new CredentialManager();
        }

        public static CredentialManager GetInstance()
        {
            return InnerHolder.sInstance;
        }

        public int Init(string unionID, string at, DriveCredential.AccessMethod refreshAT)
        {
            if (string.IsNullOrEmpty(unionID) || string.IsNullOrEmpty(at))
            {
                return DriveCode.ERROR;
            }
            DriveCredential.Builder builder = new DriveCredential.Builder(unionID, refreshAT);
            mDriveCredential = builder.Build().SetAccessToken(at);
            return DriveCode.SUCCESS;
        }

        public DriveCredential GetCredential()
        {
            return mDriveCredential;
        }

    }
}
