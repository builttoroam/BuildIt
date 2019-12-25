using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildIt.UI
{
    public static class EventMapHelpers
    {
        public static EventMapBuilderViewModelEvent<TViewModel, TDelegate>
            Do<TViewModel, TDelegate>(this EventMapBuilderViewModel<TViewModel> viewModelBuilder, TDelegate viewModelEvent)
        {
            return viewModelBuilder.BuildWithEvent(viewModelEvent);
        }

        public static EventMapBuilderViewModel<TViewModel> For<TViewModel>(this IDictionary<Type, IEventMap[]> maps)
        {
            return new EventMapBuilderViewModel<TViewModel>() { Maps = maps };
        }

        public static void When<TViewModel, TDelegate>(this EventMapBuilderViewModelEvent<TViewModel, TDelegate> builder, Action<TViewModel, TDelegate> arrive, Action<TViewModel, TDelegate> leave)
        {
            var key = typeof(TViewModel);
            var map = new EventMap<TViewModel, TDelegate>(builder.ViewModelAction, arrive, leave);
            if (builder.Maps.TryGetValue(key, out IEventMap[] existing))
            {
                builder.Maps[key] = existing.Union(new[] { map }).ToArray();
                return;
            }

            builder.Maps[key] = new[] { map };
        }

        public class EventMapBuilderViewModel<TViewModel>
        {
            public IDictionary<Type, IEventMap[]> Maps { get; set; }

            public EventMapBuilderViewModelEvent<TViewModel, TDelegate> BuildWithEvent<TDelegate>(TDelegate viewModelAction)
            {
                return new EventMapBuilderViewModelEvent<TViewModel, TDelegate> { ViewModelAction = viewModelAction, Maps = Maps };
            }
        }

        public class EventMapBuilderViewModelEvent<TViewModel, TDelegate> :
                    EventMapBuilderViewModel<TViewModel>
        {
            public TDelegate ViewModelAction { get; set; }
        }
    }
}