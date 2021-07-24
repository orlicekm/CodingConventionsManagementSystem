using GalaSoft.MvvmLight.Messaging;

namespace CCMS.BL.ViewModels.Messages
{
    public class AccessTokenChangedMessage : MessageBase
    {
        public AccessTokenChangedMessage(string accessToken)
        {
            AccessToken = accessToken;
        }

        public string AccessToken { get; }
    }
}