# StoryBoard Generator

**Work in progress** — a personal ASP.NET Core application that turns short stories into illustrated storyboards using AI-generated images.

## What it does

1. Enter a story title and story text.
2. Split the text into scenes using a rule-based scene splitter.
3. Generate an illustration for each scene using the OpenAI Images API.
4. Store story and scene metadata in SQLite, and save generated images locally.
5. Browse saved stories and their illustrations in a gallery.

The current scene splitter uses paragraph breaks (or sentences when appropriate) and supports up to eight scenes per story.

## Tech stack

- C# / .NET 10
- ASP.NET Core Razor Pages
- Entity Framework Core with SQLite
- OpenAI Images API
- Dependency injection and `HttpClient`

## Architecture

The application separates story orchestration, scene splitting, and image generation into services. The `ISceneSplitter` and `IImageGenerator` interfaces make these responsibilities replaceable.

This is an evolving learning project, not a production-ready service.

## Run locally

**Prerequisites:** .NET 10 SDK and an OpenAI API key. Generating images uses the OpenAI API and may incur charges.

```bash
git clone https://github.com/mcyuune/StoryBoardGenerator.git
cd StoryBoardGenerator
dotnet restore
dotnet user-secrets set "OpenAI:ApiKey" "YOUR_OPENAI_API_KEY"
dotnet run
```

Open **http://localhost:5088** in your browser (using the repository's development launch profile).

The SQLite database is created automatically on startup. Generated images are saved locally under `wwwroot/uploads/`.

**Security:** Never commit API keys or other secrets. The example above uses .NET User Secrets for local development.

## Current limitations

- Scene splitting is rule-based, not AI-powered.
- Illustration generation happens as part of story creation.
- Images and the SQLite database are stored locally.
- The interface and workflow are still being developed.

## Roadmap

Planned improvements include chapter-based story editing, story-level illustration styles, and a React + TypeScript frontend. See the repository's GitHub Issues for ongoing work.
