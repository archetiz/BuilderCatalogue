kiota generate \
	--language CSharp \
	--class-name BrickApiClient \
	--namespace-name BrickApi.Client \
	--openapi ./OpenAPI/brickapi-spec.json \
	--output ./Clients/BrickApi \
	--exclude-backward-compatible \
	--clean-output \
	--serializer Microsoft.Kiota.Serialization.Json.JsonSerializationWriterFactory \
	--deserializer Microsoft.Kiota.Serialization.Json.JsonParseNodeFactory
