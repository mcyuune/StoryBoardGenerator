from anthropic import Anthropic
from shared.tools.file_tools import READ_FILE_TOOL, read_file
from rich.console import Console
from rich.markdown import Markdown

def main():
    client = Anthropic()

    messages = [
        {
            "role": "user",
            "content": (
                "Read Services/SceneSplitter/RuleSplitGenerator.cs "
                "and suggest unit tests for it."
            )
        }
    ]

    response = client.messages.create(
    model="claude-haiku-5-5",
    max_tokens=500,
    tools=[READ_FILE_TOOL],
    messages=messages
    )

    messages.append({
        "role": "assistant",
        "content": response.content
    })

    tool_results = []

    # Execute the requested tool
    for block in response.content:
        if block.type == "tool_use":

            if block.name == "read_file":
                path = block.input["path"]

            if path != "Services/SceneSplitter/RuleSplitGenerator.cs":
                raise ValueError("File is not allowed")

            console.print(f"[cyan]Tool requested:[/cyan] {block.name}")
            console.print(f"[yellow]Reading file:[/yellow] {path}")

            result = read_file(path)

            tool_results.append({
                "type": "tool_result",
                "tool_use_id": block.id,
                "content": result
            })

    if not tool_results:
        console.print("[red]No tools were executed[/red]")
        return

    messages.append({
        "role": "user",
        "content": tool_results
    })

    final_response = client.messages.create(
        model="claude-haiku-5-5",
        max_tokens=4000,
        tools=[READ_FILE_TOOL],
        tool_choice={"type": "none"},
        messages=messages
    )

    for block in final_response.content:
        if block.type == "text":
            console.print(Markdown(block.text))

    console.print(final_response.stop_reason)
    console.print(final_response.usage)

if __name__ == "__main__":
    console = Console()
    main()