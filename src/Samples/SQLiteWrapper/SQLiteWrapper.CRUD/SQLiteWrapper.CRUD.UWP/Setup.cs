using Windows.UI.Xaml.Controls;
using BuildIt.SQLiteWrapper.Database.Interfaces;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.WindowsUWP.Platform;
using SQLiteWrapper.CRUD.Core.Services;
using SQLiteWrapper.CRUD.Core.Services.Interfaces;
using SQLiteWrapper.CRUD.UWP.Services;

namespace SQLiteWrapper.CRUD.UWP
{
    public class Setup : MvxWindowsSetup
    {
        public Setup(Frame rootFrame)
            : base(rootFrame)
        {

        }

        protected override IMvxApplication CreateApp()
        {
            return new Core.App();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();

            Mvx.LazyConstructAndRegisterSingleton<ISqlitePlatformProvider, SqlitePlatformProvider>();
            Mvx.LazyConstructAndRegisterSingleton<ILocalFileService, LocalFileService>();
            Mvx.LazyConstructAndRegisterSingleton<IDatabaseNameProvider, DatabaseNameProvider>();
            


            //Mvx.LazyConstructAndRegisterSingleton<IDatabaseService, DatabaseService>();
        }
    }
}