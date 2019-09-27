namespace BuildIt.Forms.Controls.Extensions
{
    public static class StatefulControlStatesExtension
    {
        public static bool IsPullToRefreshRelated(this StatefulControlStates state)
        {
            return state == StatefulControlStates.PullToRefresh || state == StatefulControlStates.PullToRefreshError;
        }
    }
}