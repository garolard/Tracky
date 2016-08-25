using System.Linq;
using System.Threading.Tasks;
using GeekyTool.Base;
using GeekyTool.Helpers;
using GeekyTool.Services;
using TraktApiSharp;
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
            Actors = new OptimizedObservableCollection<TraktPerson>();
        }

        public TraktShow Show
        {
            get { return _show; }
            set { Set(ref _show, value); }
        }

        public OptimizedObservableCollection<TraktPerson> Actors { get; set; }

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
            var showPeople = await _client.Shows.GetShowPeopleAsync(Show.Ids.Trakt.ToString());
            var tasks = showPeople.Cast
                .Select(cm => cm.Person)
                .Select(p => _client.People.GetPersonAsync(p.Ids.Trakt.ToString(), new TraktExtendedOption {Images = true}))
                .ToList();

            var actors = await Task.WhenAll(tasks);
            Actors.AddRange(actors);
        }
    }
}