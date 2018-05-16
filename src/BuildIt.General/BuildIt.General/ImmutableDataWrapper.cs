using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt
{
    /// <summary>
    /// Provides a wrapper that handles when the Data property changes on an entity
    /// Instead of propagating a change to the entire entity, the wrapper does a deep
    /// update, only raising PropertyChanged for properties that have changed
    /// </summary>
    /// <typeparam name="TData">The type of data property to wrap</typeparam>
    public class ImmutableDataWrapper<TData> : NotifyBase
        where TData : class
    {
        /// <summary>
        /// Tracks whether ChangeData is running
        /// </summary>
        private int isRunningChangeData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableDataWrapper{TData}"/> class.
        /// Constru
        /// </summary>
        /// <param name="entityWithData">The entity that has a Data property to wrap</param>
        public ImmutableDataWrapper(IHasImmutableData<TData> entityWithData)
        {
            ChangeData(entityWithData.Data);
            entityWithData.PropertyChanged += DataPropertyChanged;
        }

        /// <summary>
        /// Gets or sets the current data
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether indicates whether ChangeData needs to be run again
        /// </summary>
        private bool RunChangeData { get; set; }

        /// <summary>
        /// Gets or sets the next data to be used in ChangeData
        /// </summary>
        private TData NextData { get; set; }

        /// <summary>
        /// Handles when the data property on the entity changes
        /// </summary>
        /// <param name="sender">The entity</param>
        /// <param name="e">The property that has changed</param>
        private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Data))
            {
                var data = (sender as IHasImmutableData<TData>)?.Data;
                ChangeData(data);
            }
        }

        /// <summary>
        /// Updates Data with the new data by doing a deep update on all properties
        /// </summary>
        /// <param name="nextData">The data to update to</param>
        private async void ChangeData(TData nextData)
        {
            RunChangeData = true;
            NextData = nextData;
            if (Interlocked.CompareExchange(ref isRunningChangeData, 0, 1) == 1)
            {
                return;
            }

            RunChangeData = false;
            try
            {
                var oldData = Data;
                var newData = NextData;
                if (oldData == null)
                {
                    if (newData != null && oldData != newData)
                    {
                        // There was no old data, so simply set the new data to be the current Data
                        // and raise PropertyChanged
                        Data = newData;
                        OnPropertyChanged(nameof(Data));
                    }

                    // Nothing more to do, so just return;
                    return;
                }

                if (newData == null)
                {
                    Data = newData;
                    OnPropertyChanged(nameof(Data));
                    return;
                }

                UpdateData(oldData, newData);
            }
            finally
            {
                Interlocked.Exchange(ref isRunningChangeData, 0);
            }

            await Task.Yield();
            if (RunChangeData)
            {
                ChangeData(NextData);
            }
        }

        private void UpdateData(object oldData, object newData)
        {
            var typeHelper = TypeHelper.RetrieveHelperForType(oldData.GetType());
            typeHelper.DeepUpdater(oldData, newData);
        }
    }
}