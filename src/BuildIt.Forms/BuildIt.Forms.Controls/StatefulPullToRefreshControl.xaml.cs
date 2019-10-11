using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatefulPullToRefreshControl
    {
        private const string ContentPresenterContainerName = "ContentPresenterContainer";
        private const string EmptyStateContainerName = "EmptyStateContainer";
        private const string LoadingErrorStateContainerName = "LoadingErrorStateContainer";
        private const string LoadingStateContainerName = "LoadingStateContainer";
        private const string PullToRefreshContainerName = "PullToRefreshContainer";
        private const string PullToRefreshErrorStateContainerName = "PullToRefreshErrorStateContainer";
        private const string PullToRefreshInnerContainerName = "PullToRefreshInnerContainer";
        private const string PullToRefreshOuterContainerName = "PullToRefreshOuterContainer";

        private Grid contentPresenterContainer;
        private Grid emptyStateContainer;
        private Grid loadingErrorStateContainer;
        private Grid loadingStateContainer;
        private Grid mainContainer;
        private Grid pullToRefreshContainer;
        private Grid pullToRefreshErrorStateContainer;
        private Grid pullToRefreshInnerContainer;
        private Grid pullToRefreshOuterContainer;

        public StatefulPullToRefreshControl()
        {
            InitializeComponent();
        }

        internal Grid ContentPresenterContainer => contentPresenterContainer ?? (contentPresenterContainer = MainContainer?.FindByName<Grid>(ContentPresenterContainerName));

        internal Grid EmptyStateContainer => emptyStateContainer ?? (emptyStateContainer = MainContainer?.FindByName<Grid>(EmptyStateContainerName));

        internal Grid LoadingErrorStateContainer => loadingErrorStateContainer ?? (loadingErrorStateContainer = MainContainer?.FindByName<Grid>(LoadingErrorStateContainerName));

        internal Grid LoadingStateContainer => loadingStateContainer ?? (loadingStateContainer = MainContainer?.FindByName<Grid>(LoadingStateContainerName));

        internal Grid MainContainer => mainContainer ?? (mainContainer = Content as Grid);

        internal Grid PullToRefreshContainer => pullToRefreshContainer ?? (pullToRefreshContainer = PullToRefreshInnerContainer?.FindByName<Grid>(PullToRefreshContainerName));

        internal Grid PullToRefreshErrorStateContainer => pullToRefreshErrorStateContainer ?? (pullToRefreshErrorStateContainer = PullToRefreshOuterContainer?.FindByName<Grid>(PullToRefreshErrorStateContainerName));

        internal Grid PullToRefreshInnerContainer => pullToRefreshInnerContainer ?? (pullToRefreshInnerContainer = PullToRefreshOuterContainer?.FindByName<Grid>(PullToRefreshInnerContainerName));

        internal Grid PullToRefreshOuterContainer => pullToRefreshOuterContainer ?? (pullToRefreshOuterContainer = MainContainer?.FindByName<Grid>(PullToRefreshOuterContainerName));

        internal VisualElement RetrieveStatefulContainer(StatefulControlStates state)
        {
            switch (state)
            {
                case StatefulControlStates.Loading:
                    return LoadingStateContainer;

                case StatefulControlStates.Empty:
                    return EmptyStateContainer;

                case StatefulControlStates.LoadingError:
                    return LoadingErrorStateContainer;

                case StatefulControlStates.PullToRefresh:
                    return PullToRefreshContainer;

                case StatefulControlStates.PullToRefreshError:
                    return PullToRefreshErrorStateContainer;
            }

            return null;
        }
    }
}