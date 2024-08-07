// <auto-generated/>
using BrickApi.Client.Api.User.ById.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace BrickApi.Client.Api.User.ById
{
    /// <summary>
    /// Builds and executes requests for operations under \api\user\by-id
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.16.0")]
    public partial class ByIdRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the BrickApi.Client.api.user.byId.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::BrickApi.Client.Api.User.ById.Item.ByIdItemRequestBuilder"/></returns>
        public global::BrickApi.Client.Api.User.ById.Item.ByIdItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("id", position);
                return new global::BrickApi.Client.Api.User.ById.Item.ByIdItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::BrickApi.Client.Api.User.ById.ByIdRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ByIdRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/user/by-id", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::BrickApi.Client.Api.User.ById.ByIdRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ByIdRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/api/user/by-id", rawUrl)
        {
        }
    }
}
