# BuilderCatalogue

BuilderCatalogue is a simple .Net web application created as a small demo, that completes various assignments, that can be called via its exposed API.

Swagger UI is available in the running application to be able to easily call the endpoints, that return the solutions for each assignment.

## Building & running the application

The application can be built and run as a standard .Net web application without any other special prerequisites, either via the dotnet CLI or using an IDE (e.g. VisualStudio).

Alternatively, the application also supplies a Dockerfile in the project (`/src/BuilderCatalogue`) folder, that can be used to run the application via Docker.

## API client generation

The API client (re)generation involves specific steps. For details, check the readme in the `/tools` directory.

## Questions

During development, I've ran into some questions that were unclear from the problem description. I tried to note these down to in the `QUESTIONS.md` file.
