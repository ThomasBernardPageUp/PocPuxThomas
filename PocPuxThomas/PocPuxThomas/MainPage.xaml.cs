using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PageUpX.Core.Entities;
using PageUpX.Core.Log;
using PageUpX.DataAccess.Repository;
using PageUpX.DataAccess.SQLite;
using Xamarin.Forms;

namespace PocPuxThomas
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var logger = new ConsoleLoggerService();
            var dataAssesor = new PuxSimpleDataAccessorSQLite<User>(logger);

            var userRepo = new PuxSimpleRepositoryBase<User>(dataAssesor);
        }
    }



    public class User : IPuxEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
