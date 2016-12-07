// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace BuildIt.CognitiveServices
{
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for SpellCheckAPIV5.
    /// </summary>
    public static partial class SpellCheckAPIV5Extensions
    {
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='mode'>
            /// Mode of spellcheck:
            /// &lt;ul&gt;&lt;li&gt;Proof - Meant to provide Office Word like spelling
            /// corrections. It can correct long queries, provide casing corrections and
            /// suppresses aggressive corrections.&lt;/li&gt;
            /// &lt;li&gt;Spell - Meant to provide Search engine like spelling
            /// corrections. It will correct small queries(up to length 9 tokens) without
            /// any casing changes and will be more optimized (perf and relevance)
            /// towards search like queries.&lt;/li&gt;&lt;/ul&gt;
            /// . Possible values include: 'spell', 'proof'
            /// </param>
            /// <param name='mkt'>
            /// For proof mode, only support en-us, es-es, pt-br,
            /// For spell mode, support all language codes. Possible values include:
            /// 'en-us', 'es-es', 'pt-br'
            /// </param>
            /// <param name='subscriptionKey'>
            /// subscription key in url
            /// </param>
            /// <param name='ocpApimSubscriptionKey'>
            /// subscription key in header
            /// </param>
            public static void SpellCheck(this ISpellCheckAPIV5 operations, string mode = default(string), string mkt = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string))
            {
                System.Threading.Tasks.Task.Factory.StartNew(s => ((ISpellCheckAPIV5)s).SpellCheckAsync(mode, mkt, subscriptionKey, ocpApimSubscriptionKey), operations, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskCreationOptions.None,  System.Threading.Tasks.TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='mode'>
            /// Mode of spellcheck:
            /// &lt;ul&gt;&lt;li&gt;Proof - Meant to provide Office Word like spelling
            /// corrections. It can correct long queries, provide casing corrections and
            /// suppresses aggressive corrections.&lt;/li&gt;
            /// &lt;li&gt;Spell - Meant to provide Search engine like spelling
            /// corrections. It will correct small queries(up to length 9 tokens) without
            /// any casing changes and will be more optimized (perf and relevance)
            /// towards search like queries.&lt;/li&gt;&lt;/ul&gt;
            /// . Possible values include: 'spell', 'proof'
            /// </param>
            /// <param name='mkt'>
            /// For proof mode, only support en-us, es-es, pt-br,
            /// For spell mode, support all language codes. Possible values include:
            /// 'en-us', 'es-es', 'pt-br'
            /// </param>
            /// <param name='subscriptionKey'>
            /// subscription key in url
            /// </param>
            /// <param name='ocpApimSubscriptionKey'>
            /// subscription key in header
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async System.Threading.Tasks.Task SpellCheckAsync(this ISpellCheckAPIV5 operations, string mode = default(string), string mkt = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
            {
                await operations.SpellCheckWithHttpMessagesAsync(mode, mkt, subscriptionKey, ocpApimSubscriptionKey, null, cancellationToken).ConfigureAwait(false);
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='mode'>
            /// Mode of spellcheck:
            /// &lt;ul&gt;&lt;li&gt;Proof - Meant to provide Office Word like spelling
            /// corrections. It can correct long queries, provide casing corrections and
            /// suppresses aggressive corrections.&lt;/li&gt;
            /// &lt;li&gt;Spell - Meant to provide Search engine like spelling
            /// corrections. It will correct small queries(up to length 9 tokens) without
            /// any casing changes and will be more optimized (perf and relevance)
            /// towards search like queries.&lt;/li&gt;&lt;/ul&gt;
            /// . Possible values include: 'spell', 'proof'
            /// </param>
            /// <param name='preContextText'>
            /// A string that gives context to the text string. For example, the text
            /// string petal is valid; however, if you set preContextText to bike, the
            /// context changes and the text string becomes not valid. In this case, the
            /// API will suggest that you change petal to pedal (as in bike pedal).
            /// </param>
            /// <param name='postContextText'>
            /// A string that gives context to the text string. For example, the text
            /// string read is valid; however, if you set postContextText to carpet, the
            /// context changes and the text string becomes not valid. In this case, the
            /// API will suggest that you change read to red (as in red carpet).
            /// </param>
            /// <param name='mkt'>
            /// For proof mode, only support en-us, es-es, pt-br,
            /// For spell mode, support all language codes. Possible values include:
            /// 'en-us', 'es-es', 'pt-br'
            /// </param>
            /// <param name='subscriptionKey'>
            /// subscription key in url
            /// </param>
            /// <param name='ocpApimSubscriptionKey'>
            /// subscription key in header
            /// </param>
            public static void SpellCheck(this ISpellCheckAPIV5 operations, string mode = default(string), string preContextText = default(string), string postContextText = default(string), string mkt = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string))
            {
                System.Threading.Tasks.Task.Factory.StartNew(s => ((ISpellCheckAPIV5)s).SpellCheckAsync(mode, preContextText, postContextText, mkt, subscriptionKey, ocpApimSubscriptionKey), operations, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskCreationOptions.None,  System.Threading.Tasks.TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='mode'>
            /// Mode of spellcheck:
            /// &lt;ul&gt;&lt;li&gt;Proof - Meant to provide Office Word like spelling
            /// corrections. It can correct long queries, provide casing corrections and
            /// suppresses aggressive corrections.&lt;/li&gt;
            /// &lt;li&gt;Spell - Meant to provide Search engine like spelling
            /// corrections. It will correct small queries(up to length 9 tokens) without
            /// any casing changes and will be more optimized (perf and relevance)
            /// towards search like queries.&lt;/li&gt;&lt;/ul&gt;
            /// . Possible values include: 'spell', 'proof'
            /// </param>
            /// <param name='preContextText'>
            /// A string that gives context to the text string. For example, the text
            /// string petal is valid; however, if you set preContextText to bike, the
            /// context changes and the text string becomes not valid. In this case, the
            /// API will suggest that you change petal to pedal (as in bike pedal).
            /// </param>
            /// <param name='postContextText'>
            /// A string that gives context to the text string. For example, the text
            /// string read is valid; however, if you set postContextText to carpet, the
            /// context changes and the text string becomes not valid. In this case, the
            /// API will suggest that you change read to red (as in red carpet).
            /// </param>
            /// <param name='mkt'>
            /// For proof mode, only support en-us, es-es, pt-br,
            /// For spell mode, support all language codes. Possible values include:
            /// 'en-us', 'es-es', 'pt-br'
            /// </param>
            /// <param name='subscriptionKey'>
            /// subscription key in url
            /// </param>
            /// <param name='ocpApimSubscriptionKey'>
            /// subscription key in header
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async System.Threading.Tasks.Task SpellCheckAsync(this ISpellCheckAPIV5 operations, string mode = default(string), string preContextText = default(string), string postContextText = default(string), string mkt = default(string), string subscriptionKey = default(string), string ocpApimSubscriptionKey = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
            {
                await operations.SpellCheckWithHttpMessagesAsync(mode, preContextText, postContextText, mkt, subscriptionKey, ocpApimSubscriptionKey, null, cancellationToken).ConfigureAwait(false);
            }

    }
}