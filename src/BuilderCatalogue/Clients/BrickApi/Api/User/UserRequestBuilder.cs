// <auto-generated/>
using BrickApi.Client.Api.User.ById;
using BrickApi.Client.Api.User.ByUsername;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace BrickApi.Client.Api.User
{
    /// <summary>
    /// Builds and executes requests for operations under \api\user
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class UserRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The byId property</summary>
        public global::BrickApi.Client.Api.User.ById.ByIdRequestBuilder ById
        {
            get => new global::BrickApi.Client.Api.User.ById.ByIdRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The byUsername property</summary>
        public global::BrickApi.Client.Api.User.ByUsername.ByUsernameRequestBuilder ByUsername
        {
            get => new global::BrickApi.Client.Api.User.ByUsername.ByUsernameRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="global::BrickApi.Client.Api.User.UserRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public UserRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/user", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::BrickApi.Client.Api.User.UserRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public UserRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/user", rawUrl)
        {
        }
    }
}
