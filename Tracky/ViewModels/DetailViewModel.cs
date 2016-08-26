using System.Linq;
using System.Threading.Tasks;
using GeekyTool.Base;
using GeekyTool.Helpers;
using GeekyTool.Services;
using TraktApiSharp;
using TraktApiSharp.Objects.Basic;
using TraktApiSharp.Objects.Get.People;
using TraktApiSharp.Objects.Get.Shows;
using TraktApiSharp.Objects.Get.Shows.Episodes;
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
            RecentEpisodes = new OptimizedObservableCollection<TraktEpisode>();
        }

        public TraktShow Show
        {
            get { return _show; }
            set { Set(ref _show, value); }
        }

        public OptimizedObservableCollection<TraktCastMember> Actors { get; private set; }

        public OptimizedObservableCollection<TraktEpisode> RecentEpisodes { get; private set; }

        public Task OnNavigatedFrom(TraktShow e)
        {
            throw new System.NotImplementedException();
        }

        public async Task OnNavigatedTo(TraktShow e)
        {
            Show = e;
            Actors.Clear();
            RecentEpisodes.Clear();
            await LoadActorsAsync();
            await LoadRecentEpisodesAsync();
        }

        private async Task LoadActorsAsync()
        {
            var showPeople = await _client.Shows.GetShowPeopleAsync(Show.Ids.Trakt.ToString(), new TraktExtendedOption {Full = true, Images = true});
            Actors.AddRange(showPeople.Cast);
        }

        private async Task LoadRecentEpisodesAsync()
        {
            var showSeasons = await _client.Seasons.GetAllSeasonsAsync(Show.Ids.Slug);
            var lastSeason =
                await
                    _client.Seasons.GetSeasonAsync(Show.Ids.Slug, showSeasons.Last().Number.Value,
                        new TraktExtendedOption() {Episodes = true, Images = true});
            var recentEpisodes = lastSeason.Reverse().Take(3).ToList();
            RecentEpisodes.AddRange(recentEpisodes);
        }
    }
}