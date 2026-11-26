from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[3]


def read_file(path: str) -> str:
    file_path = (REPO_ROOT / path).resolve()

    if not file_path.is_relative_to(REPO_ROOT):
        raise ValueError("Access outside repository is forbidden")

    if file_path.suffix != ".cs":
        raise ValueError("Only C# files are allowed")

    return file_path.read_text(encoding="utf-8")

READ_FILE_TOOL = {
    "name": "read_file",
    "description": "Read the content of a C# source file in the repository.",
    "input_schema": {
        "type": "object",
        "properties": {
            "path": {
                "type": "string",
                "description": "File path relative to the repository root"
            }
        },
        "required": ["path"]
    }
}