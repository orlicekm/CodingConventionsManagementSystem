using CCMS.BL.Models.Detail;
using GalaSoft.MvvmLight.Messaging;

namespace CCMS.BL.ViewModels.Messages
{
    public class SelectedRepositoryChangedMessage : MessageBase
    {
        public SelectedRepositoryChangedMessage(RepositoryDetailModel selectedRepository)
        {
            SelectedRepository = selectedRepository;
        }

        public RepositoryDetailModel SelectedRepository { get; }
    }
}