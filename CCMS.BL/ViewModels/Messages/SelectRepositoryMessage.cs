using System;
using GalaSoft.MvvmLight.Messaging;

namespace CCMS.BL.ViewModels.Messages
{
    public class SelectRepositoryMessage : MessageBase
    {
        public SelectRepositoryMessage(Guid repositoryId)
        {
            RepositoryId = repositoryId;
        }

        public Guid RepositoryId { get; }
    }
}