using System.Linq;
using System.Threading.Tasks;
using GeekyTool.Base;
using GeekyTool.Helpers;
using GeekyTool.Services;
using TraktApiSharp;
using TraktApiSharp.Objects.Basic;
using TraktApiSharp.Objects.Get.People;
using TraktApiSharp.Objects.Get.Shows;
using TraktApiSharp.Requests.Params;

namespace Tracky.ViewModels
{
    public class DetailViewModel : BaseViewModel, INavigable<TraktShow>
    {
        private readonly TraktClient _client;

        private TraktShow _show;

        public DetailViewModel()
        {
            _client = new TraktClient(Constants.TraktId);
            Actors = new OptimizedObservableCollection<TraktCastMember>();
        }

        public TraktShow Show
        {
            get { return _show; }
            set { Set(ref _show, value); }
        }

        public OptimizedObservableCollection<TraktCastMember> Actors { get; set; }

        public Task OnNavigatedFrom(TraktShow e)
        {
            throw new System.NotImplementedException();
        }

        public async Task OnNavigatedTo(TraktShow e)
        {
            Show = e;
            Actors.Clear();
            await LoadActorsAsync();
        }

        private async Task LoadActorsAsync()
        {
            var showPeople = await _client.Shows.GetShowPeopleAsync(Show.Ids.Trakt.ToString(), new TraktExtendedOption {Full = true, Images = true});
            Actors.AddRange(showPeople.Cast);
        }
    }
}