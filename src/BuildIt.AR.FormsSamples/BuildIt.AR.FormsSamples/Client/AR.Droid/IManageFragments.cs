//using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentManager = Android.App.FragmentManager;

namespace BuildIt.AR.FormsSamples.Android
{
    public interface IManageFragments
    {
        void SetFragmentManager(FragmentManager fragmentManager);
    }
}