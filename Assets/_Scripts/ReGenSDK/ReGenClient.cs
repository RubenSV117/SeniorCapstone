using System;
using System.Threading.Tasks;
using Firebase.Auth;
using JetBrains.Annotations;
using ReGenSDK.Service;
using ReGenSDK.Service.Impl;

namespace ReGenSDK
{
    [UsedImplicitly]
    public class ReGenClient
    {
        public static ReGenClient Instance
        {
            get => _instance;
            private set
            {
                if (_instance != null) throw new InvalidOperationException();
                _instance = value;
            }
        }

        private readonly string endpoint;
        private readonly Func<Task<string>> authorizationProvider;
        private static ReGenClient _instance = Initialize("https://regenapi.azurewebsites.net");


        public static ReGenClient Initialize([NotNull] string endpoint)
        {
            return Initialize(endpoint, () =>
            {
                var user = FirebaseAuth.DefaultInstance.CurrentUser;
                if (user == null)
                {
                    throw new ArgumentNullException($"FirebaseAuth.CurrentUser");
                }
                return user.TokenAsync(false);
            });
        }

        public static ReGenClient Initialize([NotNull] string endpoint,
            [NotNull] Func<Task<string>> authorizationProvider)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (authorizationProvider == null) throw new ArgumentNullException(nameof(authorizationProvider));
            return new ReGenClient(endpoint, authorizationProvider);
        }

        public ReGenClient(string endpoint, Func<Task<string>> authorizationProvider)
        {
            this.endpoint = endpoint;
            this.authorizationProvider = authorizationProvider;
            if (Instance == null)
                Instance = this;
        }

        public AuthenticationService Authentication => new AuthenticationServiceImpl();

        public FavoriteService Favorites =>
            new FavoriteServiceImpl(endpoint + "/api/Favorites", authorizationProvider);

        public RatingService Ratings =>
            new RatingServiceImpl(endpoint + "/api/Ratings", authorizationProvider);

        public RecipeService Recipes =>
            new RecipeServiceImpl(endpoint + "/api/Recipes", authorizationProvider);

        public ReviewService Reviews =>
            new ReviewServiceImpl(endpoint + "/api/Reviews", authorizationProvider);

        public SearchService Search =>
            new SearchServiceImpl(endpoint + "/api/Search", authorizationProvider);
    }
    
}