using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HuaweiConstants
{
    public static class IAP
    {
        public enum IapType : int
        {
            CONSUMABLE = 0,
            NON_CONSUMABLE = 1,
            SUBSCRIPTION = 2
        }

    }

    public static class UnityBannerAdPositionCode
    {
        public enum UnityBannerAdPositionCodeType : int
        {
            /**
             * Position constant for a position with a custom offset.
             */
            POSITION_CUSTOM = -1,

            /**
             * Position constant for top of the screen.
             */
            POSITION_TOP = 0,

            /**
             * Position constant for bottom of the screen.
             */
            POSITION_BOTTOM = 1,

            /**
             * Position constant for top-left of the screen.
             */
            POSITION_TOP_LEFT = 2,

            /**
             * Position constant for top-right of the screen.
             */
            POSITION_TOP_RIGHT = 3,

            /**
             * Position constant for bottom-left of the screen.
             */
            POSITION_BOTTOM_LEFT = 4,

            /**
             * Position constant bottom-right of the screen.
             */
            POSITION_BOTTOM_RIGHT = 5,

            /**
             * Position constant center of the screen.
             */
            POSITION_CENTER = 6
        }

    }

    public enum UnityBannerAdSizeType
    {
        BANNER_SIZE_320_50,
        BANNER_SIZE_320_100,
        BANNER_SIZE_468_60,
        BANNER_SIZE_DYNAMIC,
        BANNER_SIZE_728_90,
        BANNER_SIZE_300_250,
        BANNER_SIZE_SMART,
        BANNER_SIZE_160_600,
        BANNER_SIZE_360_57,
        BANNER_SIZE_360_144
    }

    public static class UnityBannerAdSize
    {

        public const string USER_DEFINED = "USER_DEFINED";

        public const string BANNER_SIZE_320_50 = "BANNER_SIZE_320_50";

        public const string BANNER_SIZE_320_100 = "BANNER_SIZE_320_100";

        public const string BANNER_SIZE_468_60 = "BANNER_SIZE_468_60";

        public const string BANNER_SIZE_DYNAMIC = "BANNER_SIZE_DYNAMIC";

        public const string BANNER_SIZE_728_90 = "BANNER_SIZE_728_90";

        public const string BANNER_SIZE_300_250 = "BANNER_SIZE_300_250";

        public const string BANNER_SIZE_SMART = "BANNER_SIZE_SMART";

        public const string BANNER_SIZE_160_600 = "BANNER_SIZE_160_600";

        public const string BANNER_SIZE_360_57 = "BANNER_SIZE_360_57";

        public const string BANNER_SIZE_360_144 = "BANNER_SIZE_360_144";

        public static string CustomBannerSize(int widht, int height)
        {
            return ($"BANNER_SIZE_{widht}_{height}");
        }


    }

    public static class HMSResponses
    {
        public static List<int> UnresolvableStatusCodes = new List<int> { 6003 };

        public enum GameStatusCodes
        {
            GAME_STATE_SUCCESS = 0,
            GAME_STATE_FAILED = -1,
            GAME_STATE_ERROR = 7001,
            GAME_STATE_NETWORK_ERROR = 7002,
            GAME_STATE_USER_CANCEL = 7003,
            GAME_STATE_USER_CANCEL_LOGIN = 7004,
            GAME_STATE_PARAM_ERROR = 7005,
            GAME_STATE_NO_SUPPORT = 7006,
            GAME_STATE_PARAM_NULL = 7011,
            GAME_STATE_CALL_REPEAT = 7012,
            GAME_STATE_NOT_LOGIN = 7013,
            GAME_STATE_DISAGREE_PROTOCOL = 7014

        }

        public enum PayStatusCodes
        {
            PAY_STATE_SUCCESS = 0,
            PAY_STATE_FAILED = -1,
            PAY_STATE_CANCEL = 30000,
            PAY_STATE_PARAM_ERROR = 30001,
            PAY_STATE_TIME_OUT = 30002,
            PAY_STATE_NET_ERROR = 30005,
            PAY_STATE_ERROR = 30099,
            PAY_OTHER_ERROR = 30006,
            HWPAY_COUNTRY_CODE_ERROR = 30011,
            ORDER_STATUS_HANDLING = 30012,
            ORDER_STATUS_UNTREATED = 30013,
            PAY_GAME_ACCOUNT_ERROR = 30101,
            PRODUCT_NOT_EXIST = 40001,
            PRODUCT_AUTHENTICATION_FAILED = 40002,
            PRODUCT_SERVER_INTERNAL_EXCEPTION = 40003,
            PRODUCT_SOME_NOT_EXIST = 40004

        }

        public enum IapStatusCodes : int
        {
            ORDER_STATE_SUCCESS = 0,
            ORDER_STATE_FAILED = -1,
            ORDER_STATE_DEFAULT_CODE = 1,
            ORDER_STATE_CANCEL = 60000,
            ORDER_STATE_PARAM_ERROR = 60001,
            ORDER_STATE_IAP_NOT_ACTIVATED = 60002,
            ORDER_STATE_PRODUCT_INVALID = 60003,
            ORDER_STATE_CALLS_FREQUENT = 60004,
            ORDER_STATE_NET_ERROR = 60005,
            ORDER_STATE_PMS_TYPE_NOT_MATCH = 60006,
            ORDER_STATE_PRODUCT_COUNTRY_NOT_SUPPORTED = 60007,
            ORDER_HWID_NOT_LOGIN = 60050,
            ORDER_PRODUCT_OWNED = 60051,
            ORDER_PRODUCT_NOT_OWNED = 60052,
            ORDER_PRODUCT_CONSUMED = 60053,
            ORDER_ACCOUNT_AREA_NOT_SUPPORTED = 60054,
            ORDER_HIGH_RISK_OPERATIONS = 60056
        }

        public enum AppUpdateStatusCode : int
        {
            PARAMETER_ERROR = 1,
            CONNECT_ERROR = 2,
            NO_UPGRADE_INFO = 3,
            CANCEL = 4,
            INSTALL_FAILED = 5,
            CHECK_FAILED = 6,
            HAS_UPGRADE_INFO = 7,
            MARKET_FORBID = 8,
            IN_MARKET_UPDATING = 9
        }

        public enum AppUpdateRtnCode : int
        {
            NORMAL = 0,
            NO_NETWORK_AVAILABLE = 1,
            JSON_EXCEPTION = 2,
            PARAMETER_EXCEPTION = 3,
            IO_EXCEPTION = 4,
            NETWORK_EXCEPTION = 5,
            UNKNOWN_ERROR = 6,
            OBSFUCATION_NOT_EXCLUDED = 7
        }

        public enum AppUpdateButtonStatus : int
        {
            USER_CHOOSES_UPDATE_LATER = 100,
            USER_CHOOSES_UPDATE_NOW = 101
        }
    }
}
