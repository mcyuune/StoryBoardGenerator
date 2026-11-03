# StoryBoard Generator

ASP.NET Core Razor Pages app: paste a story, split it into scenes, save a picture per scene in SQLite + files, browse them in a gallery.

Images are placeholders (`PlaceholderImageGenerator`) so the app runs without an API key. The `IImageGenerator` interface is the swap point for a real model later.

## One-time setup (this Mac)

1. **.NET 10 SDK** — https://dotnet.microsoft.com/download/dotnet/10.0  
   Confirm with `dotnet --version`.
2. **Git** — macOS needs Command Line Tools. In Terminal: `xcode-select --install`, then:
   ```bash
   cd ~/StoryBoardGenerator
   git init
   git add .
   git commit -m "Initial StoryBoardGenerator scaffold"
   ```

## Run

```bash
cd ~/StoryBoardGenerator
dotnet restore
dotnet run
```

Open http://localhost:5088 — New story, submit text, then Gallery.

Blank lines in the story become separate scenes (up to 8).
