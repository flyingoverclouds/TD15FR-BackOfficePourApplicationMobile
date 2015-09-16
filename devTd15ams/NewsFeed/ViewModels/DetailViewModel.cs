using MCS.MVVM;
using Microsoft.WindowsAzure.MobileServices;
using NewsFeed.Common;
using NewsFeed.Model;
using NewsFeed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFeed.ViewModels
{
    public class DetailViewModel : ViewModelBase
    {
        private FeedItem _item;
        public FeedItem Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                NotifyPropertyChanged("Item");
            }
        }

        public DetailViewModel(object data)
        {
            Item = data as FeedItem;
            FavoritesCommand = new RelayCommand(() => SaveFavoris());
        }

        public RelayCommand FavoritesCommand { get; private set; }

        private async void SaveFavoris()
        {
            IMobileServiceTable<BlogSubscription> favoriteTable = App.MobileService.GetTable<BlogSubscription>();
            var favoriteItem = new BlogSubscription { BlogName = Item.Title };
            await favoriteTable.InsertAsync(favoriteItem);
        }
    }
}
