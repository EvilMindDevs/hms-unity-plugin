using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HuaweiConstants
{
    public static class IAP
    {
        public enum IapType :int
        {
            CONSUMABLE = 0,
            NON_CONSUMABLE = 1,
            SUBSCRIPTION = 2
        }
        
    }
    public static class HMSResponses
    {

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
    }

}