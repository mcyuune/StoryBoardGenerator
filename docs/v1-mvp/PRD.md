# V1 — Intelligent Storyboard Studio
Status: Draft — proposed product, not implemented

## Product goal
Understand a complete book before generating a visually coherent sketchbook-style storyboard.

## User workflow
1. Create a story workspace and upload a complete book.
2. Analyze the text to identify chapters, characters and relevant places.
3. Review and correct extracted information; optionally create and approve character sheets and choose illustration style.
4. Once analysis is ready, explicitly click Generate.
5. Generate ordered scene illustrations using story context, approved character references and style.
6. Browse numbered panels with short captions in a paginated storyboard.

## Requirements
- Preserve original text and chapter/scene order.
- Persist extracted entities and allow corrections.
- Separate analysis from illustration generation; no automatic full storyboard generation on upload.
- Show analysis and generation progress and failures.
- Use consistent visual references; perfect character identity is not guaranteed.
- Compare flat grid and open sketchbook layouts (initial design target: eight panels per desktop spread).

## Proposed technical design
Keep the C# ASP.NET Core backend; incrementally introduce React and TypeScript with HTTP APIs. Use structured AI outputs for extraction and reference-aware image generation. Persist book, chapter, character, place, scene and generation metadata. Docker is a proposed deployment and learning step.

## Acceptance criteria
A complete supported book can be uploaded, analyzed, reviewed and explicitly generated into an ordered storyboard. Failures are visible and the user can retry without losing approved metadata.

## Out of scope
Distributed book ingestion, animation production and guaranteed character consistency.

## Learning goals
REST, structured AI outputs, prompt design, persistence, testing, background processing and Docker.

## Open decisions
Supported book formats and size limits, analysis strategy for long books, reference image API, storage, generation granularity and layout.
