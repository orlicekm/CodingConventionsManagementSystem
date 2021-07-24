using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCMS.BL.Configurator;
using CCMS.BL.Models.Detail;
using CCMS.BL.Models.List;
using CCMS.BL.Services;
using CCMS.BL.Services.Database;
using CCMS.BL.ViewModels.Base;
using CCMS.BL.ViewModels.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace CCMS.BL.ViewModels
{
    public class ConventionViewModel : BaseViewModel, IScoped
    {
        private readonly CommentDataService commentDataService;
        private readonly ConventionDataService conventionDataService;
        private readonly HighlightService highlightService;
        private readonly IMessenger messenger;
        private readonly PatchesDataService patchesDataService;
        private readonly PatchService patchService;
        private readonly UpdatedTextDataService updatedTextDataService;
        private ICollection<(string, PatchListModel)> allHistory = new List<(string, PatchListModel)>();
        private string formalText;
        private ConventionDetailModel selectedConvention;
        private ICollection<(string, PatchListModel)> visibleHistory = new List<(string, PatchListModel)>();

        public ConventionViewModel(ConventionDataService conventionDataService,
            CommentDataService commentDataService, UpdatedTextDataService updatedTextDataService,
            PatchesDataService patchesDataService, HighlightService highlightService, PatchService patchService,
            IMessenger messenger)
        {
            this.conventionDataService = conventionDataService;
            this.commentDataService = commentDataService;
            this.updatedTextDataService = updatedTextDataService;
            this.patchesDataService = patchesDataService;
            this.highlightService = highlightService;
            this.patchService = patchService;
            this.messenger = messenger;

            messenger.Register<SelectedUserChangedMessage>(this, SelectedUserChangedMessageReceived);
            messenger.Send(new SelectedUserAskMessage());
        }

        public UserListModel SelectedUser { get; private set; }

        public int PageSize => 1;

        public string FormalText
        {
            get => formalText;
            set => SetPropertyValue(ref formalText, value);
        }

        public ICollection<(string, PatchListModel)> AllHistory
        {
            get => allHistory;
            set
            {
                SetPropertyValue(ref allHistory, value);
                VisibleHistory = AllHistory.Take(PageSize).ToList();
            }
        }

        public ICollection<(string, PatchListModel)> VisibleHistory
        {
            get => visibleHistory;
            set => SetPropertyValue(ref visibleHistory, value);
        }


        public ConventionDetailModel SelectedConvention
        {
            get => selectedConvention;
            set => SetPropertyValue(ref selectedConvention, value);
        }

        private void SelectedUserChangedMessageReceived(SelectedUserChangedMessage m)
        {
            SelectedUser = m.SelectedUser;
        }

        public async Task<bool> SetSelectedConvention(Guid id)
        {
            SelectedConvention = await conventionDataService.Get(id);
            if (SelectedConvention == null)
                return false;
            FormalText = GetFormalHighlighted();
            messenger.Send(new SelectRepositoryMessage(SelectedConvention.Repository.Id));
            return true;
        }

        public async Task UpdateTitle(string title)
        {
            SelectedConvention = await conventionDataService.UpdateTitle(title, SelectedConvention, SelectedUser);
        }

        public async Task DeleteSelectedConvention()
        {
            await conventionDataService.Delete(SelectedConvention);
        }

        public async Task AddComment(string text)
        {
            var comment = await commentDataService.Insert(text, SelectedUser, SelectedConvention);
            SelectedConvention.Comments.Add(comment);
            OnPropertyChanged(nameof(SelectedConvention));
        }

        public async Task UpdateFormattedText(string text)
        {
            SelectedConvention = await updatedTextDataService.UpdateFormatted(text, SelectedConvention, SelectedUser);
        }

        public async Task UpdateFormalText(string text)
        {
            SelectedConvention = await updatedTextDataService.UpdateFormal(text, SelectedConvention, SelectedUser);
            FormalText = GetFormalHighlighted();
        }

        public string GetFormalHighlighted()
        {
            const string error = "<p><span style=\"color: #ff0000;\">Formatting request failed!</span><br /><br /></p>";
            try
            {
                return highlightService.HighlightInIni(SelectedConvention.FormalText.Text);
            }
            catch (Exception)
            {
                return $"{error}<span>{SelectedConvention.FormalText.Text.Replace("\n", "<br />")}</span>";
            }
        }

        public async Task UpdateFormalHistory()
        {
            var patches = await patchesDataService.Get(SelectedConvention.FormalText);
            AllHistory = patchService.GetTexts(patches, SelectedConvention.FormalText.Text)
                .Select(p => (highlightService.FormatPatch(p.Item1), p.Item2)).ToList();
        }

        public async Task UpdateFormattedHistory()
        {
            var patches = await patchesDataService.Get(SelectedConvention.FormattedText);
            AllHistory = patchService.GetTexts(patches, SelectedConvention.FormattedText.Text, true)
                .Select(p => (highlightService.FormatPatch(p.Item1), p.Item2)).ToList();
        }
    }
}