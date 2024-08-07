// <auto-generated/>
using BrickApi.Client.Api.Set.ById;
using BrickApi.Client.Api.Set.ByName;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace BrickApi.Client.Api.Set
{
    /// <summary>
    /// Builds and executes requests for operations under \api\set
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class SetRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The byId property</summary>
        public global::BrickApi.Client.Api.Set.ById.ByIdRequestBuilder ById
        {
            get => new global::BrickApi.Client.Api.Set.ById.ByIdRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The byName property</summary>
        public global::BrickApi.Client.Api.Set.ByName.ByNameRequestBuilder ByName
        {
            get => new global::BrickApi.Client.Api.Set.ByName.ByNameRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="global::BrickApi.Client.Api.Set.SetRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public SetRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/set", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::BrickApi.Client.Api.Set.SetRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public SetRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/set", rawUrl)
        {
        }
    }
}
