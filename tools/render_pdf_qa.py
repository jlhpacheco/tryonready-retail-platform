"""Render a PDF to temporary PNG pages for visual QA."""

from pathlib import Path
import sys

import pypdfium2 as pdfium


def main() -> None:
    if len(sys.argv) != 3:
        raise SystemExit("usage: render_pdf_qa.py INPUT_PDF OUTPUT_DIRECTORY")

    input_path = Path(sys.argv[1])
    output_directory = Path(sys.argv[2])
    output_directory.mkdir(parents=True, exist_ok=True)

    pdf = pdfium.PdfDocument(input_path)
    for index in range(len(pdf)):
        page = pdf[index]
        image = page.render(scale=1.6).to_pil()
        image.save(output_directory / f"page-{index + 1:02d}.png")

    print(f"Rendered {len(pdf)} pages.")


if __name__ == "__main__":
    main()
