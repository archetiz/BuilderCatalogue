// <auto-generated/>
using BrickApi.Client.Api.Colours;
using BrickApi.Client.Api.Set;
using BrickApi.Client.Api.Sets;
using BrickApi.Client.Api.User;
using BrickApi.Client.Api.Users;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace BrickApi.Client.Api
{
    /// <summary>
    /// Builds and executes requests for operations under \api
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class ApiRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The colours property</summary>
        public global::BrickApi.Client.Api.Colours.ColoursRequestBuilder Colours
        {
            get => new global::BrickApi.Client.Api.Colours.ColoursRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The set property</summary>
        public global::BrickApi.Client.Api.Set.SetRequestBuilder Set
        {
            get => new global::BrickApi.Client.Api.Set.SetRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The sets property</summary>
        public global::BrickApi.Client.Api.Sets.SetsRequestBuilder Sets
        {
            get => new global::BrickApi.Client.Api.Sets.SetsRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The user property</summary>
        public global::BrickApi.Client.Api.User.UserRequestBuilder User
        {
            get => new global::BrickApi.Client.Api.User.UserRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The users property</summary>
        public global::BrickApi.Client.Api.Users.UsersRequestBuilder Users
        {
            get => new global::BrickApi.Client.Api.Users.UsersRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="global::BrickApi.Client.Api.ApiRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ApiRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::BrickApi.Client.Api.ApiRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ApiRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api", rawUrl)
        {
        }
    }
}
