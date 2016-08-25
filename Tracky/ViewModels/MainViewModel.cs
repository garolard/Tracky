using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using GeekyTool.Base;
using GeekyTool.Helpers;
using TraktApiSharp;
using TraktApiSharp.Enums;
using TraktApiSharp.Objects.Get.Shows;
using TraktApiSharp.Requests.Params;

namespace Tracky.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly TraktClient _client;

        private OptimizedObservableCollection<TraktShow> _searchSuggestions;
        private string _searchQuery;

        public MainViewModel()
        {
            _client = new TraktClient(Constants.TraktId);
            SearchSuggestions = new OptimizedObservableCollection<TraktShow>();
            Shows = new OptimizedObservableCollection<TraktShow>();

            PullSearchSuggestionsCommand = new DelegateCommandAsync<bool>(PullSearchSuggestionAsync);
            PerformSearchQueryCommand = new DelegateCommandAsync<TraktShow>(PerformSearchQueryAsync);
        }

        public OptimizedObservableCollection<TraktShow> SearchSuggestions
        {
            get { return _searchSuggestions; }
            private set { Set(ref _searchSuggestions, value); }
        }

        public OptimizedObservableCollection<TraktShow> Shows { get; private set; }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set { Set(ref _searchQuery, value); }
        }

        public ICommand PullSearchSuggestionsCommand { get; private set; }
        
        public ICommand PerformSearchQueryCommand { get; private set; }

        public ICommand NavigateToShowCommand { get; private set; }

        private async Task PullSearchSuggestionAsync(bool userChangedQuery)
        {
            if (!userChangedQuery) return;

            var query = SearchQuery;
            if (string.IsNullOrEmpty(query))
            {
                SearchSuggestions.Clear();
                SearchSuggestions.Add(new TraktShow {Title = "No result"});
                return;
            }

            var searchResults = await _client.Search.GetTextQueryResultsAsync(TraktSearchResultType.Show, query);

            var tasks = searchResults
                .Select(result => result.Show.Ids.Trakt.ToString())
                .Select(showId => _client.Shows.GetShowAsync(showId, new TraktExtendedOption { Full = true, Images = true }))
                .ToList();

            var fullShows = await Task.WhenAll(tasks);
            if (fullShows.Any())
            {
                SearchSuggestions.Clear();
                SearchSuggestions.AddRange(fullShows);
            }
            else
            {
                SearchSuggestions.Clear();
                SearchSuggestions.Add(new TraktShow {Title = "No result"});
            }
        }

        private async Task PerformSearchQueryAsync(TraktShow selectedShow)
        {
            var query = string.Empty;

            query = selectedShow == null ? SearchQuery : selectedShow.Title;

            var searchResults = await _client.Search.GetTextQueryResultsAsync(TraktSearchResultType.Show, query);

            var tasks = searchResults
                .Select(result => result.Show.Ids.Trakt.ToString())
                .Select(showId => _client.Shows.GetShowAsync(showId, new TraktExtendedOption { Full = true, Images = true }))
                .ToList();

            var fullShows = await Task.WhenAll(tasks);
            Shows.Clear();
            Shows.AddRange(fullShows);
        }
    }
}