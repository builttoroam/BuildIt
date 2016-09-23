using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.IoC;
using SQLiteWrapper.CRUD.Core.ViewModels;

namespace SQLiteWrapper.CRUD.Core
{
    public class App :MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<MainViewModel>();
        }
    }
}
