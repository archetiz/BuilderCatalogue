# BuilderCatalogue tools

This folder contains the auxiliary tooling for the application.

## OpenAPI Specification generation

Generally, the tools are centered around generating an OpenAPI specification document, that can later be used to generate an API client for the actual application.

The whole process can be divided into 3 steps:

1. Record OpenAPI spec by using DevProxy.
2. Clean up the automatically generated document to be usable for code generation.
3. Generate API Client in the project.

### Prerequisites

#### Install DevProxy

Detailed installation instructions can be found in the relevant Microsoft documentation: [Get started with Dev Proxy](https://learn.microsoft.com/en-us/microsoft-cloud/dev/dev-proxy/get-started?tabs=automated&pivots=client-operating-system-windows#install-dev-proxy).

### Run the API spec generation

Simply run the following script in a Bash-compatible shell.
```bash
./generate-openapi-spec.sh
```

The script above launches DevProxy and calls each API endpoint one by one, so the request/result specifications can be recorded.

**Important:** When the recording is finished, the script will prompt you to stop the run by pressing CTRL + C. This step is required for the specification to get properly saved.

### Cleanup

The OpenAPI specification generated by DevProxy is quite good, but not perfect, and there is a bit of clean up required for it to be usable. There is a custom cleanup script prepared in this folder, that will ready the created document for actual code generation. To launch the cleanup simply run the following:
```bash
./clean-api-spec.sh the_name_of_the_generated_json_document_in_the_previous_step.json
```

The output document will have an `out_` prefix.

### Code generation prerequisites

Make sure Kiota is installed on your computer, either by running `dotnet tool restore` in the project folder (`/src/BuilderCatalogue`) or installing it as a global tool according to the official documentation: [Install Kiota](https://learn.microsoft.com/en-us/openapi/kiota/install?tabs=bash#install-as-net-tool).

### Code generation

Once the clean OpenAPI spec is ready, copy it to the `/src/BuilderCatalogue/OpenAPI` folder, with the name `brickapi-spec.json`.

Check the parameters inside `generate-openapi-client.sh` in the project (`/src/BuilderCatalogue`) directory, and if it matches your desires and it correctly points to the previously created file, run the script:
```bash
./generate-openapi-client.sh
```

It should generate the C# source files for the API client under the configured directory (by default `/Clients`).

The generator uses the [Kiota tool](https://learn.microsoft.com/en-us/openapi/kiota/overview) for API generation and the created code will also depend on the _Microsoft.Kiota_ packages.

### Configuration files

The folder has two configuration files, that can be used to configure the process, these are:
- `config.sh`: contains common configuration variables for the generation and cleanup scripts.
- `generate-openapi-spec-config.json`: contains the configuration for DevProxy.
