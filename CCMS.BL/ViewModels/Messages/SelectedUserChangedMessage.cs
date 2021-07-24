using CCMS.BL.Models.List;
using GalaSoft.MvvmLight.Messaging;

namespace CCMS.BL.ViewModels.Messages
{
    public class SelectedUserChangedMessage : MessageBase
    {
        public SelectedUserChangedMessage(UserListModel selectedUser)
        {
            SelectedUser = selectedUser;
        }

        public UserListModel SelectedUser { get; }
    }
}